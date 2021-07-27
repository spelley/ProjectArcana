using System;

[Serializable]
public struct JobSkill
{
    public bool jobLocked;
    public int learnLevel;
    public SkillData skill;

    public JobSkillSaveData GetSaveData()
    {
        JobSkillSaveData saveData = new JobSkillSaveData();
        saveData.jobLocked = jobLocked;
        saveData.learnLevel = learnLevel;
        saveData.skillID = skill.id;

        return saveData;
    }
}