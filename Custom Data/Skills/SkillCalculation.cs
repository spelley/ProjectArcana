using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillCalculation: ScriptableObject
{
    [SerializeField] string _id;
    public string id { get { return _id; } }

    [SerializeField]
    string _calculationName;
    public string calculationName
    { 
        get
        {
            return _calculationName;
        } 
        private set
        {
            _calculationName = value;
        }
    }

    [SerializeField]
    string _description;
    public string description
    { 
        get
        {
            return _description;
        } 
        private set
        {
            _description = value;
        }
    }

    public Vector3Int GetAttributeValues(SkillData skillData, UnitData unitData)
    {
        int pAV = unitData.stats.GetByStat(skillData.primaryAttribute);
        int sAV = skillData.secondaryAttribute != skillData.primaryAttribute ? unitData.stats.GetByStat(skillData.secondaryAttribute) : pAV;
        int tAV = 0;
        if(skillData.tertiaryAttribute == skillData.primaryAttribute)
        {
            tAV = pAV;
        }
        else if(skillData.tertiaryAttribute == skillData.secondaryAttribute)
        {
            tAV = sAV;
        }
        else
        {
            tAV = unitData.stats.GetByStat(skillData.tertiaryAttribute);
        }

        return new Vector3Int(pAV, sAV, tAV);
    }
    
    public abstract int Calculate(SkillData skillData, UnitData unitData, int numMatches = 0);
}