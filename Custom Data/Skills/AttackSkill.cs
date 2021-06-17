using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackSkill", menuName = "Custom Data/Skill Data/Attack Skill", order = 1)]
public class AttackSkill : SkillData
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
            int matches = BattleManager.Instance.GetRiverMatches(this.elements);
            int baseDamage = skillCalculation.Calculate(this, unitData, matches);
            unitData.DealDamage(baseDamage, gridCell.occupiedBy, this.elements);
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
            int matches = BattleManager.Instance.GetRiverMatches(this.elements);
            int baseDamage = skillCalculation.Calculate(this, unitData, matches);
            int predictDamage = unitData.PredictDealDamage(baseDamage, target, this.elements) * (target.faction == unitData.faction ? -1 : 1);
            if(predictDamage >= target.hp)
            {
                return predictDamage * 4;
            }
            return predictDamage;
        }
        return 0;
    }
}