using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect : ScriptableObject
{
    [SerializeField] string _statusID;
    public string statusID { get { return _statusID; } }
    public string statusName;
    public Sprite statusIcon;

    public abstract void Apply(UnitData unitData);

    public abstract void Remove(UnitData unitData);
}