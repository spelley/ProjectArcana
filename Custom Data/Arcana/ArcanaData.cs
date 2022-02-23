using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArcanaData", menuName = "Custom Data/Arcana Data", order = 1)]
public class ArcanaData : ScriptableObject, ILoadable<ArcanaSaveData>
{
    [SerializeField] string _id;
    public string id { get { return _id; } }
    string _loadType = "Arcana";
    public string loadType { get { return _loadType; } }
    [SerializeField] string _arcanaName;
    public string arcanaName { get { return _arcanaName; } }
    [SerializeField] Sprite _icon;
    public Sprite icon { get { return _icon; } }
    [SerializeField] Sprite _cardImage;
    public Sprite cardImage { get { return _cardImage; } }
    [SerializeField] List<ElementData> _elements;
    public List<ElementData> elements { get { return _elements; } }

    public ArcanaSaveData GetSaveData()
    {
        ArcanaSaveData saveData = new ArcanaSaveData();
        saveData.id = id;
        saveData.loadType = loadType;
        saveData.arcanaName = arcanaName;
        saveData.iconPath = (icon != null) ? icon.name : "";
        saveData.cardImagePath = (cardImage != null) ? cardImage.name : "";

        string[] elementIDs = new string[elements.Count];
        for(int i = 0; i < elements.Count; i++)
        {
            // Debug.Log(JsonUtility.ToJson(elements[i].GetSaveData()));
            elementIDs[i] = elements[i].id;
        }
        saveData.elementIDs = elementIDs;

        // Debug.Log(JsonUtility.ToJson(saveData));

        return saveData;
    }

    public bool LoadFromSaveData(ArcanaSaveData saveData)
    {
        _id = saveData.ID;
        _arcanaName = saveData.arcanaName;
        _icon = Resources.Load<Sprite>("Arcana/Icons/" + saveData.iconPath);
        _cardImage = Resources.Load<Sprite>("Arcana/Icons/" + saveData.cardImagePath);
        _elements.Clear();
        foreach(string elementID in saveData.elementIDs)
        {
            ElementData element = SaveDataLoader.Instance.GetElementData(elementID);
            if(element != null)
            {
                _elements.Add(element);
            }
        }
        
        // Debug.Log("Loaded: "+_arcanaName);
        return true;
    }
}
