using System;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;

[CreateAssetMenu(fileName = "AIBrain", menuName = "Custom Data/AI Brain Data", order = 1)]
public class AIBrain : ScriptableObject
{
    public string id;
    public string brainName;
    public string description;

    AIJob aiJob;
    NativeArray<SkillStruct> skillsStruct;
    NativeArray<Vector3Int> combatantPositions;
    NativeHashMap<Vector3Int, int> combatantIndexByPosition;
    NativeHashMap<Vector3Int, Faction> factionsByCombatant;
    NativeMultiHashMap<int, Vector3Int> targetableAreasByCombatantSkill;
    NativeMultiHashMap<int, Vector3Int> skillShapes;
    NativeArray<int> skillScoresByCombatantSkill;
    NativeArray<Vector3Int> walkablePositions;
    JobHandle jobHandle;
    Stopwatch stopWatch;
    Action<AIInstruction> callbackAction;
    NativeQueue<SkillScore> scores;
    NativeQueue<SkillScore>.ParallelWriter scoresParallel;
    UnitData unit;
    List<SkillData> skills;

    public int ManhattanDistance(Vector3Int a, Vector3Int b)
    {
        checked
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
    }

    public int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        checked
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
    }

    public void DetermineAction(UnitData unitData, Action<AIInstruction> callback)
    {
        stopWatch = new Stopwatch();
        stopWatch.Start();
        // get all the data we are working with
        skills = unitData.GetAvailableSkills(true);
        List<UnitData> combatants = BattleManager.Instance.GetCombatants();
        List<GridCell> walkableCells = MapManager.Instance.GetWalkableCells();
        walkableCells.Add(MapManager.Instance.GetCell(unitData.curPosition));
        unit = unitData;
        callbackAction = callback;

        // allocate the memory for our shared data native containers
        int originIndex = -1;
        skillsStruct = new NativeArray<SkillStruct>(skills.Count, Allocator.TempJob);
        combatantPositions = new NativeArray<Vector3Int>(combatants.Count, Allocator.TempJob);
        combatantIndexByPosition = new NativeHashMap<Vector3Int, int>(combatants.Count, Allocator.TempJob);
        factionsByCombatant = new NativeHashMap<Vector3Int, Faction>(combatants.Count, Allocator.TempJob);
        targetableAreasByCombatantSkill = new NativeMultiHashMap<int, Vector3Int>(skills.Count * combatants.Count, Allocator.TempJob);
        skillShapes = new NativeMultiHashMap<int, Vector3Int>(skills.Count, Allocator.TempJob);
        skillScoresByCombatantSkill = new NativeArray<int>(skills.Count * combatants.Count, Allocator.TempJob);
        walkablePositions = new NativeArray<Vector3Int>(walkableCells.Count, Allocator.TempJob);

        // convert over the walkable positions to the native array
        for(int w = 0; w < walkableCells.Count; w++)
        {
            if(walkableCells[w].occupiedBy == null)
            {
                walkablePositions[w] = walkableCells[w].position;
            }
        }

        // loop through every combatant that we could want to target
        // and figure out the positions we could target between them 
        int index = 0;
        for(int cIdx = 0; cIdx < combatants.Count; cIdx++)
        {
            UnitData combatant = combatants[cIdx];
            if(combatant == unitData)
            {
                originIndex = cIdx;
            }
            combatantPositions[cIdx] = combatant.curPosition;
            combatantIndexByPosition.Add(combatant.curPosition, cIdx);
            factionsByCombatant[combatant.curPosition] = combatant.faction;
            // iterate through all the skills to get their data
            for(int skillIdx = 0; skillIdx < skills.Count; skillIdx++)
            {
                SkillData skill = skills[skillIdx];
                if(cIdx == 0)
                {
                    skillsStruct[skillIdx] = skill.GetSkillStruct();
                    List<Vector3Int> shapeOffsets = SkillTargetShape.GetTargetShape(skill);
                    for(int shapeIdx = 0; shapeIdx < shapeOffsets.Count; shapeIdx++)
                    {
                        skillShapes.Add(skillIdx, shapeOffsets[shapeIdx]);
                    }
                }
                // get all the valid targetable areas around this combatant
                List<Vector3Int> offsets = SkillOffsets.GetTargetOffsets(skill, combatant.curPosition);
                for(int oIdx = 0; oIdx < offsets.Count; oIdx++)
                {
                    targetableAreasByCombatantSkill.Add(index, offsets[oIdx]);
                }
                // build up the array of skill scores
                if(skill.skillEffect != null)
                {
                    skillScoresByCombatantSkill[index] = skill.skillEffect.GetSkillScore(skill, unitData, MapManager.Instance.GetCell(combatant.curPosition));
                }
                else
                {
                    skillScoresByCombatantSkill[index] = skill.GetSkillScore(unitData, MapManager.Instance.GetCell(combatant.curPosition));
                }
                
                index++;
            }
        }

        scores = new NativeQueue<SkillScore>(Allocator.TempJob);
        NativeQueue<SkillScore>.ParallelWriter scoresParallel = scores.AsParallelWriter();

        aiJob = new AIJob();
        aiJob.originPosition = unitData.curPosition;
        aiJob.originIndex = originIndex;
        aiJob.skills = skillsStruct;
        aiJob.factionsByCombatant = factionsByCombatant;
        aiJob.combatants = combatantPositions;
        aiJob.combatantIndexByPosition = combatantIndexByPosition;
        aiJob.targetableAreasByCombatantSkill = targetableAreasByCombatantSkill;
        aiJob.scoresByCombatantSkill = skillScoresByCombatantSkill;
        aiJob.walkableArea = walkablePositions;
        aiJob.skillShapes = skillShapes;
        aiJob.scores = scoresParallel;

        jobHandle = aiJob.Schedule(walkablePositions.Length, 8);

        GameManager.Instance.StartCoroutine(WaitForExecution(FinishAI));
    }

    public void FinishAI()
    {
        jobHandle.Complete();

        int maxScore = 0;
        int maxScoreSkillIndex = -1;
        int maxDistanceScore = 0;
        Vector3Int resultWalk = Vector3Int.zero;
        Vector3Int resultTarget = Vector3Int.zero;

        UnityEngine.Debug.Log("Scores: "+ scores.Count);

        if(scores.Count > 0)
        {
            while(scores.Count > 0)
            {
                SkillScore skillScore = scores.Dequeue();
                if(MapManager.Instance.CheckIfTargetable(skillScore.target)) {
                    if((skillScore.score > maxScore) || (skillScore.score == maxScore && skillScore.distanceScore > maxDistanceScore))
                    {
                        maxScore = skillScore.score;
                        maxScoreSkillIndex = skillScore.skillIndex;
                        maxDistanceScore = skillScore.distanceScore;
                        resultWalk = skillScore.origin;
                        resultTarget = skillScore.target;
                    }
                }
            }
        }
        
        // dispose of all the memory
        skillsStruct.Dispose();
        combatantPositions.Dispose();
        combatantIndexByPosition.Dispose();
        factionsByCombatant.Dispose();
        targetableAreasByCombatantSkill.Dispose();
        skillShapes.Dispose();
        skillScoresByCombatantSkill.Dispose();
        walkablePositions.Dispose();
        scores.Dispose();

        // get our time spent calculating
        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;

        // Format and display the TimeSpan value.
        string elapsedTime = String.Format("{3:000}ms",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds);
        UnityEngine.Debug.Log("RunTime " + elapsedTime);

        if(maxScore <= 0)
        {
            callbackAction.Invoke(new AIInstruction(MapManager.Instance.GetCell(unit.curPosition), null, null));
        }
        else
        {
            callbackAction.Invoke(new AIInstruction(MapManager.Instance.GetCell(resultWalk), skills[maxScoreSkillIndex], MapManager.Instance.GetCell(resultTarget)));
        }

        skills.Clear();
        unit = null;
        callbackAction = null;
    }

    public IEnumerator WaitForExecution(Action callback)
    {
        yield return 0;
        yield return 0;
        yield return 0;
        callback.Invoke();
    }
}
