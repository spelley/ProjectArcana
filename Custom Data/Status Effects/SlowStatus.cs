using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Slow", menuName = "Custom Data/Status Effect/Slow")]
public class SlowStatus : StatusEffect
{
    [SerializeField]
    float slowMultiplier = 0.5f;

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
        modifiedMove.resultMultiplier -= slowMultiplier;
    }

    public override void Remove(UnitData unitData)
    {
        unitData.stats.OnCalculateMove -= OnCalculateMove;
        unitData.stats.OnCalculateJump -= OnCalculateMove;
    }
}