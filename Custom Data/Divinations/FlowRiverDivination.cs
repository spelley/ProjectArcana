using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Flow Divination", menuName = "Custom Data/Divinations/Flow Divination", order = 1)]
public class FlowRiverDivination : DivinationData, ILoadable<FlowRiverDivinationSaveData>
{
    string _flowLoadType = "Divination";
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

    public FlowRiverDivinationSaveData GetSaveData()
    {
        FlowRiverDivinationSaveData saveData = new FlowRiverDivinationSaveData();
        saveData.id = _id;
        saveData.name = _divinationName;
        saveData.description = _description;
        saveData.riverSelectType = (int)_riverSelectType;
        saveData.minSelection = _minSelection;
        saveData.maxSelection = _maxSelection;
        saveData.lockSelections = _lockSelections;
        saveData.unlockSelections = _unlockSelections;
        saveData.deactivateSelections = _deactivateSelections;
        saveData.activateSelections = _activateSelections;
        saveData.instructionsText = _instructionsText;
        saveData.castAnimation = (int)_castAnimation;
        saveData.executeAnimation = "";
        saveData.flowAmount = _flowAmount;

        Debug.Log(JsonUtility.ToJson(saveData));

        return saveData;
    }

    public bool LoadFromSaveData(FlowRiverDivinationSaveData saveData)
    {
        _id = saveData.id;
        _divinationName = saveData.name;
        _description = saveData.description;
        _riverSelectType = (RiverSelectType)saveData.riverSelectType;
        _minSelection = saveData.minSelection;
        _maxSelection = saveData.maxSelection;
        _lockSelections = saveData.lockSelections;
        _unlockSelections = saveData.unlockSelections;
        _deactivateSelections = saveData.deactivateSelections;
        _activateSelections = saveData.activateSelections;
        _instructionsText = saveData.instructionsText;
        _castAnimation = (BattleAnimation)saveData.castAnimation;
        _executeAnimation = null;
        _flowAmount = saveData.flowAmount;

        return true;
    }
}