using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillOffsets
{
    public static void TryAddOffset(Vector3Int targetPosition, Vector3Int offset, List<Vector3Int> offsets)
    {
        if(MapManager.Instance.CheckIfTargetable(targetPosition.x + offset.x, targetPosition.y + offset.y, targetPosition.z + offset.z))
        {
            offsets.Add(offset);
        }
    }

    public static List<Vector3Int> GetTargetOffsets(SkillData skillData, Vector3Int targetPosition)
    {
        List<Vector3Int> offsets = new List<Vector3Int>();
        switch(skillData.targetShape)
        {
            case TargetShape.SINGLE:
                SkillOffsets.TryAddOffset(targetPosition, new Vector3Int(0, 0, 0), offsets);
            break;
            case TargetShape.ALL:
                if(skillData.rangeType == RangeType.LINE)
                {
                    SkillOffsets.TryAddOffset(targetPosition, new Vector3Int(0, 0, 0), offsets);
                    for(int r = 1; r < skillData.range; r++)
                    {
                        for(int h = 0; h <= skillData.heightTolerance; h++)
                        {
                            SkillOffsets.TryAddOffset(targetPosition, new Vector3Int(r, 0, h), offsets);
                            SkillOffsets.TryAddOffset(targetPosition, new Vector3Int(-r, 0, h), offsets);
                            SkillOffsets.TryAddOffset(targetPosition, new Vector3Int(0, r, h), offsets);
                            SkillOffsets.TryAddOffset(targetPosition, new Vector3Int(0, -r, h), offsets);
                            if(h > 0)
                            {
                                SkillOffsets.TryAddOffset(targetPosition, new Vector3Int(r, 0, -h), offsets);
                                SkillOffsets.TryAddOffset(targetPosition, new Vector3Int(-r, 0, -h), offsets);
                                SkillOffsets.TryAddOffset(targetPosition, new Vector3Int(0, r, -h), offsets);
                                SkillOffsets.TryAddOffset(targetPosition, new Vector3Int(0, -r, -h), offsets);
                            }
                        }
                    }
                }
                else
                {
                    // we don't use offsets for "ALL" target shapes
                    SkillOffsets.TryAddOffset(targetPosition, new Vector3Int(0, 0, 0), offsets);
                }
            break;
            case TargetShape.CROSS:
                SkillOffsets.TryAddOffset(targetPosition, new Vector3Int(0, 0, 0), offsets);
                for(int a = 1; a <= skillData.areaOfEffect; a++)
                {
                    int targetXA = a;
                    int targetXB = -a;
                    for(int i = 0; i <= skillData.heightTolerance; i++)
                    {
                        SkillOffsets.TryAddOffset(targetPosition, new Vector3Int(targetXA, 0, i), offsets);
                        SkillOffsets.TryAddOffset(targetPosition, new Vector3Int(0, targetXA, i), offsets);
                        SkillOffsets.TryAddOffset(targetPosition, new Vector3Int(targetXB, 0, i), offsets);
                        SkillOffsets.TryAddOffset(targetPosition, new Vector3Int(0, targetXB, i), offsets);
                        if(i != 0)
                        {
                            SkillOffsets.TryAddOffset(targetPosition, new Vector3Int(targetXA, 0, -i), offsets);
                            SkillOffsets.TryAddOffset(targetPosition, new Vector3Int(0, targetXA, -i), offsets);
                            SkillOffsets.TryAddOffset(targetPosition, new Vector3Int(targetXB, 0, -i), offsets);
                            SkillOffsets.TryAddOffset(targetPosition, new Vector3Int(0, targetXB, -i), offsets);
                        }
                    }
                }
            break;
        }
        return offsets;
    }
}