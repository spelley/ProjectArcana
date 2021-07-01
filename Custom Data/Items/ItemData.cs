using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData: ScriptableObject
{
    [SerializeField]
    [Header("Unique ID")]
    int _itemID;
    public int itemID { get { return _itemID; } }

    [Header("Basic Information")]
    [SerializeField]
    string _itemName;
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
}