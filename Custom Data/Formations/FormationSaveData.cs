using System;

[Serializable]
public class FormationSaveData : ISaveData
{
    public string id;
    public string ID { get { return id; } }
    public string loadType;
    public string LoadType { get { return loadType; } }
    public string name;
    public string description;
    public string imagePath;
    public SimpleVector3Int[] offsets;
}