using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Flow Divination", menuName = "Custom Data/Divinations/Flow Divination", order = 1)]
public class FlowRiverDivination : DivinationData
{
    [SerializeField, Range(1, 5)] int _flowAmount;
    public int flowAmount {
        get { return _flowAmount; }
    }

    public override void Execute(UnitData unitData, List<RiverCard> selectedRiverCards)
    {
        HandleCardStates(selectedRiverCards);
        BattleManager.Instance.FlowRiver(flowAmount);
    }
}