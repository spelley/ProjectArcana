using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JobData", menuName = "Custom Data/Job Data")]
public class JobData: ScriptableObject
{
    [SerializeField] string _jobID;
    public string jobID 
    {
        get 
        {
            return _jobID;
        }
        private set 
        {
            _jobID = value;
        }
    }

    [SerializeField] string _jobName;
    public string jobName 
    {
        get 
        {
            return _jobName;
        }
        private set 
        {
            _jobName = value;
        }
    }

    [SerializeField] string _description;
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

    [SerializeField] Sprite _icon;
    public Sprite icon 
    {
        get 
        {
            return _icon;
        }
        private set 
        {
            _icon = value;
        }
    }

    [Header("Stat Multipliers")]
    [SerializeField, Range(0.5f, 2f)] float _hpMult;
    public float hpMult { get { return _hpMult; } }
    [SerializeField, Range(0.5f, 2f)] float _mpMult;
    public float mpMult { get { return _mpMult; } }
    [SerializeField, Range(0.5f, 2f)] float _bodyMult;
    public float bodyMult { get { return _bodyMult; } }
    [SerializeField, Range(0.5f, 2f)] float _mindMult;
    public float mindMult { get { return _mindMult; } }
    [SerializeField, Range(0.5f, 2f)] float _spiritMult;
    public float spiritMult { get { return _spiritMult; } }
    [SerializeField, Range(0.5f, 2f)] float _speedMult;
    public float speedMult { get { return _speedMult; } }
    [SerializeField, Range(1, 10)] int _baseMove;
    public int baseMove { get { return _baseMove; } }
    [SerializeField, Range(1, 10)] int _baseJump;
    public int baseJump { get { return _baseJump; } }
    [SerializeField, Range(1, 100)] int _baseEvasion;
    public int baseEvasion { get { return _baseEvasion; } }

    [SerializeField]
    List<JobSkill> _skills = new List<JobSkill>();
    public List<JobSkill> skills
    {
        get
        {
            return _skills;
        }
        private set
        {
            _skills = value;
        }
    }

    [SerializeField] DivinationData _divinationSkill;
    public DivinationData divinationSkill {
        get { return _divinationSkill; }
    }

    [SerializeField]
    List<UnitJob> jobRequirements = new List<UnitJob>();

    public bool IsQualifiedFor(UnitData unit)
    {
        // has no requirements
        if(jobRequirements.Count == 0)
        {
            return true;
        }

        int numRequirementsRemaining = jobRequirements.Count;
        foreach(UnitJob requirementJob in jobRequirements)
        {
            foreach(UnitJob unitJob in unit.availableJobs)
            {
                if(unitJob.jobData == requirementJob.jobData && unitJob.level >= requirementJob.level)
                {
                    numRequirementsRemaining--;
                }
            }
        }

        if(numRequirementsRemaining <= 0)
        {
            return true;
        }
        
        return false;
    }

    public void Load(UnitData unitData)
    {
        unitData.stats.BindEventToStat(Stat.MAX_HP, OnCalculateStat);
        unitData.stats.BindEventToStat(Stat.MAX_MP, OnCalculateStat);
        unitData.stats.BindEventToStat(Stat.BODY, OnCalculateStat);
        unitData.stats.BindEventToStat(Stat.MIND, OnCalculateStat);
        unitData.stats.BindEventToStat(Stat.SPIRIT, OnCalculateStat);
        unitData.stats.BindEventToStat(Stat.SPEED, OnCalculateStat);
        unitData.stats.BindEventToStat(Stat.MOVE, OnCalculateStat);
        unitData.stats.BindEventToStat(Stat.JUMP, OnCalculateStat);
        unitData.stats.BindEventToStat(Stat.EVASION, OnCalculateStat);
    }

    public void OnCalculateStat(ModInt modInt, Stat stat)
    {
        switch(stat)
        {
            case Stat.MAX_HP:
                modInt.baseMultiplier = hpMult;
            break;
            case Stat.MAX_MP:
                modInt.baseMultiplier = mpMult;
            break;
            case Stat.BODY:
                modInt.baseMultiplier = bodyMult;
            break;
            case Stat.MIND:
                modInt.baseMultiplier = mindMult;
            break;
            case Stat.SPIRIT:
                modInt.baseMultiplier = spiritMult;
            break;
            case Stat.SPEED:
                modInt.baseMultiplier = speedMult;
            break;
            case Stat.MOVE:
                modInt.baseAdd = baseMove;
            break;
            case Stat.JUMP:
                modInt.baseAdd = baseJump;
            break;
            case Stat.EVASION:
                modInt.baseAdd = baseEvasion;
            break;
        }
    }
}