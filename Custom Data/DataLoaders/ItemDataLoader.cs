using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataLoader
{
    Dictionary<string, ItemData> dataDictionary = new Dictionary<string, ItemData>();
    Dictionary<string, ItemSaveData> preloadDataDictionary = new Dictionary<string, ItemSaveData>();

    List<ItemData> defaultData = new List<ItemData>();
    string folderName;
    List<InstalledMod> installedMods = new List<InstalledMod>();

    public ItemDataLoader(List<ItemData> defaultData, 
                      string folderName, 
                      List<InstalledMod> installedMods)
    {
        this.defaultData = defaultData;
        this.folderName = folderName;
        this.installedMods = installedMods;
    }

    public void Preload(List<ItemData> typePrefabs)
    {
        // add the default data
        foreach(ItemData data in defaultData)
        {
            dataDictionary[data.id] = data;
            Debug.Log(data.id);
        }

        foreach(InstalledMod mod in installedMods)
        {
            string folder = SaveDataLoader.Instance.MOD_FOLDER + "/" + mod.name + "/" + folderName + "/";
            if(Directory.Exists(folder))
            {
                string[] files = Directory.GetFiles(folder);
                foreach(string filepath in files)
                {
                    string key = Path.GetFileNameWithoutExtension(filepath);
                    // by having the installed mods be in priority order
                    // the first one to assign custom data to a skill takes priority
                    // so we never have to add more to it
                    if(!preloadDataDictionary.ContainsKey(key))
                    {
                        if(Path.GetExtension(filepath).ToLower() == ".json")
                        {
                            string json = File.ReadAllText(filepath);
                            ItemSaveData saveData = JsonUtility.FromJson<ItemSaveData>(json);

                            switch(saveData.loadType)
                            {
                                case "Equipment":
                                    saveData = JsonUtility.FromJson<EquipmentSaveData>(json);
                                break;
                                case "Weapon":
                                    saveData = JsonUtility.FromJson<WeaponSaveData>(json);
                                break;
                            }

                            // this is new custom data, make a new container for it
                            if(!dataDictionary.ContainsKey(key))
                            {
                                foreach(ItemData typePrefab in typePrefabs)
                                {
                                    if(typePrefab.loadType == saveData.LoadType)
                                    {
                                        ItemData data = UnityEngine.Object.Instantiate(typePrefab);
                                        dataDictionary[saveData.ID] = data;
                                        Debug.Log("Custom element: "+key+" / "+data.loadType);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                Debug.Log("Overwritten element: "+key);
                            }
                            preloadDataDictionary[saveData.ID] = saveData;
                        }
                    }
                }
            }
        }
    }

    public void Populate()
    {
        foreach(KeyValuePair<string, ItemSaveData> saveDataPair in preloadDataDictionary)
        {
            if(dataDictionary.ContainsKey(saveDataPair.Key))
            {
                dataDictionary[saveDataPair.Key].LoadFromSaveData(saveDataPair.Value);
            }
        }
    }

    public ItemData GetData(string id)
    {
        if(dataDictionary.ContainsKey(id))
        {
            return dataDictionary[id];
        }

        return null;
    }
}