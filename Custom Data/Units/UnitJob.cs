using System;
using UnityEngine;

[Serializable]
public struct UnitJob
{
    public int level;
    public int experience;
    public JobData jobData;

    public bool AddExperience(int amount, UnitData unitData)
    {
        if(level <= 11)
        {
            if((experience + amount) >= 100)
            {
                experience = 0;
                level += 1;
                unitData.GainedJobExperience(this, amount);
                unitData.GainedJobLevel(this, 1);
                return true;
            }
            experience += amount;
            unitData.GainedJobExperience(this, amount);
        }
        return false;
    }

    public UnitJobSaveData GetSaveData()
    {
        UnitJobSaveData saveData = new UnitJobSaveData();
        saveData.level = level;
        saveData.experience = experience;
        saveData.id = jobData.id;

        return saveData;
    }

    public bool LoadFromSaveData(UnitJobSaveData saveData)
    {
        level = saveData.level;
        experience = saveData.experience;
        jobData = SaveDataLoader.Instance.GetJobData(saveData.id);
        
        return true;
    }
}