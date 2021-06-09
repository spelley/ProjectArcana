using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "All Enemies Defeated", menuName = "Custom Data/Encounter Condition/All Enemies Defeated")]
public class AllEnemiesDefeatedCondition : EncounterCondition
{
    public override bool IsCompleted()
    {
        foreach(UnitData enemy in BattleManager.Instance.enemies)
        {
            if(!enemy.incapacitated)
            {
                return false;
            }
        }
        return true;
    }
}
