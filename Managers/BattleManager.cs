using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private static BattleManager _instance;
    public static BattleManager Instance 
    { 
        get 
        { 
            return _instance; 
        }
    }

    public TurnManager turnManager;
    GameManager gameManager;
    MapManager mapManager;
    
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
        }
    }

    // events
    public event Action OnBattleManagerLoad;
    public event Action OnEncounterStart;
    public event Action OnEncounterEnd;
    public event Action OnEncounterWon;
    public event Action OnEncounterLost;
    public event Action<SkillData, UnitData, GridCell> OnSkillTarget;
    public event Action<SkillData, UnitData> OnSkillTargetCancel;
    public event Action<SkillData, GridCell, GridCell> OnSkillSelectTarget;
    public event Action<SkillData, GridCell, GridCell> OnSkillSelectTargetCancel;
    public event Action<SkillData, UnitData, List<GridCell>> OnSkillConfirm;
    public event Action<SkillData, UnitData, List<GridCell>> OnSkillExecute;
    public event Action<SkillData> OnSkillClear;

    List<ITurnTaker> enemies = new List<ITurnTaker>();
    List<ITurnTaker> party = new List<ITurnTaker>();
    List<UnitData> combatants = new List<UnitData>();
    public bool inCombat { get; private set; }
    public SkillData curSkill { get; private set; }
    public UnitData curUnit
    {
        get
        {
            return turnManager != null ? turnManager.curTurnTaker as UnitData : null;
        }
    }
    public ITurnTaker curTurnTaker
    {
        get
        {
            return turnManager != null ? turnManager.curTurnTaker : null;
        }
    }
    public List<GridCell> curTargets = new List<GridCell>();
    public bool targeting { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        mapManager = MapManager.Instance;

        OnBattleManagerLoad?.Invoke();
    }

    public void StartEncounter(List<ITurnTaker> enemyCombatants)
    {
        combatants.Clear();
        party.Clear();
        enemies.Clear();
        List<ITurnTaker> turnTakers = new List<ITurnTaker>();

        combatants.AddRange(gameManager.party);
        gameManager.activePlayer.unitGO.GetComponent<TacticsMotor>().SnapToGrid();

        int formationPosition = 0;
        for(int u = 0; u < gameManager.party.Count; u++)
        {
            UnitData turnTaker = gameManager.party[u];
            if(turnTaker != null && turnTaker != gameManager.activePlayer)
            {
                Vector3Int offsetPosition = gameManager.curFormation.offsets[formationPosition] + gameManager.activePlayer.curPosition;
                GridCell closestCell = mapManager.GetClosestUnoccupiedGridCell(offsetPosition, turnTaker);
                GameObject ally = gameManager.SpawnUnit(turnTaker, closestCell.realWorldPosition);
                ally.GetComponent<TacticsMotor>().SnapToGrid();
                formationPosition++;
            }
            party.Add(turnTaker);
            turnTakers.Add(turnTaker);
        }

        if(enemyCombatants.Count > 0)
        {
            foreach(UnitData turnTaker in enemyCombatants)
            {
                UnitData enemyUnit = turnTaker as UnitData;
                if(enemyUnit != null)
                {
                    combatants.Add(enemyUnit);
                    enemies.Add(enemyUnit);
                    enemyUnit.unitGO.GetComponent<TacticsMotor>().SnapToGrid();
                }
                turnTakers.Add(turnTaker);
            }
        }

        inCombat = true;

        // TODO: write script to detect enemies
        turnManager = new TurnManager(turnTakers);
        OnEncounterStart?.Invoke();
        turnManager.StartNextTurn();
    }

    public void EndEncounter()
    {
        inCombat = false;
        ResetEncounter();
        OnEncounterEnd?.Invoke();
        for(int u = 0; u < gameManager.party.Count; u++)
        {
            UnitData turnTaker = gameManager.party[u];
            if(turnTaker != null && turnTaker != gameManager.activePlayer)
            {
                GameObject.Destroy(turnTaker.unitGO);
                turnTaker.unitGO = null;
            }
        }
        Camera.main.GetComponent<CameraController>().SetFocus(GameObject.FindWithTag("Player"));
        turnManager = null;
    }

    void ResetEncounter()
    {
        enemies.Clear();
        party.Clear();
    }

    public List<UnitData> GetCombatants()
    {
        return combatants;
    }

    public void SkillTarget(SkillData skillData, UnitData unitData, GridCell originCell)
    {
        targeting = true;
        curSkill = skillData;
        OnSkillTarget?.Invoke(skillData, unitData, originCell);
    }

    public void SkillTargetCancel(SkillData skillData, UnitData unitData)
    {
        targeting = false;
        curSkill = null;
        OnSkillTargetCancel?.Invoke(skillData, unitData);
    }

    public void SkillSelectTarget(SkillData skillData, GridCell targetCell)
    {
        if(!targeting)
        {
            return;
        }

        GridCell originCell = null;
        if(turnManager?.curTurnTaker != null)
        {
            originCell = mapManager.GetCell(turnManager.curTurnTaker.curPosition);
        }
        else
        {
            originCell = GameObject.FindWithTag("Player").GetComponent<TacticsMotor>().curCell;
        }
        curSkill = skillData;
        OnSkillSelectTarget?.Invoke(skillData, originCell, targetCell);
    }
    public void SkillSelectTargetCancel(SkillData skillData, GridCell targetCell)
    {
        // TODO: fix originCell to get the current player position
        GridCell originCell = null;
        if(turnManager?.curTurnTaker != null)
        {
            originCell = mapManager.GetCell(turnManager.curTurnTaker.curPosition);
        }
        else
        {
            originCell = GameObject.FindWithTag("Player").GetComponent<TacticsMotor>().curCell;
        } 
        OnSkillSelectTargetCancel?.Invoke(skillData, originCell, targetCell);
    }

    public void SkillConfirm(SkillData skillData, UnitData unitData, List<GridCell> targets)
    {
        curUnit.acted = true;
        targeting = false;

        curSkill = skillData;
        curTargets = targets;
        OnSkillConfirm?.Invoke(skillData, unitData, targets);
    }

    public void SkillExecute()
    {
        if(curSkill.executeAnimation != null)
        {
            GameObject skillAnimGO = Instantiate(curSkill.executeAnimation);
            skillAnimGO.GetComponent<SkillAnimation>().SetSkill(curSkill, curUnit, curTargets);
        }
        else
        {
            curSkill.Execute(curUnit, curTargets);
        }
        OnSkillExecute?.Invoke(curSkill, curUnit, curTargets);
    }

    public void SkillClear()
    {
        OnSkillClear?.Invoke(curSkill);
        curSkill = null;
        curTargets.Clear();
        CheckIfWin();
    }

    public void CheckIfWin()
    {
        bool allAlliesIncap = true;
        foreach(UnitData ally in party)
        {
            if(!ally.incapacitated)
            {
                allAlliesIncap = false;
                break;
            }
        }

        if(allAlliesIncap)
        {
            EncounterLost();
            return;
        }

        bool allEnemiesIncap = true;
        foreach(UnitData enemy in enemies)
        {
            if(!enemy.incapacitated)
            {
                allEnemiesIncap = false;
                break;
            }
        }

        if(allEnemiesIncap)
        {
            EncounterWon();
            return;
        }
        // TODO: Add WinCondition checking
    }

    public void EncounterWon()
    {
        Debug.Log("Battle has been won!");
        // TODO: Actual encounter rewards
        OnEncounterWon?.Invoke();
        EndEncounter();
    }

    public void EncounterLost()
    {
        Debug.Log("Battle has been lost!");
        // TODO: Actual encounter cleanup?
        OnEncounterLost?.Invoke();
        EndEncounter();
    }
}
