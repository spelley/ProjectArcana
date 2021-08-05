using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StaticValueCalculation", menuName = "Custom Data/Skill Calculations/Static Value Calculation", order = 1)]
public class StaticValueCalculation : SkillCalculation
{    
    public override int Calculate(SkillData skillData, UnitData unitData, int numMatches = 0)
    {
        return skillData.primaryValue;
    }
}