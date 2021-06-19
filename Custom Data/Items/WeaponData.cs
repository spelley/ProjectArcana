using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Custom Data/Items/Weapon Data")]
public class WeaponData: EquipmentData
{
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
}