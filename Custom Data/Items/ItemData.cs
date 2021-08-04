using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData: ScriptableObject, ILoadable<ItemSaveData>
{
    [SerializeField] protected string _id;
    public string id { get { return _id; } }

    protected string _loadType = "Item";
    public virtual string loadType { get { return _loadType; } }

    [Header("Basic Information")]
    [SerializeField]
    protected string _itemName;
    public string itemName
    {
        get
        {
            return _itemName;
        }
        private set
        {
            _itemName = value;
        }
    }

    [SerializeField]
    protected string _description;
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

    public virtual ItemSaveData GetSaveData()
    {
        ItemSaveData saveData = new ItemSaveData();
        saveData.id = _id;
        saveData.loadType = _loadType;
        saveData.name = _itemName;
        saveData.description = _description;

        return saveData;
    }

    public virtual bool LoadFromSaveData(ItemSaveData saveData)
    {
        _id = saveData.id;
        _loadType = saveData.loadType;
        _description = saveData.description;
        _itemName = saveData.name;

        return true;
    }
}