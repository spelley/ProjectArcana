using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassiveData: ScriptableObject, IAssignableSkill
{
    [SerializeField] string _id;
    public string id { get { return _id; } }

    [SerializeField] string _name;
    public string skillName { get { return _name; } }

    [SerializeField] string _description;
    public string description { get { return _description; } }

    [SerializeField] int _spCost;
    public int spCost { get { return _spCost; } }

    public abstract void Assign(UnitData unitData);

    public abstract void Unassign(UnitData unitData);
}