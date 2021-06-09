using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicAttackCalculation", menuName = "Custom Data/Skill Calculations/BA Calculation", order = 1)]
public class BasicAttackCalculation : SkillCalculation
{    
    public override int Calculate(SkillData skillData, UnitData unitData)
    {
        Vector3Int av = GetAttributeValues(skillData, unitData);
        return skillData.primaryValue * ((av.x + av.y + av.z) / 3);
    }
}