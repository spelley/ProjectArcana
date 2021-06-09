using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Allies Defeated", menuName = "Custom Data/Encounter Condition/Allies Defeated")]
public class AlliesDefeatedCondition : EncounterCondition
{
    public override bool IsCompleted()
    {
        foreach(UnitData ally in BattleManager.Instance.party)
        {
            if(!ally.incapacitated)
            {
                return false;
            }
        }
        return true;
    }
}
