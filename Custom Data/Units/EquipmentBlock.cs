using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentBlock
{
    [SerializeField]
    WeaponData weapon;
    [SerializeField]
    WeaponData offhand;
    [SerializeField]
    EquipmentData helmet;
    [SerializeField]
    EquipmentData armor;
    [SerializeField]
    EquipmentData accessory;

    public event Action<EquipmentData, UnitData, bool> OnEquip;
    public event Action<EquipmentData, UnitData, bool> OnUnequip;

    public void Load(UnitData unitData)
    {
        if(weapon != null)
        {
            Equip(weapon, unitData, false, false);
        }
        if(offhand != null)
        {
            Equip(offhand, unitData, true, false);
        }
        if(helmet != null)
        {
            Equip(helmet, unitData, false, false);
        }
        if(armor != null)
        {
            Equip(armor, unitData, false, false);
        }
        if(accessory != null)
        {
            Equip(accessory, unitData, false, false);
        }
    }

    public void Equip(EquipmentData equipment, UnitData unitData, bool offhandSlot = false, bool invokeEvents = true)
    {
        WeaponData weaponData = equipment as WeaponData;
        if(weaponData != null)
        {
            if(offhandSlot)
            {
                if(offhand != null)
                {
                    Unequip(offhand, unitData, offhandSlot);
                }
                offhand = weaponData;
            }
            else
            {
                if(weapon != null)
                {
                    Unequip(weapon, unitData, offhandSlot);
                }
                weapon = weaponData;
            }     
        }
        else
        {
            switch(equipment.equipmentSlot)
            {
                case EquipmentSlot.HELMET:
                    if(helmet != null)
                    {
                        Unequip(helmet, unitData, offhandSlot);
                    }
                    helmet = equipment;
                break;
                case EquipmentSlot.ARMOR:
                    if(armor != null)
                    {
                        Unequip(armor, unitData, offhandSlot);
                    }
                    armor = equipment;
                break;
                case EquipmentSlot.ACCESSORY:
                    if(accessory != null)
                    {
                        Unequip(accessory, unitData, offhandSlot);
                    }
                    accessory = equipment;
                break;
            }
        }
        equipment.Equip(unitData);
        unitData.RecalculateResources();
        if(invokeEvents)
        {
            OnEquip?.Invoke(equipment, unitData, offhandSlot);
        }
    }

    public void Unequip(EquipmentData equipment, UnitData unitData, bool offhandSlot = false)
    {
        switch(equipment.equipmentSlot)
        {
            case EquipmentSlot.WEAPON:
                WeaponData weaponData = equipment as WeaponData;
                if(weaponData != null)
                {
                    if(offhandSlot)
                    {
                        offhand = null;
                    }
                    else
                    {
                        weapon = null;
                    }
                }
            break;
            case EquipmentSlot.HELMET:
                helmet = null;
            break;
            case EquipmentSlot.ARMOR:
                armor = null;
            break;
            case EquipmentSlot.ACCESSORY:
                accessory = null;
            break;
        }

        OnUnequip?.Invoke(equipment, unitData, offhandSlot);
    }
}
