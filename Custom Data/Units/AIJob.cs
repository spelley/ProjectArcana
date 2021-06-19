using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

struct AIJob : IJobParallelFor
{
    [ReadOnly]
    public Vector3Int originPosition;
    [ReadOnly]
    public int originIndex;
    [ReadOnly]
    public NativeArray<Vector3Int> walkableArea;
    [ReadOnly]
    public NativeArray<SkillStruct> skills;
    [ReadOnly]
    public NativeArray<Vector3Int> combatants;
    [ReadOnly]
    public NativeHashMap<Vector3Int, int> combatantIndexByPosition;
    [ReadOnly]
    public NativeHashMap<Vector3Int, Faction> factionsByCombatant;
    [ReadOnly]
    public NativeArray<int> scoresByCombatantSkill;
    [ReadOnly]
    public NativeMultiHashMap<int, Vector3Int> targetableAreasByCombatantSkill;
    [ReadOnly]
    public NativeMultiHashMap<int, Vector3Int> skillShapes;
    // TODO: Make this a struct with all the details
    [NativeDisableParallelForRestriction]
    public NativeQueue<SkillScore>.ParallelWriter scores;

    int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        checked
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
    }

    int ManhattanDistance(Vector3Int a, Vector3Int b)
    {
        checked
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
    }

    int GetScore(int cIdx, int skillIdx)
    {
        return scoresByCombatantSkill[GetScoreIndex(cIdx, skillIdx)];
    }

    int GetScore(int index)
    {
        return scoresByCombatantSkill[index];
    }

    int GetScoreIndex(int cIdx, int skillIdx)
    {
        return (skills.Length * cIdx) + skillIdx;
    }

    // The code actually running on the job
    public void Execute(int walkIndex)
    {
        Vector3Int walkCell = walkableArea[walkIndex];
        
        for(int skillIdx = 0; skillIdx < skills.Length; skillIdx++)
        {
            SkillStruct skill = skills[skillIdx];

            // these simply use Manhattan distance to let us know if they are within range and thus are targetable
            // single targets make their own individual score, while "ALL" gets the tally of hitting everyone in range
            if(skill.rangeType == RangeType.AREA && (skill.targetShape == TargetShape.ALL || skill.targetShape == TargetShape.SINGLE))
            {     
                int totalScore = 0;
                Vector3Int allTarget = new Vector3Int(0, 0, 0);
                for(int cIdx = 0; cIdx < combatants.Length; cIdx++)
                {
                    // this makes it so we act as though we are in our new location, not our original one
                    Vector3Int targetPosition = cIdx == originIndex ? walkCell : combatants[cIdx];

                    // physically impossible to be close enough to the opponent, skip
                    if(ManhattanDistance(walkCell, targetPosition) <= skill.range && Mathf.Abs(walkCell.z - targetPosition.z) <= skill.heightTolerance)
                    {
                        if(skill.targetShape == TargetShape.ALL)
                        {
                            int score = GetScore(cIdx, skillIdx);
                            if(score > 0)
                            {
                                allTarget = targetPosition;
                            }
                            totalScore += score;
                        }
                        else
                        {
                            scores.Enqueue(new SkillScore(walkCell, targetPosition, skillIdx, GetScore(cIdx, skillIdx)));
                        }
                    }
                }
                if(skill.targetShape == TargetShape.ALL && totalScore > 0)
                {
                    scores.Enqueue(new SkillScore(walkCell, allTarget, skillIdx, totalScore));
                }
            }
            else if(skill.rangeType == RangeType.AREA)
            {
                Vector3Int allTarget = new Vector3Int(0, 0, 0);
                for(int cIdx = 0; cIdx < combatants.Length; cIdx++)
                {
                    int testScore = GetScore(cIdx, skillIdx);
                    if(testScore <= 0)
                    {
                        continue;
                    }
                    Vector3Int offset = new Vector3Int(0, 0, 0);
                    if(targetableAreasByCombatantSkill.TryGetFirstValue(GetScoreIndex(cIdx, skillIdx), out offset, out var iterator)) {
                        do {
                            // this makes it so we act as though we are in our new location, not our original one
                            Vector3Int targetPosition = (cIdx == originIndex ? walkCell : combatants[cIdx]) + offset;
                            if(ManhattanDistance(walkCell, targetPosition) <= skill.range)
                            { 
                                int totalScore = 0;
                                Vector3Int shapeOffset = new Vector3Int(0, 0, 0);
                                if(skillShapes.TryGetFirstValue(skillIdx, out shapeOffset, out var shapeIterator)) {
                                    do {
                                        Vector3Int shapeTargetPosition = targetPosition + shapeOffset;
                                        // make sure the target is within range from our current position, and if this contains a combatant
                                        if(combatantIndexByPosition.ContainsKey(shapeTargetPosition))
                                        {
                                            int score = GetScore(combatantIndexByPosition[shapeTargetPosition], skillIdx);
                                            totalScore += score;
                                        }
                                        else if(shapeTargetPosition == walkCell)
                                        {
                                            int score = GetScore(originIndex, skillIdx);
                                            totalScore += score;
                                        }
                                    } while(skillShapes.TryGetNextValue(out shapeOffset, ref shapeIterator));
                                }
                                scores.Enqueue(new SkillScore(walkCell, targetPosition, skillIdx, totalScore));
                            }
                        } while(targetableAreasByCombatantSkill.TryGetNextValue(out offset, ref iterator));
                    }
                }
            }
            else if(skill.rangeType == RangeType.LINE && (skill.targetShape == TargetShape.ALL || skill.targetShape == TargetShape.SINGLE))
            {
                int northScore = 0;
                Vector3Int allTargetNorth = new Vector3Int(0, 0, 0);
                int eastScore = 0;
                Vector3Int allTargetEast = new Vector3Int(0, 0, 0);
                int southScore = 0;
                Vector3Int allTargetSouth = new Vector3Int(0, 0, 0);
                int westScore = 0;
                Vector3Int allTargetWest = new Vector3Int(0, 0, 0);

                for(int cIdx = 0; cIdx < combatants.Length; cIdx++)
                {
                    // this makes it so we act as though we are in our new location, not our original one
                    Vector3Int targetPosition = cIdx == originIndex ? walkCell : combatants[cIdx];
                    
                    // they are within range of us and within our height tolerance, so now we just determine which direction they are pointing from us
                    if(ManhattanDistance(walkCell, targetPosition) <= skill.range && Mathf.Abs(walkCell.z - targetPosition.z) <= skill.heightTolerance)
                    {
                        if(skill.targetShape == TargetShape.ALL)
                        {
                            // they are aligned with us
                            if((walkCell.x == targetPosition.x))
                            {
                                // check if they are to the north/south
                                if(walkCell.y < targetPosition.y)
                                {
                                    allTargetNorth = targetPosition;
                                    northScore += GetScore(cIdx, skillIdx);
                                }
                                else
                                {
                                    allTargetSouth = targetPosition;
                                    southScore += GetScore(cIdx, skillIdx);
                                }
                            }
                            else if(walkCell.y == targetPosition.y)
                            {
                                // check if they are to the east/west
                                if(walkCell.x < targetPosition.x)
                                {
                                    allTargetEast = targetPosition;
                                    eastScore += GetScore(cIdx, skillIdx);
                                }
                                else
                                {
                                    allTargetWest = targetPosition;
                                    westScore += GetScore(cIdx, skillIdx);
                                }
                            }
                        }
                        else
                        {
                            scores.Enqueue(new SkillScore(walkCell, targetPosition, skillIdx, GetScore(cIdx, skillIdx)));
                        }
                    }
                }

                if(skill.targetShape == TargetShape.ALL)
                {
                    if(northScore > 0)
                    {
                        scores.Enqueue(new SkillScore(walkCell, allTargetNorth, skillIdx, northScore));
                    }
                    if(southScore > 0)
                    {
                        scores.Enqueue(new SkillScore(walkCell, allTargetSouth, skillIdx, southScore));
                    }
                    if(eastScore > 0)
                    {
                        scores.Enqueue(new SkillScore(walkCell, allTargetEast, skillIdx, eastScore));
                    }
                    if(westScore > 0)
                    {
                        scores.Enqueue(new SkillScore(walkCell, allTargetWest, skillIdx, westScore));
                    }
                }
            }
        }
    }
}