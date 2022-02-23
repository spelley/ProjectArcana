using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Trap Effect", menuName = "Custom Data/Skill Effects/Trap Effect")]
public class TrapEffect : SkillEffect
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
        if(target == null && gridCell != null && unitData != null)
        {
            GameObject trapObjectPrefab = BattleManager.Instance.GetTrap();
            GameObject trapObject = Instantiate(trapObjectPrefab, gridCell.realWorldPosition, Quaternion.identity);
            trapObject.GetComponent<TrapSetter>().SetTrap(skill.associatedSkill, unitData, gridCell);
        }
    }

    public override SkillPreview GetPreview(SkillData skill, UnitData unitData, GridCell targetCell)
    {
        UnitData target = targetCell != null ? targetCell.occupiedBy : null;
        if(targetCell != null && target == null)
        {
            string output = "Place "+skill.associatedSkill.skillName+" Trap";

            return new SkillPreview(output, 100);
        }

        return new SkillPreview("N/A", 0);
    }

    public override int GetSkillScore(SkillData skill, UnitData unitData, GridCell gridCell)
    {
        if(gridCell.occupiedBy == null)
        {
            return GetSkillScore(skill.associatedSkill, unitData, gridCell.occupiedBy);
        }
        return 0;
    }

    public override int GetSkillScore(SkillData skill, UnitData unitData, UnitData target)
    {
        if(target == null)
        {
            // TODO: properly score this
            return 25;
        }
        return 0;
    }
}