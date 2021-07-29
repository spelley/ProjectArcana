using System;

[Serializable]
public class UnitSaveData : ISaveData
{
    public string id;
    public string ID { get { return id; } }
    public string loadType = "Unit";
    public string LoadType { get { return loadType; } }
    public string name;
    public string arcanaID;
    public string unitModelID;
    public string faction;
    public string aiBrainID;
    public StatBlockSaveData statBlock;
    public EquipmentBlockSaveData equipmentBlock;
    public string baseJobID;
    public string activeJobID;
    public UnitJobSaveData[] availableJobs;
    public int ct;
    public bool moved;
    public bool acted;
    public bool usedBonus;
    public string spritePath;
    public string[] learnedSkillIDs;
    public string[] assignedSkillIDs;
    public SimpleVector3Int curPosition;
    public string[] statusEffectIDs;
}