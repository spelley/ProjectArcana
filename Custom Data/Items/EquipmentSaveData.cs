using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class EquipmentSaveData : ISaveData
{
    public string id;
    public string ID { get { return id; } }
    public string loadType;
    public string LoadType { get { return loadType; } }
    public string name;
    public string description;
    public int equipmentSlot;
    public List<StatBonus> statBonuses;
    public string[] statusEffectIDs;
}