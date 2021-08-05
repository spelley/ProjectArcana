using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Heal Effect", menuName = "Custom Data/Skill Effects/Heal Effect")]
public class HealEffect : SkillEffect
{
    public override void Execute(SkillData skill, UnitData unitData, List<GridCell> targets)
    {
        foreach(GridCell gridCell in targets)
        {
            ExecutePerTarget(skill, unitData, gridCell);
        }
    }

    public override void ExecutePerTarget(SkillData skill, UnitData unitData, GridCell gridCell)
    {
        UnitData target = gridCell.occupiedBy;
        if(target != null)
        {
            if(skill.IsHit(skill.GetHitChance(unitData, target, unitData.faction == target.faction)) && skill.IsValidTargetType(unitData, target))
            {
                int matches = BattleManager.Instance.GetRiverMatches(skill.elements);
                int baseHeal = skill.skillCalculation.Calculate(skill, unitData, matches);
                unitData.HealOther(baseHeal, target, skill.elements);
                skill.HandlePush(unitData, target);
                skill.executedOn.Add(unitData);
            }
            else
            {
                target.Miss();
            }
        }
    }

    public override SkillPreview GetPreview(SkillData skill, UnitData unitData, GridCell targetCell)
    {
        UnitData target = targetCell.occupiedBy;
        if(target != null)
        {
            int matches = BattleManager.Instance.GetRiverMatches(skill.elements);
            int baseHeal = skill.skillCalculation.Calculate(skill, unitData, matches);
            int predictHeal = unitData.PredictHealOther(baseHeal, target, skill.elements);
            int toHit = skill.GetHitChance(unitData, target, unitData.faction == target.faction);
            string output = predictHeal >= 0 ? "+"+predictHeal.ToString()+"HP" : "-"+predictHeal.ToString()+"HP";
            
            if(skill.push != 0)
            {
                output += "\nPush "+skill.push.ToString();
            }

            return new SkillPreview(output, toHit);
        }

        return new SkillPreview("No Effect", 0);
    }

    public override int GetSkillScore(SkillData skill, UnitData unitData, GridCell gridCell)
    {
        if(gridCell.occupiedBy != null)
        {
            return GetSkillScore(skill, unitData, gridCell.occupiedBy);
        }
        return 0;
    }

    public override int GetSkillScore(SkillData skill, UnitData unitData, UnitData target)
    {
        if(target != null && skill.IsValidTargetType(unitData, target) && !target.incapacitated)
        {
            int matches = BattleManager.Instance.GetRiverMatches(skill.elements);
            int baseHeal = skill.skillCalculation.Calculate(skill, unitData, matches);
            int toHit = unitData.GetHitChance(skill.hitChance, target, skill.elements);
            int predictHeal = unitData.PredictHealOther(baseHeal, target, skill.elements) * (target.faction == unitData.faction ? 1 : -1);
            if(target.faction == unitData.faction && target.hp <= (target.maxHP * .7f))
            {
                predictHeal *= 2;
            }
            predictHeal = toHit == 0 ? 0 : Mathf.RoundToInt(predictHeal * (toHit / 100f));

            return predictHeal;
        }
        return 0;
    }
}