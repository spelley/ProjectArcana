using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Formation Data", menuName = "Custom Data/Formation", order = 1)]
public class FormationData : ScriptableObject, ILoadable<FormationSaveData>
{
    [SerializeField] string _id;
    public string id { get { return _id; } }
    string _loadType = "Formation";
    public string loadType { get { return _loadType; } }

    [SerializeField]
    string _formationName;
    public string formationName
    {
        get
        {
            return _formationName;
        }
        private set
        {
            _formationName = value;
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
    Sprite _image;
    public Sprite image
    {
        get
        {
            return _image;
        }
        private set
        {
            _image = value;
        }
    }
    
    [SerializeField]
    Vector3Int[] _offsets;
    public Vector3Int[] offsets
    {
        get
        {
            return _offsets;
        }
        private set
        {
            _offsets = value;
        }
    }

    public FormationSaveData GetSaveData()
    {
        FormationSaveData saveData = new FormationSaveData();
        saveData.id = id;
        saveData.loadType = loadType;
        saveData.name = formationName;
        saveData.description = description;
        saveData.imagePath = (image != null) ? image.name : "";
        saveData.offsets = new SimpleVector3Int[offsets.Length];

        for(int i = 0; i < offsets.Length; i++)
        {
            saveData.offsets[i] = new SimpleVector3Int(offsets[i].x, offsets[i].y, offsets[i].z);
        }
        // Debug.LogJsonUtility.ToJson(saveData));
        return saveData;
    }

    public bool LoadFromSaveData(FormationSaveData saveData)
    {
        _id = saveData.id;
        _loadType = saveData.loadType;
        _formationName = saveData.name;
        _description = saveData.description;
        _image = Resources.Load<Sprite>("Formations/Icons/"+saveData.imagePath);
        // Debug.LogsaveData.offsets);
        return true;
    }
}