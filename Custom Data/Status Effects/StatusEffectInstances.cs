using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectInstances : ISerializationCallbackReceiver
{
    [NonSerialized]
    public List<StatusEffectSaveData> items = new List<StatusEffectSaveData>();

    public List<string> _serializedItems;
    public List<string> _itemType;

    public void OnBeforeSerialize()
    {
        _serializedItems = new List<string>();
        _itemType = new List<string>();
        for(int i = 0; i < items.Count; i++)
        {
            _serializedItems.Add(JsonUtility.ToJson(items[i]));
            _itemType.Add(items[i].GetType().ToString());
        }   
    }

    public void OnAfterDeserialize()
    {
        for(int i = 0; i < _serializedItems.Count; i++)
        {
            Type type = Type.GetType(_itemType[i]);
            items.Add((StatusEffectSaveData)JsonUtility.FromJson(_serializedItems[i], type));
        }
    }
}