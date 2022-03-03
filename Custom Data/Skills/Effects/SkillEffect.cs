using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillEffect : ScriptableObject, IStaticData
{
    [SerializeField] string _id;
    public string id { get { return _id; } }
    public abstract void ExecutePerTarget(SkillData skill, UnitData unitData, GridCell gridCell, Action callback);
    public abstract int GetSkillScore(SkillData skill, UnitData unitData, UnitData target);
    public abstract int GetSkillScore(SkillData skill, UnitData unitData, GridCell gridCell);
    public abstract SkillPreview GetPreview(SkillData skill, UnitData unitData, GridCell targetCell);
}