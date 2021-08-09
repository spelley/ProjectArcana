using System;

[Serializable]
public struct JobPassive
{
    public bool jobLocked;
    public int learnLevel;
    public PassiveData skill;

    public JobSkillSaveData GetSaveData()
    {
        JobSkillSaveData saveData = new JobSkillSaveData();
        saveData.jobLocked = jobLocked;
        saveData.learnLevel = learnLevel;
        saveData.skillID = skill.id;

        return saveData;
    }
}