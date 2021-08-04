using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack Effect", menuName = "Custom Data/Skill Effects/Attack Effect")]
public class AttackEffect : SkillEffect
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
            if(skill.IsHit(skill.GetHitChance(unitData, target, false)))
            {
                int matches = BattleManager.Instance.GetRiverMatches(skill.elements);
                int baseDamage = skill.skillCalculation.Calculate(skill, unitData, matches);
                unitData.DealDamage(baseDamage, target, skill.elements);
                skill.HandlePush(unitData, target);
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
            int baseDamage = skill.skillCalculation.Calculate(skill, unitData, matches);
            int predictDamage = unitData.PredictDealDamage(baseDamage, target, skill.elements);
            int toHit = skill.GetHitChance(unitData, target, false);
            string output = predictDamage >= 0 ? predictDamage.ToString()+" Damage" : "+"+predictDamage.ToString()+"HP";
            
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
        if(target != null && skill.IsValidTargetType(unitData, target))
        {
            int matches = BattleManager.Instance.GetRiverMatches(skill.elements);
            int baseDamage = skill.skillCalculation.Calculate(skill, unitData, matches);
            int predictDamage = unitData.PredictDealDamage(baseDamage, target, skill.elements);
            int toHit = unitData.GetHitChance(skill.hitChance, target, skill.elements);

            predictDamage = toHit == 0 ? 0 : Mathf.RoundToInt(predictDamage * (toHit / 100f));

            if(predictDamage >= target.hp)
            {
                return predictDamage * 4;
            }
            return predictDamage * (target.faction == unitData.faction ? -2 : 1);
        }
        return 0;
    }
}