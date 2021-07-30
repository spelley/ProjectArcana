using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class WeaponSaveData : EquipmentSaveData
{
    public string weaponSkillID;
    public int weaponAttack;
    public bool twoHanded;
    public bool offHandOnly;
}