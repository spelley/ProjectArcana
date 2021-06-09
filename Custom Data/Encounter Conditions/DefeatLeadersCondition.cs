using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Defeat Leaders", menuName = "Custom Data/Encounter Condition/Defeat Leaders")]
public class DefeatLeadersCondition : EncounterCondition
{
    public override bool IsCompleted()
    {
        foreach(UnitData leader in leaders)
        {
            if(!leader.incapacitated)
            {
                return false;
            }
        }
        return true;
    }
}