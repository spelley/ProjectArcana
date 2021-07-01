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
        UnitData target = gridCell.occupiedBy;
        if(target != null)
        {
            if(IsHit(GetHitChance(unitData, target, unitData.faction == target.faction)))
            {
                int matches = BattleManager.Instance.GetRiverMatches(this.elements);
                int baseHeal = skillCalculation.Calculate(this, unitData, matches);
                unitData.HealOther(baseHeal, target, this.elements);
                HandlePush(unitData, target);
            }
            else
            {
                target.Miss();
            }
        }
    }

    public override SkillPreview GetPreview(UnitData unitData, GridCell targetCell)
    {
        UnitData target = targetCell.occupiedBy;
        if(target != null)
        {
            int matches = BattleManager.Instance.GetRiverMatches(this.elements);
            int baseHeal = skillCalculation.Calculate(this, unitData, matches);
            int predictHeal = unitData.PredictHealOther(baseHeal, target, this.elements);
            string output = predictHeal >= 0 ? "+"+predictHeal.ToString()+"HP" : "-"+predictHeal.ToString()+"HP";
            if(push != 0)
            {
                output += "\nPush "+push.ToString();
            }

            int toHit = GetHitChance(unitData, target, unitData.faction == target.faction);

            return new SkillPreview(output, toHit);
        }

        return new SkillPreview("No Effect", 0);
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
            int baseHeal = skillCalculation.Calculate(this, unitData, matches);
            int toHit = unitData.GetHitChance(this.hitChance, target, this.elements);
            int predictHeal = unitData.PredictHealOther(baseHeal, target, this.elements) * (target.faction == unitData.faction ? 1 : -1);

            predictHeal = toHit == 0 ? 0 : Mathf.RoundToInt(predictHeal * (toHit / 100f));

            return predictHeal;
        }
        return 0;
    }
}