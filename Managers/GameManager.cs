using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    public static GameManager Instance 
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
            DontDestroyOnLoad(this.gameObject);

            // setup the other managers
            Instantiate(battleManagerPrefab);
            _battleManager = BattleManager.Instance;

            Instantiate(mapManagerPrefab);
            _mapManager = MapManager.Instance;

            playerInventory = new Inventory();
        }
    }

    [SerializeField]
    GameObject battleManagerPrefab;
    BattleManager _battleManager;
    public BattleManager BattleManager
    {
        get
        {
            return _battleManager;
        }
        private set
        {
            _battleManager = value;
        }
    }

    [SerializeField]
    GameObject mapManagerPrefab;
    MapManager _mapManager;
    public MapManager MapManager
    {
        get
        {
            return _mapManager;
        }
        private set
        {
            _mapManager = value;
        }
    }

    public UnitData activePlayer { get; private set; }
    public List<UnitData> party { get; private set; }
    public FormationData curFormation { get; private set; }

    [SerializeField]
    List<UnitData> curPlayers = new List<UnitData>();
    [SerializeField]
    List<Vector3> spawnLocations = new List<Vector3>();

    public Inventory playerInventory { get; private set; }

    [SerializeField]
    FormationData defaultFormation;
    public WeaponData testWeapon;
    public WeaponData testOffHand;
    public EquipmentData testHelmet;
    public EquipmentData testArmor;
    public EquipmentData testAccessory;

    public event Action OnGameManagerLoaded;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        
        party = new List<UnitData>();
        for(int i = 0; i < curPlayers.Count; i++)
        {
            UnitData playerData = Instantiate(curPlayers[i]);
            playerData.Load();
            party.Add(playerData);
        }

        curFormation = defaultFormation;

        GameObject mainPlayer = SpawnUnit(party[0], spawnLocations[0], true);
        activePlayer = party[0];

        // TODO: remove test code
        playerInventory.AddItem(testWeapon, true);
        playerInventory.AddItem(testOffHand, true);
        playerInventory.AddItem(testHelmet, true);
        playerInventory.AddItem(testArmor, true);
        playerInventory.AddItem(testAccessory, true);
        playerInventory.AddItem(testWeapon, true);
        playerInventory.AddItem(testOffHand, true);
        playerInventory.AddItem(testHelmet, true);
        playerInventory.AddItem(testArmor, true);
        playerInventory.AddItem(testAccessory, true);
        playerInventory.AddItem(testWeapon, true);
        playerInventory.AddItem(testOffHand, true);
        playerInventory.AddItem(testHelmet, true);
        playerInventory.AddItem(testArmor, true);
        playerInventory.AddItem(testAccessory, true);
        playerInventory.AddItem(testWeapon, true);
        playerInventory.AddItem(testOffHand, true);
        playerInventory.AddItem(testHelmet, true);
        playerInventory.AddItem(testArmor, true);
        playerInventory.AddItem(testAccessory, true);
        
        playerInventory.EquipFromInventory(testWeapon, activePlayer);
        playerInventory.EquipFromInventory(testOffHand, activePlayer, true);
        playerInventory.EquipFromInventory(testHelmet, activePlayer);
        playerInventory.EquipFromInventory(testArmor, activePlayer);
        playerInventory.EquipFromInventory(testAccessory, activePlayer);

        activePlayer.RefreshUnit();

        StartCoroutine(DelayedInvoke());
    }

    IEnumerator DelayedInvoke()
    {
        yield return null;
        OnGameManagerLoaded?.Invoke();
    }

    public GameObject SpawnUnit(UnitData unitData, Vector3 spawnPosition, bool isPlayerControlled = false)
    {
        unitData.Load();

        GameObject unitGO = Instantiate(unitData.unitModel, spawnPosition, Quaternion.identity);
        unitGO.GetComponent<CharacterMotor>().unitData = unitData;
        if(isPlayerControlled)
        {
            unitGO.tag = "Player";
        }
        unitData.unitGO = unitGO;
        return unitGO;
    }

    public GameSaveData GetSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.sceneID = SceneManager.GetActiveScene().name;
        saveData.partyUnits = new UnitSaveData[party.Count];
        saveData.inventorySaveData = playerInventory.GetSaveData();
        for(int i = 0; i < party.Count; i++)
        {
            saveData.partyUnits[i] = party[i].GetSaveData();
            if(party[i] == activePlayer)
            {
                saveData.activePlayerIndex = i;
            }
        }
        Vector3 currentPosition = activePlayer.unitGO.transform.position;
        saveData.curPosition = new SimpleVector3(currentPosition.x, currentPosition.y, currentPosition.z);

        return saveData;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F5))
        {
            Debug.Log("Quick Save");
            QuickSave();
        }
    }

    public void QuickSave()
    {
        string folder = Application.persistentDataPath + "/Saves/Quick Save/";
        string filename = folder + "GameState.json";
        GameSaveData saveData = GetSaveData();
        if(!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(filename, json);
    }
}
