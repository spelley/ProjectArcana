using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillTargetShape
{
    public static List<Vector3Int> GetTargetShape(SkillData skill)
    {
        List<Vector3Int> offsets = new List<Vector3Int>();
        switch(skill.targetShape)
        {
            case TargetShape.SINGLE:
                offsets.Add(new Vector3Int(0, 0, 0));
            break;
            case TargetShape.ALL:
                if(skill.rangeType == RangeType.LINE)
                {
                    offsets.Add(new Vector3Int(0, 0, 0));
                    for(int r = 1; r < skill.range; r++)
                    {
                        for(int h = 0; h <= skill.heightTolerance; h++)
                        {
                            offsets.Add(new Vector3Int(r, 0, h));
                            offsets.Add(new Vector3Int(-r, 0, h));
                            offsets.Add(new Vector3Int(0, r, h));
                            offsets.Add(new Vector3Int(0, -r, h));
                            if(h > 0)
                            {
                                offsets.Add(new Vector3Int(r, 0, -h));
                                offsets.Add(new Vector3Int(-r, 0, -h));
                                offsets.Add(new Vector3Int(0, r, -h));
                                offsets.Add(new Vector3Int(0, -r, -h));
                            }
                        }
                    }
                }
                else
                {
                    // we don't use offsets for "ALL" target shapes
                    offsets.Add(new Vector3Int(0, 0, 0));
                }
            break;
            case TargetShape.CROSS:
                offsets.Add(new Vector3Int(0, 0, 0));
                Debug.Log(skill.areaOfEffect);
                for(int a = 1; a <= skill.areaOfEffect; a++)
                {
                    int targetXA = a;
                    int targetXB = -a;
                    for(int i = 0; i <= skill.heightTolerance; i++)
                    {
                        offsets.Add(new Vector3Int(targetXA, 0, i));
                        offsets.Add(new Vector3Int(0, targetXA, i));
                        offsets.Add(new Vector3Int(targetXB, 0, i));
                        offsets.Add(new Vector3Int(0, targetXB, i));
                        if(i != 0)
                        {
                            offsets.Add(new Vector3Int(targetXA, 0, -i));
                            offsets.Add(new Vector3Int(0, targetXA, -i));
                            offsets.Add(new Vector3Int(targetXB, 0, -i));
                            offsets.Add(new Vector3Int(0, targetXB, -i));
                        }
                    }
                }
            break;
        }
        return offsets;
    }

    public static List<Vector3Int> GetTargetShape(SkillData skill, GridCell originCell, GridCell targetCell, List<GridCell> targetableArea)
    {
        List<Vector3Int> offsets = new List<Vector3Int>();
        switch(skill.targetShape)
        {
            case TargetShape.SINGLE:
                offsets.Add(new Vector3Int(0, 0, 0));
            break;
            case TargetShape.ALL:
                if(skill.rangeType == RangeType.LINE)
                {
                    if(skill.IsSelfTargeting())
                    {
                        offsets.Add(originCell.position - targetCell.position);
                    }
                    foreach(GridCell gridCell in targetableArea)
                    {
                        if(Mathf.Abs(gridCell.position.z - originCell.position.z) > skill.heightTolerance)
                        {
                            continue;
                        }

                        if(targetCell.position.x > originCell.position.x
                            && targetCell.position.y == originCell.position.y
                            && gridCell.position.y == originCell.position.y
                            && gridCell.position.x > originCell.position.x)
                        {
                            offsets.Add(new Vector3Int(gridCell.position.x - targetCell.position.x, 0, gridCell.position.z - targetCell.position.z));
                        }
                        else if(targetCell.position.x < originCell.position.x 
                            && targetCell.position.y == originCell.position.y
                            && gridCell.position.y == originCell.position.y 
                            && gridCell.position.x < originCell.position.x)
                        {
                            offsets.Add(new Vector3Int(gridCell.position.x - targetCell.position.x, 0, gridCell.position.z - targetCell.position.z));
                        }
                        else if(targetCell.position.y > originCell.position.y
                            && targetCell.position.x == originCell.position.x
                            && gridCell.position.x == originCell.position.x 
                            && gridCell.position.y > originCell.position.y)
                        {
                            offsets.Add(new Vector3Int(0, gridCell.position.y - targetCell.position.y, gridCell.position.z - targetCell.position.z));
                        }
                        else if(targetCell.position.y < originCell.position.y
                            && targetCell.position.x == originCell.position.x
                            && gridCell.position.x == originCell.position.x
                            && gridCell.position.y < originCell.position.y)
                        {
                            offsets.Add(new Vector3Int(0, gridCell.position.y - targetCell.position.y, gridCell.position.z - targetCell.position.z));
                        }
                    }
                }
                else
                {
                    foreach(GridCell gridCell in targetableArea)
                    {
                        offsets.Add(gridCell.position - targetCell.position);
                    }
                }
            break;
            case TargetShape.CROSS:
                offsets.Add(new Vector3Int(0, 0, 0));
                for(int a = 1; a <= skill.areaOfEffect; a++)
                {
                    int targetXA = a;
                    int targetXB = -a;
                    for(int i = 0; i <= skill.heightTolerance; i++)
                    {
                        offsets.Add(new Vector3Int(targetXA, 0, i));
                        offsets.Add(new Vector3Int(0, targetXA, i));
                        offsets.Add(new Vector3Int(targetXB, 0, i));
                        offsets.Add(new Vector3Int(0, targetXB, i));
                        if(i != 0)
                        {
                            offsets.Add(new Vector3Int(targetXA, 0, -i));
                            offsets.Add(new Vector3Int(0, targetXA, -i));
                            offsets.Add(new Vector3Int(targetXB, 0, -i));
                            offsets.Add(new Vector3Int(0, targetXB, -i));
                        }
                    }
                }
            break;
        }

        if(!skill.IsSelfTargeting())
        {
            Vector3Int selfOffset = originCell.position - targetCell.position;
            int idx = offsets.IndexOf(selfOffset);
            if(idx > -1)
            {
                offsets.RemoveAt(idx);
            }
        }
        return offsets;
    }
}