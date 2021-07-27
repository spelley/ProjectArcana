using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class JobSaveData : ISaveData
{
    public string id;
    public string ID { get { return id; } }
    public string loadType;
    public string LoadType { get { return loadType; } }
    public string name;
    public string description;
    public string iconPath;
    public int hpMultiplier;
    public int mpMultiplier;
    public int bodyMultiplier;
    public int mindMultiplier;
    public int spiritMultiplier;
    public int speedMultiplier;
    public int baseMove;
    public int baseJump;
    public int baseEvasion;
    public string divinationSkillID;
    public List<JobSkillSaveData> jobSkillIDs;
    public List<UnitJobSaveData> jobRequirements;
}