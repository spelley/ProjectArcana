using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticDataLoader<T> where T : UnityEngine.Object, IStaticData
{
    Dictionary<string, T> dataDictionary = new Dictionary<string, T>();

    List<T> defaultData = new List<T>();

    public StaticDataLoader(List<T> defaultData)
    {
        this.defaultData = defaultData;
        LoadDictionary();
    }

    void LoadDictionary()
    {
        foreach(T data in defaultData)
        {
            dataDictionary.Add(data.id, data);
        }
    }

    public T GetData(string id)
    {
        if(dataDictionary.ContainsKey(id))
        {
            return dataDictionary[id];
        }
        return null;
    }
}