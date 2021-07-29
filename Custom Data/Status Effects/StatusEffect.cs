using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect : ScriptableObject
{
    [SerializeField] string _id;
    public string id { get { return _id; } }
    public string statusName;
    public Sprite statusIcon;

    public abstract void Apply(UnitData unitData);

    public abstract void Remove(UnitData unitData);
}