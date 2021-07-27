[System.Serializable]
public class ArcanaSaveData : ISaveData
{
    public string id;
    public string ID { get { return id; } }
    public string loadType;
    public string LoadType { get { return loadType; } }
    public string arcanaName;
    public string iconPath;
    public string cardImagePath;
    public string[] elementIDs;
}