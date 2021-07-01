using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Randomize Divination", menuName = "Custom Data/Divinations/Randomize Divination", order = 1)]
public class RandomizeRiverDivination : DivinationData
{
    public override void Execute(UnitData unitData, List<RiverCard> selectedRiverCards)
    {
        HandleCardStates(selectedRiverCards);
        BattleManager.Instance.RandomizeRiver();
    }
}