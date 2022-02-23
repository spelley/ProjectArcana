using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Randomize Divination", menuName = "Custom Data/Divinations/Randomize Divination", order = 1)]
public class RandomizeRiverDivination : DivinationData
{
    string _randomizeLoadType = "Randomize";
    public override string loadType { get { return _randomizeLoadType; } }
    
    public override void Execute(UnitData unitData, List<RiverCard> selectedRiverCards)
    {
        HandleCardStates(selectedRiverCards);
        BattleManager.Instance.RandomizeRiver();
    }

    public override DivinationSaveData GetSaveData()
    {
        DivinationSaveData saveData = base.GetSaveData();
        saveData.loadType = loadType;

        // Debug.LogJsonUtility.ToJson(saveData));
        
        return saveData;
    }

    public override bool LoadFromSaveData(DivinationSaveData saveData)
    {
        base.LoadFromSaveData(saveData);

        return true;
    }
}