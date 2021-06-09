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

    [SerializeField]
    private List<JobSkill> _skills = new List<JobSkill>();
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
    private List<UnitJob> jobRequirements = new List<UnitJob>();

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
}