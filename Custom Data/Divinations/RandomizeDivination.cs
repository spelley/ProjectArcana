using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Randomize Divination", menuName = "Custom Data/Divinations/Randomize Divination", order = 1)]
public class RandomizeRiverDivination : DivinationData, ILoadable<DivinationSaveData>
{
    public override void Execute(UnitData unitData, List<RiverCard> selectedRiverCards)
    {
        HandleCardStates(selectedRiverCards);
        BattleManager.Instance.RandomizeRiver();
    }

    public DivinationSaveData GetSaveData()
    {
        DivinationSaveData saveData = new DivinationSaveData();
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

        Debug.Log(JsonUtility.ToJson(saveData));

        return saveData;
    }

    public bool LoadFromSaveData(DivinationSaveData saveData)
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

        return true;
    }
}