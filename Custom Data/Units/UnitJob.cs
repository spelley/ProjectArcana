using System;

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
                unitData.GainedJobLevel(this, amount);
                return true;
            }
            experience += amount;
            unitData.GainedJobExperience(this, amount);
        }
        return false;
    }
}