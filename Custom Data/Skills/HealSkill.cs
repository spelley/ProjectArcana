using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealSkill", menuName = "Custom Data/Skill Data/HealSkill", order = 1)]
public class HealSkill : SkillData
{
    public override void Execute(UnitData unitData, List<GridCell> targets)
    {
        foreach(GridCell gridCell in targets)
        {
            ExecutePerTarget(unitData, gridCell);
        }
    }

    public override void ExecutePerTarget(UnitData unitData, GridCell gridCell)
    {
        if(gridCell.occupiedBy != null)
        {
            int baseHeal = skillCalculation.Calculate(this, unitData);
            unitData.HealOther(baseHeal, gridCell.occupiedBy, this.elements);
        }
    }

    public override int GetSkillScore(UnitData unitData, GridCell gridCell)
    {
        if(gridCell.occupiedBy != null)
        {
            return GetSkillScore(unitData, gridCell.occupiedBy);
        }
        return 0;
    }

    public override int GetSkillScore(UnitData unitData, UnitData target)
    {
        if(target != null && IsValidTargetType(unitData, target))
        {
            int baseHeal = skillCalculation.Calculate(this, unitData);
            return unitData.PredictHealOther(baseHeal, target, this.elements) * (target.faction == unitData.faction ? 1 : -1);
        }
        return 0;
    }
}