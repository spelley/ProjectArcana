using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Haste", menuName = "Custom Data/Status Effect/Haste")]
public class HasteStatus : StatusEffect
{
    [SerializeField]
    private float hasteMultiplier = 1.5f;

    [SerializeField]
    private StatusEffect neutralizedBy;
    
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

    public void OnCalculateMove(ModInt modifiedMove)
    {
        modifiedMove.resultMultiplier += hasteMultiplier;
    }

    public override void Remove(UnitData unitData)
    {
        unitData.stats.OnCalculateMove -= OnCalculateMove;
        unitData.stats.OnCalculateJump -= OnCalculateMove;
    }
}
