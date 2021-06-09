using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField]
    FormationData defaultFormation;

    // Start is called before the first frame update
    void Start()
    {
        party = new List<UnitData>();
        for(int i = 0; i < curPlayers.Count; i++)
        {
            UnitData playerData = Instantiate(curPlayers[i]);
            party.Add(playerData);
        }

        curFormation = defaultFormation;

        GameObject mainPlayer = SpawnUnit(party[0], spawnLocations[0]);
        activePlayer = party[0];
        StartCoroutine(SetPlayerFocus(mainPlayer));
    }

    public GameObject SpawnUnit(UnitData unitData, Vector3 spawnPosition)
    {
        unitData.Load();

        GameObject unitGO = Instantiate(unitData.unitModel, spawnPosition, Quaternion.identity);
        unitGO.GetComponent<CharacterMotor>().unitData = unitData;
        unitData.unitGO = unitGO;
        return unitGO;
    }

    IEnumerator SetPlayerFocus(GameObject player)
    {
        yield return new WaitForSeconds(.1f);
        Camera.main.GetComponent<CameraController>().SetFocus(player);
    }

    // TODO: Remove this
    [SerializeField] StatusEffect testStatus;
    void Update()
    {
        // TODO: remove test code
        if(Input.GetKeyDown(KeyCode.P))
        {
            party[0].AddStatus(testStatus);
        }

        if(Input.GetKeyDown(KeyCode.L))
        {
            party[0].RemoveStatus(testStatus);
        }
    }
}
