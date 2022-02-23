using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Flow Divination", menuName = "Custom Data/Divinations/Flow Divination", order = 1)]
public class FlowRiverDivination : DivinationData
{
    string _flowLoadType = "Flow";
    public override string loadType { get { return _flowLoadType; } }

    [SerializeField, Range(1, 5)] int _flowAmount;
    public int flowAmount {
        get { return _flowAmount; }
    }

    public override void Execute(UnitData unitData, List<RiverCard> selectedRiverCards)
    {
        HandleCardStates(selectedRiverCards);
        BattleManager.Instance.FlowRiver(flowAmount);
    }

    public override DivinationSaveData GetSaveData()
    {
        DivinationSaveData saveData = base.GetSaveData();
        saveData.loadType = loadType;
        saveData.flowAmount = _flowAmount;

        // Debug.LogJsonUtility.ToJson(saveData));

        return saveData;
    }

    public override bool LoadFromSaveData(DivinationSaveData saveData)
    {
        base.LoadFromSaveData(saveData);
        // Debug.LogsaveData.ToString());
        _flowAmount = saveData.flowAmount;

        return true;
    }
}