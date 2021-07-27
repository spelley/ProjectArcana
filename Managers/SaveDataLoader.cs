using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveDataLoader : MonoBehaviour
{
    static SaveDataLoader _instance;
    public static SaveDataLoader Instance 
    { 
        get 
        { 
            return _instance; 
        }
    }

    void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } 
        else
        {
            _instance = this;
            DEFAULT_DATA_FOLDER = Application.persistentDataPath + "/Default Data/";
            DEFAULT_JOBS_FOLDER = DEFAULT_DATA_FOLDER + "/Classes/";
            DEFAULT_SKILLS_FOLDER = DEFAULT_DATA_FOLDER + "/Skills/";
            DEFAULT_ELEMENTS_FOLDER = DEFAULT_DATA_FOLDER + "/Elements/";
            DEFAULT_FORMATIONS_FOLDER = DEFAULT_DATA_FOLDER + "/Formations/";
            DEFAULT_ARCANA_FOLDER = DEFAULT_DATA_FOLDER + "/Arcana/";

            MOD_FOLDER = Application.persistentDataPath + "/Mods/";

            List<string> folders = new List<string>();
            folders.Add(DEFAULT_DATA_FOLDER);
            folders.Add(DEFAULT_JOBS_FOLDER);
            folders.Add(DEFAULT_ELEMENTS_FOLDER);
            folders.Add(DEFAULT_SKILLS_FOLDER);
            folders.Add(DEFAULT_FORMATIONS_FOLDER);
            folders.Add(DEFAULT_ARCANA_FOLDER);

            DontDestroyOnLoad(this.gameObject);

            foreach(string folder in folders)
            {
                if(!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
            LoadInitialData();
        }
    }

    public string DEFAULT_DATA_FOLDER { get; private set; }
    public string DEFAULT_JOBS_FOLDER { get; private set; }
    public string DEFAULT_SKILLS_FOLDER { get; private set; }
    public string DEFAULT_FORMATIONS_FOLDER { get; private set; }
    public string DEFAULT_ELEMENTS_FOLDER { get; private set; }
    public string DEFAULT_ARCANA_FOLDER { get; private set; }
    public string MOD_FOLDER { get; private set; }

    [Header("TEST DATA -- Mods")]
    [SerializeField] List<InstalledMod> installedMods = new List<InstalledMod>();

    [Header("Default Data")]
    [SerializeField] List<JobData> defaultJobs = new List<JobData>();
    [SerializeField] List<SkillData> defaultSkills = new List<SkillData>();
    [SerializeField] List<FormationData> defaultFormations = new List<FormationData>();
    [SerializeField] List<ElementData> defaultElements = new List<ElementData>();
    [SerializeField] List<ArcanaData> defaultArcana = new List<ArcanaData>();

    [Header("Prefab Data")]
    [SerializeField] List<ElementData> elementPrefabs;
    [SerializeField] List<FormationData> formationPrefabs;
    [SerializeField] List<ArcanaData> arcanaPrefabs;
    [SerializeField] List<JobData> jobPrefabs;

    [Header("Static Data")]
    [SerializeField] List<SkillCalculation> skillCalculations = new List<SkillCalculation>();
    Dictionary<string, SkillCalculation> calculationDictionary = new Dictionary<string, SkillCalculation>();

    DataLoader<ElementData, ElementSaveData> elementDataLoader;
    DataLoader<FormationData, FormationSaveData> formationDataLoader;
    DataLoader<ArcanaData, ArcanaSaveData> arcanaDataLoader;
    DataLoader<JobData, JobSaveData> jobDataLoader;

    void LoadInitialData()
    {
        elementDataLoader = new DataLoader<ElementData, ElementSaveData>(defaultElements, "Elements", installedMods);
        formationDataLoader = new DataLoader<FormationData, FormationSaveData>(defaultFormations, "Formations", installedMods);
        arcanaDataLoader = new DataLoader<ArcanaData, ArcanaSaveData>(defaultArcana, "Arcana", installedMods);
        jobDataLoader = new DataLoader<JobData, JobSaveData>(defaultJobs, "Classes", installedMods);

        elementDataLoader.Preload(elementPrefabs);
        arcanaDataLoader.Preload(arcanaPrefabs);
        formationDataLoader.Preload(formationPrefabs);
        jobDataLoader.Preload(jobPrefabs);

        calculationDictionary.Clear();
        foreach(SkillCalculation skillCalculation in skillCalculations)
        {
            calculationDictionary.Add(skillCalculation.id, skillCalculation);
        }

        elementDataLoader.Populate();
        arcanaDataLoader.Populate();
        formationDataLoader.Populate();
        jobDataLoader.Populate();
    }

    public JobData GetJobData(string id)
    {
        return jobDataLoader.GetData(id);
    }

    public FormationData GetFormationData(string id)
    {
        return formationDataLoader.GetData(id);
    }

    public ElementData GetElementData(string id)
    {
        return elementDataLoader.GetData(id);
    }

    public SkillCalculation GetSkillCalculation(string id)
    {
        if(calculationDictionary.ContainsKey(id))
        {
            return calculationDictionary[id];
        }
        return null;
    }

    public void SaveDefaultData()
    {
        foreach(JobData defaultJob in defaultJobs)
        {
            JobSaveData saveData = defaultJob.GetSaveData();
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(DEFAULT_JOBS_FOLDER + saveData.id + ".json", json);
        }

        foreach(SkillData defaultSkill in defaultSkills)
        {
            // SkillSaveData saveData = defaultSkill.GetSaveData();
            // string json = JsonUtility.ToJson(saveData);
            // File.WriteAllText(DEFAULT_SKILLS_FOLDER + saveData.id + ".json", json);
        }

        foreach(FormationData defaultFormation in defaultFormations)
        {
            FormationSaveData saveData = defaultFormation.GetSaveData();
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(DEFAULT_FORMATIONS_FOLDER + saveData.id + ".json", json);
        }

        foreach(ElementData defaultElement in defaultElements)
        {
            ElementSaveData saveData = defaultElement.GetSaveData();
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(DEFAULT_ELEMENTS_FOLDER + saveData.id + ".json", json);
        }

        foreach(ArcanaData defArcana in defaultArcana)
        {
            ArcanaSaveData saveData = defArcana.GetSaveData();
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(DEFAULT_ARCANA_FOLDER + saveData.id + ".json", json);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            SaveDefaultData();
        }
    }
}