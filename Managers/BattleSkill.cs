using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSkill
{
    public SkillData skillData;
    public UnitData unitData;
    public List<GridCell> targets;

    public BattleSkill() {}

    public BattleSkill(SkillData _skill, UnitData _unitData, List<GridCell> _targets)
    {
        skillData = _skill;
        unitData = _unitData;
        targets = _targets;
    }
}