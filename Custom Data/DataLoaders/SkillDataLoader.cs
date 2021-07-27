using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDataLoader
{
    Dictionary<string, SkillData> dataDictionary = new Dictionary<string, SkillData>();
    Dictionary<string, SkillSaveData> preloadDataDictionary = new Dictionary<string, SkillSaveData>();

    List<SkillData> defaultData = new List<SkillData>();
    string folderName;
    List<InstalledMod> installedMods = new List<InstalledMod>();

    public SkillDataLoader(List<SkillData> defaultData, string folderName, List<InstalledMod> installedMods)
    {
        this.defaultData = defaultData;
        this.folderName = folderName;
        this.installedMods = installedMods;
    }

    public void Preload()
    {
        // add the default data
        foreach(SkillData data in defaultData)
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
                            SkillSaveData saveData = JsonUtility.FromJson<SkillSaveData>(json);
                            /**
                            // this is new custom data, make a new container for it
                            if(!dataDictionary.ContainsKey(key))
                            {
                                SkillData data = UnityEngine.Object.Instantiate(prefab);
                                dataDictionary[saveData.ID] = data;
                                Debug.Log("Custom element: "+key);
                            }
                            else
                            {
                                Debug.Log("Overwritten element: "+key);
                            }
                            preloadDataDictionary[saveData.ID] = saveData;
                            **/
                        }
                    }
                }
            }
        }
    }

    public void Populate()
    {
        foreach(KeyValuePair<string, SkillSaveData> saveDataPair in preloadDataDictionary)
        {
            if(dataDictionary.ContainsKey(saveDataPair.Key))
            {
                dataDictionary[saveDataPair.Key].LoadFromSaveData(saveDataPair.Value);
            }
        }
    }

    public SkillData GetData(string id)
    {
        if(dataDictionary.ContainsKey(id))
        {
            return dataDictionary[id];
        }

        return null;
    }
}