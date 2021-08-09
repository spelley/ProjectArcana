using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Improved River Passive", menuName = "Custom Data/Passives/Improved River")]
public class ImprovedRiverPassive : PassiveData
{
    [SerializeField] int _amount;

    public override void Assign(UnitData unitData)
    {
        Unassign(unitData);
        unitData.OnUnitMatches += OnCalculateRiverMatches;
    }

    public override void Unassign(UnitData unitData)
    {
        unitData.OnUnitMatches -= OnCalculateRiverMatches;
    }

    public void OnCalculateRiverMatches(ModInt modInt, UnitData unitData, List<ElementData> elementsToMatch)
    {
        if(modInt.GetCalculated() > 0)
        {
            modInt.resultAdd += _amount;
        }
    }
}