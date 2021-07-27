using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ElementData", menuName = "Custom Data/Element")]
public class ElementData : ScriptableObject, ILoadable<ElementSaveData>
{
    [SerializeField] string _id;
    public string id { get { return _id; } }
    string _loadType = "Element";
    public string loadType { get { return _loadType; } }

    [SerializeField]
    string _elementName;
    public string elementName
    {
        get
        {
            return _elementName;
        }
        private set
        {
            _elementName = value;
        }
    }

    [SerializeField]
    string _description;
    public string description
    {
        get
        {
            return _description;
        }
        private set
        {
            _description = value;
        }
    }

    [SerializeField]
    Sprite _icon;
    public Sprite icon
    {
        get
        {
            return _icon;
        }
        private set
        {
            _icon = value;
        }
    }

    [SerializeField]
    Color _color;
    public Color color
    {
        get
        {
            return _color;
        }
        private set
        {
            _color = value;
        }
    }
    
    [SerializeField]
    List<ElementData> _weaknesses = new List<ElementData>();
    public List<ElementData> weaknesses
    {
        get
        {
            return _weaknesses;
        }
        private set
        {
            _weaknesses = value;
        }
    }

    [SerializeField]
    List<ElementData> _strengths = new List<ElementData>();
    public List<ElementData> strengths
    {
        get
        {
            return _strengths;
        }
        private set
        {
            _strengths = value;
        }
    }

    public ElementSaveData GetSaveData()
    {
        ElementSaveData saveData = new ElementSaveData();
        saveData.id = id;
        saveData.loadType = loadType;
        saveData.name = elementName;
        saveData.description = description;
        saveData.iconPath = (icon != null) ? icon.name : "";
        saveData.color = ColorUtility.ToHtmlStringRGB(color);

        string[] weaknessesIDs = new string[_weaknesses.Count];
        for(int i = 0; i < _weaknesses.Count; i++)
        {
            weaknessesIDs[i] = _weaknesses[i].id;
        }
        saveData.weaknessesIDs = weaknessesIDs;

        string[] strengthsIDs = new string[strengths.Count];
        for(int i = 0; i < strengths.Count; i++)
        {
            strengthsIDs[i] = strengths[i].id;
        }
        saveData.strengthsIDs = strengthsIDs;

        Debug.Log(JsonUtility.ToJson(saveData));

        return saveData;
    }

    public bool LoadFromSaveData(ElementSaveData saveData)
    {
        _id = saveData.id;
        _elementName = saveData.name;
        _description = saveData.description;
        _icon = Resources.Load<Sprite>("Elements/Icons/" + saveData.iconPath);

        _weaknesses.Clear();
        foreach(string weaknessID in saveData.weaknessesIDs)
        {
            ElementData weakness = SaveDataLoader.Instance.GetElementData(weaknessID);
            if(weakness != null)
            {
                _weaknesses.Add(weakness);
            }
        }

        _strengths.Clear();
        foreach(string strengthID in saveData.strengthsIDs)
        {
            ElementData strength = SaveDataLoader.Instance.GetElementData(strengthID);
            if(strength != null)
            {
                _strengths.Add(strength);
            }
        }

        Debug.Log("Loaded: "+_elementName);

        return true;
    }
}
