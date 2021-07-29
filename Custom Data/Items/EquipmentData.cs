using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentData", menuName = "Custom Data/Items/Equipment Data")]
public class EquipmentData: ItemData
{
    string _loadEquipmentType = "Equipment";
    public override string loadType { get { return _loadEquipmentType; } }

    [Header("Equipment Information")]
    [SerializeField]
    EquipmentSlot _equipmentSlot;
    public EquipmentSlot equipmentSlot
    {
        get { return _equipmentSlot; }
        private set { _equipmentSlot = value; }
    }

    [SerializeField]
    List<StatBonus> _bonusStats = new List<StatBonus>();
    public List<StatBonus> bonusStats
    {
        get { return _bonusStats; }
        private set { _bonusStats = value; }
    }

    [SerializeField]
    List<StatusEffect> _equipStatusEffects = new List<StatusEffect>();
    public List<StatusEffect> equipStatusEffects
    {
        get { return _equipStatusEffects; }
        private set { _equipStatusEffects = value; }
    }

    UnitData equippedUnit;

    public void Equip(UnitData unitData)
    {
        equippedUnit = unitData;
        BindEvents(unitData);
    }

    void BindEvents(UnitData unitData)
    {
        foreach(StatBonus statBonus in bonusStats)
        {
            unitData.stats.BindEventToStat(statBonus.stat, OnCalculateStat);
        }

        if(equipStatusEffects.Count > 0)
        {
            BattleManager.Instance.OnEncounterAwake += OnEncounterAwake;
            BattleManager.Instance.OnEncounterEnd += OnEncounterEnd;
        }
    }

    void UnbindEvents(UnitData unitData)
    {
        foreach(StatBonus statBonus in bonusStats)
        {
            unitData.stats.UnbindEventToStat(statBonus.stat, OnCalculateStat);
        }

        if(equipStatusEffects.Count > 0)
        {
            BattleManager.Instance.OnEncounterAwake -= OnEncounterAwake;
            BattleManager.Instance.OnEncounterEnd -= OnEncounterEnd;
        }
    }

    void OnEncounterAwake()
    {
        if(equippedUnit != null)
        {
            foreach(StatusEffect status in equipStatusEffects)
            {
                equippedUnit.AddStatus(status);
            }
        }
    }

    void OnEncounterEnd()
    {
        if(equippedUnit != null)
        {
            foreach(StatusEffect status in equipStatusEffects)
            {
                equippedUnit.RemoveStatus(status);
            }
        }
    }

    void OnCalculateStat(ModInt mod, Stat stat)
    {
        foreach(StatBonus statBonus in bonusStats)
        {
            if(statBonus.stat == stat)
            {
                mod.resultAdd += statBonus.amount;
            }
        }
    }

    public void Unequip(UnitData unitData)
    {
        equippedUnit = null;
        UnbindEvents(unitData);
    }

    public EquipmentSaveData GetEquipmentSaveData()
    {
        EquipmentSaveData saveData = new EquipmentSaveData();
        saveData.id = itemID.ToString();
        saveData.loadType = loadType;
        saveData.name = itemName;
        saveData.description = description;
        saveData.equipmentSlot = (int)equipmentSlot;
        saveData.statBonuses = bonusStats;

        saveData.statusEffectIDs = new string[equipStatusEffects.Count];
        for(int i = 0; i < equipStatusEffects.Count; i++)
        {
            saveData.statusEffectIDs[i] = equipStatusEffects[i].id;
        }

        Debug.Log(JsonUtility.ToJson(saveData));

        return saveData;
    }
}