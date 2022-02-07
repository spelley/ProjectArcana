using System;

[Serializable]
public struct GameSaveData
{
    public UnitSaveData[] partyUnits;
    public int activePlayerIndex;
    public InventorySaveData inventorySaveData;
    public SimpleVector3 curPosition;
    public string sceneID;
}