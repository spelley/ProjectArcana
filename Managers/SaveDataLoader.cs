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
            DEFAULT_DIVINATION_FOLDER = DEFAULT_DATA_FOLDER + "/Divinations/";
            DEFAULT_ITEMS_FOLDER = DEFAULT_DATA_FOLDER + "/Items/";
            DEFAULT_UNITS_FOLDER = DEFAULT_DATA_FOLDER + "/Units/";

            MOD_FOLDER = Application.persistentDataPath + "/Mods/";

            List<string> folders = new List<string>();
            folders.Add(DEFAULT_DATA_FOLDER);
            folders.Add(DEFAULT_JOBS_FOLDER);
            folders.Add(DEFAULT_ELEMENTS_FOLDER);
            folders.Add(DEFAULT_SKILLS_FOLDER);
            folders.Add(DEFAULT_FORMATIONS_FOLDER);
            folders.Add(DEFAULT_ARCANA_FOLDER);
            folders.Add(DEFAULT_DIVINATION_FOLDER);
            folders.Add(DEFAULT_ITEMS_FOLDER);
            folders.Add(DEFAULT_UNITS_FOLDER);

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
    public string DEFAULT_DIVINATION_FOLDER { get; private set; }
    public string DEFAULT_ITEMS_FOLDER { get; private set; }
    public string DEFAULT_UNITS_FOLDER { get; private set; }
    public string MOD_FOLDER { get; private set; }

    [Header("TEST DATA -- Mods")]
    [SerializeField] List<InstalledMod> installedMods = new List<InstalledMod>();

    [Header("Default Data")]
    [SerializeField] List<JobData> defaultJobs = new List<JobData>();
    [SerializeField] List<SkillData> defaultSkills = new List<SkillData>();
    [SerializeField] List<FormationData> defaultFormations = new List<FormationData>();
    [SerializeField] List<ElementData> defaultElements = new List<ElementData>();
    [SerializeField] List<ArcanaData> defaultArcana = new List<ArcanaData>();
    [SerializeField] List<DivinationData> defaultDivinations = new List<DivinationData>();
    [SerializeField] List<UnitData> defaultUnits = new List<UnitData>();
    [SerializeField] List<ItemData> defaultItems = new List<ItemData>();

    [Header("Prefab Data")]
    [SerializeField] List<ElementData> elementPrefabs;
    [SerializeField] List<FormationData> formationPrefabs;
    [SerializeField] List<ArcanaData> arcanaPrefabs;
    [SerializeField] List<JobData> jobPrefabs;
    [SerializeField] List<SkillData> skillPrefabs;
    [SerializeField] List<DivinationData> divinationPrefabs;
    [SerializeField] List<UnitData> unitPrefabs;
    [SerializeField] List<ItemData> itemPrefabs;

    [Header("Static Data")]
    [SerializeField] List<SkillCalculation> skillCalculations = new List<SkillCalculation>();
    Dictionary<string, SkillCalculation> calculationDictionary = new Dictionary<string, SkillCalculation>();
    [SerializeField] List<SkillEffect> skillEffects = new List<SkillEffect>();
    Dictionary<string, SkillEffect> skillEffectDictionary = new Dictionary<string, SkillEffect>();
    [SerializeField] List<GameObject> executeAnimations;
    Dictionary<string, GameObject> executeAnimationDictionary = new Dictionary<string, GameObject>();
    [SerializeField] List<StatusEffect> statusEffects;
    Dictionary<string, StatusEffect> statusEffectDictionary = new Dictionary<string, StatusEffect>();
    [SerializeField] List<GameObject> unitModels;
    Dictionary<string, GameObject> unitModelDictionary = new Dictionary<string, GameObject>();
    [SerializeField] List<AIBrain> aiBrains;
    Dictionary<string, AIBrain> aiBrainDictionary = new Dictionary<string, AIBrain>();

    DataLoader<ElementData, ElementSaveData> elementDataLoader;
    DataLoader<FormationData, FormationSaveData> formationDataLoader;
    DataLoader<ArcanaData, ArcanaSaveData> arcanaDataLoader;
    DataLoader<JobData, JobSaveData> jobDataLoader;
    DataLoader<SkillData, SkillSaveData> skillDataLoader;
    DataLoader<DivinationData, DivinationSaveData> divinationDataLoader;
    DataLoader<UnitData, UnitSaveData> unitDataLoader;
    ItemDataLoader itemDataLoader;

    void LoadInitialData()
    {
        elementDataLoader = new DataLoader<ElementData, ElementSaveData>(defaultElements, "Elements", installedMods);
        formationDataLoader = new DataLoader<FormationData, FormationSaveData>(defaultFormations, "Formations", installedMods);
        arcanaDataLoader = new DataLoader<ArcanaData, ArcanaSaveData>(defaultArcana, "Arcana", installedMods);
        jobDataLoader = new DataLoader<JobData, JobSaveData>(defaultJobs, "Classes", installedMods);
        skillDataLoader = new DataLoader<SkillData, SkillSaveData>(defaultSkills, "Skills", installedMods);
        divinationDataLoader = new DataLoader<DivinationData, DivinationSaveData>(defaultDivinations, "Divinations", installedMods);
        itemDataLoader = new ItemDataLoader(defaultItems, "Items", installedMods);
        unitDataLoader = new DataLoader<UnitData, UnitSaveData>(defaultUnits, "Units", installedMods);

        elementDataLoader.Preload(elementPrefabs);
        arcanaDataLoader.Preload(arcanaPrefabs);
        formationDataLoader.Preload(formationPrefabs);
        jobDataLoader.Preload(jobPrefabs);
        skillDataLoader.Preload(skillPrefabs);
        divinationDataLoader.Preload(divinationPrefabs);
        itemDataLoader.Preload(itemPrefabs);
        unitDataLoader.Preload(unitPrefabs);

        calculationDictionary.Clear();
        foreach(SkillCalculation skillCalculation in skillCalculations)
        {
            calculationDictionary.Add(skillCalculation.id, skillCalculation);
        }

        skillEffectDictionary.Clear();
        foreach(SkillEffect skillEffect in skillEffects)
        {
            skillEffectDictionary.Add(skillEffect.id, skillEffect);
        }

        executeAnimationDictionary.Clear();
        foreach(GameObject executeAnimation in executeAnimations)
        {
            executeAnimationDictionary.Add(executeAnimation.GetComponent<SkillAnimation>().id, executeAnimation);
        }

        statusEffectDictionary.Clear();
        foreach(StatusEffect statusEffect in statusEffects)
        {
            statusEffectDictionary.Add(statusEffect.id, statusEffect);
        }

        unitModelDictionary.Clear();
        foreach(GameObject unitModel in unitModels)
        {
            unitModelDictionary.Add(unitModel.GetComponent<UnitModel>().id, unitModel);
        }

        aiBrainDictionary.Clear();
        foreach(AIBrain aiBrain in aiBrains)
        {
            aiBrainDictionary.Add(aiBrain.id, aiBrain);
        }

        elementDataLoader.Populate();
        arcanaDataLoader.Populate();
        formationDataLoader.Populate();
        jobDataLoader.Populate();
        skillDataLoader.Populate();
        divinationDataLoader.Populate();
        itemDataLoader.Populate();
        unitDataLoader.Populate();
    }

    public JobData GetJobData(string id)
    {
        return jobDataLoader.GetData(id);
    }

    public FormationData GetFormationData(string id)
    {
        return formationDataLoader.GetData(id);
    }

    public ArcanaData GetArcanaData(string id)
    {
        return arcanaDataLoader.GetData(id);
    }

    public ElementData GetElementData(string id)
    {
        return elementDataLoader.GetData(id);
    }

    public DivinationData GetDivinationData(string id)
    {
        return divinationDataLoader.GetData(id);
    }

    public SkillCalculation GetSkillCalculation(string id)
    {
        if(calculationDictionary.ContainsKey(id))
        {
            return calculationDictionary[id];
        }
        return null;
    }

    public SkillEffect GetSkillEffect(string id)
    {
        if(skillEffectDictionary.ContainsKey(id))
        {
            return skillEffectDictionary[id];
        }
        return null;
    }

    public SkillData GetSkillData(string id)
    {
        return skillDataLoader.GetData(id);
    }

    public GameObject GetUnitModel(string id)
    {
        if(unitModelDictionary.ContainsKey(id))
        {
            return unitModelDictionary[id];
        }
        return null;
    }

    public GameObject GetExecuteAnimation(string id)
    {
        if(executeAnimationDictionary.ContainsKey(id))
        {
            return executeAnimationDictionary[id];
        }
        return null;
    }

    public AIBrain GetAIBrain(string id)
    {
        if(aiBrainDictionary.ContainsKey(id))
        {
            return aiBrainDictionary[id];
        }
        return null;
    }

    public StatusEffect GetStatusEffect(string id)
    {
        if(statusEffectDictionary.ContainsKey(id))
        {
            return statusEffectDictionary[id];
        }
        return null;
    }

    public ItemData GetItemData(string id)
    {
        return itemDataLoader.GetData(id);
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
            SkillSaveData saveData = defaultSkill.GetSaveData();
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(DEFAULT_SKILLS_FOLDER + saveData.id + ".json", json);
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

        foreach(DivinationData defDivination in defaultDivinations)
        {
            DivinationSaveData saveData = defDivination.GetSaveData();
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(DEFAULT_DIVINATION_FOLDER + saveData.id + ".json", json);

        }

        foreach(ItemData defaultItem in defaultItems)
        {
            ItemSaveData saveData = defaultItem.GetSaveData();
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(DEFAULT_ITEMS_FOLDER + saveData.id + ".json", json);
        }

        foreach(UnitData defaultUnit in defaultUnits)
        {
            UnitSaveData saveData = defaultUnit.GetSaveData();
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(DEFAULT_UNITS_FOLDER + saveData.id + ".json", json);

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