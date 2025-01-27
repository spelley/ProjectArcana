using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicAttackCalculation", menuName = "Custom Data/Skill Calculations/BA Calculation", order = 1)]
public class BasicAttackCalculation : SkillCalculation
{    
    public override int Calculate(SkillData skillData, UnitData unitData, int numMatches = 0)
    {
        Vector3Int av = GetAttributeValues(skillData, unitData);
        int combinedStatTotal = (av.x + av.y + av.z);

        if(combinedStatTotal == 0) {
            return 0;
        }

        int affinity = skillData.elements.Count > 0 && unitData.arcana != null ? Arcana.CompareElements(skillData.elements, unitData.arcana) : 0;
        return Mathf.RoundToInt((skillData.primaryValue + affinity) * (combinedStatTotal / 3f));
    }
}