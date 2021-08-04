using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Custom Data/Items/Weapon Data")]
public class WeaponData: EquipmentData
{
    string _loadWeaponType = "Weapon";
    public override string loadType { get { return _loadWeaponType; } }

    [Header("Weapon Information")]
    [SerializeField]
    SkillData _weaponSkill;
    public SkillData weaponSkill 
    {
        get { return _weaponSkill; }
        private set { _weaponSkill = value; }
    }

    [SerializeField]
    int _weaponAttack;
    public int weaponAttack
    {
        get { return _weaponAttack; }
        private set { _weaponAttack = value; }
    }

    [SerializeField]
    bool _twoHanded;
    public bool twoHanded
    {
        get { return _twoHanded; }
        private set { _twoHanded = value; }
    }

    [SerializeField]
    bool _offhandOnly;
    public bool offhandOnly
    {
        get { return _offhandOnly; }
        private set { _offhandOnly = value; }
    }

    public override ItemSaveData GetSaveData()
    {
        WeaponSaveData saveData = new WeaponSaveData();
        saveData.id = id.ToString();
        saveData.loadType = loadType;
        saveData.name = itemName;
        saveData.description = description;
        saveData.equipmentSlot = _equipmentSlot.ToString();
        saveData.statBonuses = bonusStats;

        saveData.statusEffectIDs = new string[equipStatusEffects.Count];
        for(int i = 0; i < equipStatusEffects.Count; i++)
        {
            saveData.statusEffectIDs[i] = equipStatusEffects[i].id;
        }
        
        if(_weaponSkill != null)
        {
            saveData.weaponSkillID = _weaponSkill.id;
        }
        saveData.weaponAttack = _weaponAttack;
        saveData.twoHanded = _twoHanded;
        saveData.offHandOnly = _offhandOnly;

        Debug.Log(JsonUtility.ToJson(saveData));

        return saveData;
    }

    public override bool LoadFromSaveData(ItemSaveData baseSaveData)
    {
        WeaponSaveData saveData = (WeaponSaveData) baseSaveData;
        _id = saveData.id;
        _loadType = saveData.loadType;
        _description = saveData.description;
        _itemName = saveData.name;
        _equipmentSlot = (EquipmentSlot)System.Enum.Parse(typeof(EquipmentSlot), saveData.equipmentSlot);
        _bonusStats = saveData.statBonuses;

        _equipStatusEffects.Clear();
        foreach(string statusEffectID in saveData.statusEffectIDs)
        {
            StatusEffect status = SaveDataLoader.Instance.GetStatusEffect(statusEffectID);
            if(status != null)
            {
                equipStatusEffects.Add(status);
            }
        }

        _weaponSkill = SaveDataLoader.Instance.GetSkillData(saveData.weaponSkillID);
        _weaponAttack = saveData.weaponAttack;
        _twoHanded = saveData.twoHanded;
        _offhandOnly = saveData.offHandOnly;

        return true;
    }
}