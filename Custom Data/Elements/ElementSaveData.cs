using System;

[Serializable]
public class ElementSaveData : ISaveData
{
    public string id;
    public string ID { get { return id; } }
    public string loadType;
    public string LoadType { get { return loadType; } }
    public string name;
    public string description;
    public string iconPath;
    public string color;
    public string[] strengthsIDs;
    public string[] weaknessesIDs;
}