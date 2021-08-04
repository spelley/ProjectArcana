using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class EquipmentSaveData : ItemSaveData
{
    public string equipmentSlot;
    public List<StatBonus> statBonuses;
    public string[] statusEffectIDs;
}