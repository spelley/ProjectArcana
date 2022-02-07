using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JobData", menuName = "Custom Data/Job Data")]
public class JobData: ScriptableObject, ILoadable<JobSaveData>
{
    [SerializeField] string _id;
    public string id { get { return _id; } }

    string _loadType = "Class";
    public string loadType { get { return _loadType; } }

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

    [SerializeField]
    List<JobPassive> _passives = new List<JobPassive>();
    public List<JobPassive> passives
    {
        get
        {
            return _passives;
        }
        private set
        {
            _passives = value;
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

    public JobSaveData GetSaveData()
    {
        JobSaveData saveData = new JobSaveData();
        saveData.id = id;
        saveData.loadType = "Job";
        saveData.name = jobName;
        saveData.description = description;
        saveData.iconPath = (icon != null) ? icon.name : "";
        saveData.hpMultiplier = Mathf.RoundToInt(hpMult * 100);
        saveData.mpMultiplier = Mathf.RoundToInt(mpMult * 100);
        saveData.bodyMultiplier = Mathf.RoundToInt(bodyMult * 100);
        saveData.mindMultiplier = Mathf.RoundToInt(mindMult * 100);
        saveData.spiritMultiplier = Mathf.RoundToInt(spiritMult * 100);
        saveData.speedMultiplier = Mathf.RoundToInt(speedMult * 100);
        saveData.baseMove = baseMove;
        saveData.baseJump = baseJump;
        saveData.baseEvasion = baseEvasion;

        saveData.jobSkillIDs = new List<JobSkillSaveData>();
        foreach(JobSkill jobSkill in skills)
        {
            saveData.jobSkillIDs.Add(jobSkill.GetSaveData());
        }

        saveData.jobPassiveIDs = new List<JobSkillSaveData>();
        foreach(JobPassive jobPassive in passives)
        {
            saveData.jobPassiveIDs.Add(jobPassive.GetSaveData());
        }

        saveData.jobRequirements = new List<UnitJobSaveData>();
        foreach(UnitJob unitJob in jobRequirements)
        {
            saveData.jobRequirements.Add(unitJob.GetSaveData());
        }

        Debug.Log(JsonUtility.ToJson(saveData));

        return saveData;
    }

    public bool LoadFromSaveData(JobSaveData saveData)
    {
        _id = saveData.id;
        _loadType = saveData.loadType;
        _jobName = saveData.name;
        _description = saveData.description;
        _icon = Resources.Load<Sprite>("Arcana/Icons/"+saveData.iconPath);
        _hpMult = saveData.hpMultiplier * .01f;
        _mpMult = saveData.mpMultiplier * .01f;
        _bodyMult = saveData.bodyMultiplier * .01f;
        _mindMult = saveData.mindMultiplier * .01f;
        _spiritMult = saveData.spiritMultiplier * .01f;
        _speedMult = saveData.speedMultiplier * .01f;
        _baseMove = saveData.baseMove;
        _baseJump = saveData.baseJump;
        _baseEvasion = saveData.baseEvasion;

        _skills.Clear();
        foreach(JobSkillSaveData jobSkillSaveData in saveData.jobSkillIDs)
        {
            SkillData skillData = SaveDataLoader.Instance.GetSkillData(jobSkillSaveData.skillID);
            if(skillData != null)
            {
                JobSkill jobSkill = new JobSkill();
                jobSkill.jobLocked = jobSkillSaveData.jobLocked;
                jobSkill.learnLevel = jobSkillSaveData.learnLevel;
                jobSkill.skill = skillData;

                _skills.Add(jobSkill);
            }
        }

        _passives.Clear();
        foreach(JobSkillSaveData jobSkillSaveData in saveData.jobPassiveIDs)
        {
            PassiveData skillData = SaveDataLoader.Instance.GetPassiveData(jobSkillSaveData.skillID);
            if(skillData != null)
            {
                JobPassive jobPassive = new JobPassive();
                jobPassive.jobLocked = jobSkillSaveData.jobLocked;
                jobPassive.learnLevel = jobSkillSaveData.learnLevel;
                jobPassive.skill = skillData;

                _passives.Add(jobPassive);
            }
        }

        jobRequirements.Clear();
        foreach(UnitJobSaveData unitJobSaveData in saveData.jobRequirements)
        {
            JobData jobRequirementData = SaveDataLoader.Instance.GetJobData(unitJobSaveData.id);
            if(jobRequirementData == null)
            {
                continue;
            }

            UnitJob unitJob = new UnitJob();
            unitJob.level = unitJobSaveData.level;
            unitJob.experience = unitJobSaveData.experience;
            unitJob.jobData = jobRequirementData;
            jobRequirements.Add(unitJob);
        }

        return true;
    }
}