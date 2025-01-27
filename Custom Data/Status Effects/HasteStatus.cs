using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Haste", menuName = "Custom Data/Status Effect/Haste")]
public class HasteStatus : StatusEffect
{
    [SerializeField]
    float hasteMultiplier = 1.5f;

    [SerializeField]
    StatusEffect neutralizedBy;
    
    public override void Apply(UnitData unitData)
    {
        unitData.stats.OnCalculateMove += OnCalculateMove;
        unitData.stats.OnCalculateJump += OnCalculateMove;

        if(unitData.HasStatus(neutralizedBy))
        {
            unitData.RemoveStatus(neutralizedBy);
            unitData.RemoveStatus(this);
        }
    }

    public void OnCalculateMove(ModInt modifiedMove, Stat stat)
    {
        modifiedMove.resultMultiplier += hasteMultiplier;
    }

    public override void Remove(UnitData unitData)
    {
        unitData.stats.OnCalculateMove -= OnCalculateMove;
        unitData.stats.OnCalculateJump -= OnCalculateMove;
    }
}
