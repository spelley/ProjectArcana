using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackSkill", menuName = "Custom Data/Skill Data/Attack Skill", order = 1)]
public class AttackSkill : SkillData
{
    string _attackLoadType = "Attack Skill";
    public override string loadType { get { return _attackLoadType; } }
    public override void Execute(UnitData unitData, List<GridCell> targets)
    {
        foreach(GridCell gridCell in targets)
        {
            ExecutePerTarget(unitData, gridCell);
            executedOn.Add(unitData);
        }
        executedOn.Clear();
    }

    public override void ExecutePerTarget(UnitData unitData, GridCell gridCell)
    {
        UnitData target = gridCell.occupiedBy;
        if(target != null && !executedOn.Contains(target))
        {
            if(IsHit(GetHitChance(unitData, target, false)))
            {
                int matches = BattleManager.Instance.GetRiverMatches(this.elements);
                int baseDamage = skillCalculation.Calculate(this, unitData, matches);
                unitData.DealDamage(baseDamage, target, this.elements);
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
            int baseDamage = skillCalculation.Calculate(this, unitData, matches);
            int predictDamage = unitData.PredictDealDamage(baseDamage, target, this.elements);
            int toHit = GetHitChance(unitData, target, false);

            string output = predictDamage >= 0 ? predictDamage.ToString()+" Damage" : "+"+predictDamage.ToString()+"HP";
            if(push != 0)
            {
                output += "\nPush "+push.ToString();
            }

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
            int baseDamage = skillCalculation.Calculate(this, unitData, matches);
            int predictDamage = unitData.PredictDealDamage(baseDamage, target, this.elements);
            int toHit = unitData.GetHitChance(this.hitChance, target, this.elements);

            predictDamage = toHit == 0 ? 0 : Mathf.RoundToInt(predictDamage * (toHit / 100f));

            if(predictDamage >= target.hp)
            {
                return predictDamage * 4;
            }
            return predictDamage * (target.faction == unitData.faction ? -2 : 1);
        }
        return 0;
    }

    public override SkillSaveData GetSaveData()
    {
        SkillSaveData saveData = base.GetSaveData();
        saveData.loadType = loadType;

        return saveData;
    }
}