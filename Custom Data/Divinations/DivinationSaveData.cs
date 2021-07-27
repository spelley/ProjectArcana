using System;

[Serializable]
public class DivinationSaveData : ISaveData
{
    public string id;
    public string ID { get { return id; } }
    public string loadType;
    public string LoadType { get { return loadType; } }
    public string name;
    public string description;
    public int riverSelectType;
    public int minSelection;
    public int maxSelection;
    public bool lockSelections;
    public bool unlockSelections;
    public bool deactivateSelections;
    public bool activateSelections;
    public string instructionsText;
    public int castAnimation;
    public string executeAnimation;
}