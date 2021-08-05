using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    // Setup/Initialization
    static BattleManager _instance;
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

    // Events
    // encounter loading/starting
    public event Action OnBattleManagerLoad;
    public event Action OnEncounterAwake;
    public event Action OnEncounterStart;
    public event Action OnEncounterEnd;
    public event Action OnEncounterWon;
    public event Action OnEncounterLost;
    // skill events
    public event Action<SkillData, UnitData, GridCell> OnSkillTarget;
    public event Action<SkillData, UnitData> OnSkillTargetCancel;
    public event Action<SkillData, GridCell, GridCell> OnSkillSelectTarget;
    public event Action<SkillData, GridCell, GridCell> OnSkillSelectTargetCancel;
    public event Action<SkillData, UnitData, List<GridCell>> OnSkillPreview;
    public event Action OnSkillPreviewCancel;
    public event Action<SkillData, UnitData, List<GridCell>> OnSkillConfirm;
    public event Action<SkillData, UnitData, List<GridCell>> OnSkillExecute;
    public event Action<SkillData> OnSkillClear;
    // arcana events
    public event Action<DivinationData, UnitData> OnDivinationTarget;
    public event Action<DivinationData, UnitData> OnDivinationTargetCancel;
    public event Action<DivinationData, UnitData, List<RiverCard>> OnDivinationConfirm;
    public event Action<DivinationData> OnDivinationClear;
    public event Action<List<RiverCard>> OnInitializeRiver;
    public event Action<List<RiverCard>> OnUpdateRiver;

    // Encounter handling
    List<ITurnTaker> _enemies = new List<ITurnTaker>();
    public List<ITurnTaker> enemies
    {
        get
        {
            return _enemies;
        }
        private set
        {
            _enemies = value;
        }
    }
    List<ITurnTaker> _party = new List<ITurnTaker>();
    public List<ITurnTaker> party
    {
        get
        {
            return _party;
        }
        private set
        {
            _party = value;
        }
    }
    List<UnitData> combatants = new List<UnitData>();
    List<EncounterCondition> curEncounterLossConditions = new List<EncounterCondition>();
    [SerializeField]
    EncounterCondition defaultEncounterLossCondition;
    List<EncounterCondition> curEncounterWinConditions = new List<EncounterCondition>();
    [SerializeField]
    EncounterCondition defaultEncounterWinCondition;
    public List<RiverCard> riverCards { get; private set; }
    [SerializeField]
    int riverSize = 5;
    [SerializeField]
    List<ElementData> allElements = new List<ElementData>();
    public bool inCombat { get; private set; }
    
    // Skill handling
    public bool previewingSkill { get; private set; }
    public SkillData curSkill { get; private set; }

    // Divination handling
    public DivinationData curDivination { get; private set; }
    public List<RiverCard> curDivinationTargets = new List<RiverCard>();
    public bool divinationTargeting { get; private set; }

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

    public void StartEncounter(List<ITurnTaker> enemyCombatants, List<EncounterCondition> winConditions, List<EncounterCondition> lossConditions)
    {
        combatants.Clear();
        party.Clear();
        enemies.Clear();
        curEncounterWinConditions.Clear();
        curEncounterLossConditions.Clear();

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

        // handle win conditions
        if(winConditions.Count == 0)
        {
            curEncounterWinConditions.Add(defaultEncounterWinCondition);
        }
        else
        {
            curEncounterWinConditions.AddRange(winConditions);
        }

        // handle loss conditions
        if(lossConditions.Count == 0)
        {
            curEncounterLossConditions.Add(defaultEncounterLossCondition);
        }
        else
        {
            curEncounterLossConditions.AddRange(lossConditions);
        }

        inCombat = true;

        turnManager = new TurnManager(turnTakers);

        OnEncounterAwake?.Invoke();

        StartCoroutine(DelayEncounterStart(2f));
    }

    IEnumerator DelayEncounterStart(float delayTime = 2f)
    {
        yield return new WaitForSeconds(2f);

        InitializeRiver();
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

    public void SkillPreview(SkillData skillData, UnitData unitData, List<GridCell> targets)
    {
        curSkill = skillData;
        curTargets = targets;
        previewingSkill = true;
        OnSkillPreview?.Invoke(skillData, unitData, targets);
    }

    public void SkillPreviewCancel()
    {
        if(previewingSkill)
        {
            OnSkillPreviewCancel?.Invoke();
            previewingSkill = false;
        }
    }

    public void SkillConfirm(SkillData skillData, UnitData unitData, List<GridCell> targets)
    {
        previewingSkill = false;
        unitData.hp -= skillData.hpCost;
        unitData.mp -= skillData.mpCost;
        
        switch(skillData.actionType)
        {
            case ActionType.MOVE:
                curUnit.moved = true;
            break;
            case ActionType.STANDARD:
                curUnit.acted = true;
            break;
            case ActionType.BONUS:
                curUnit.usedBonus = true;
            break;
        }
        targeting = false;
        OnSkillConfirm?.Invoke(skillData, unitData, targets);
    }

    public void SkillExecute()
    {
        curSkill.executedOn.Clear();
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
        curSkill.executedOn.Clear();
    }

    public void SkillClear()
    {
        previewingSkill = false;
        List<RiverCard> updatedRiver = new List<RiverCard>();
        foreach(RiverCard riverCard in riverCards)
        {
            bool matched = false;
            if(!riverCard.locked)
            {
                foreach(ElementData element in curSkill.elements)
                {
                    if(riverCard.element == element)
                    {
                        matched = true;
                        break;
                    }
                }
            }
            if(!matched) // matched cards go away
            {
                updatedRiver.Add(riverCard);
            }
        }
        riverCards = updatedRiver;
        RefillRiver(); // fill up any missing cards in the river
        OnUpdateRiver?.Invoke(riverCards);
        OnSkillClear?.Invoke(curSkill);
        curSkill = null;
        curTargets.Clear();
        
        if(!curUnit.incapacitated)
        {
            // TODO: make EXP more refined
            curUnit.stats.AddExperience(10, curUnit);
            UnitJob unitJob = curUnit.GetUnitJob(curUnit.activeJob);
            unitJob.AddExperience(10, curUnit);
        }

        IsEncounterResolved();
    }

    public void DivinationTarget(DivinationData divinationData)
    {
        curDivination = divinationData;
        divinationTargeting = true;
        OnDivinationTarget?.Invoke(divinationData, curUnit);
    }

    public void DivinationTargetCancel(DivinationData divinationData)
    {
        curDivination = null;
        divinationTargeting = false;
        OnDivinationTargetCancel?.Invoke(divinationData, curUnit);
    }

    public void DivinationConfirm(DivinationData divinationData, List<RiverCard> selectedRiverCards)
    {
        divinationTargeting = false;
        curUnit.usedBonus = true;
        curDivinationTargets.AddRange(selectedRiverCards);
        if(curDivination.executeAnimation != null)
        {
            GameObject skillAnimGO = Instantiate(curDivination.executeAnimation);
            skillAnimGO.GetComponent<DivinationAnimation>().SetDivination(curDivination, curUnit, selectedRiverCards);
        }
        else
        {
            curDivination.Execute(curUnit, selectedRiverCards);
        }
        OnDivinationConfirm?.Invoke(divinationData, curUnit, selectedRiverCards);
    }

    public void DivinationClear()
    {
        OnDivinationClear?.Invoke(curDivination);
        curDivination = null;
        curDivinationTargets.Clear();
    }

    public bool IsEncounterResolved()
    {
        bool win = false;
        foreach(EncounterCondition winCondition in curEncounterWinConditions)
        {
            if(winCondition.IsCompleted())
            {
                win = true;
                break;
            }
        }

        if(win)
        {
            EncounterWon();
            return true;
        }

        bool loss = false;
        foreach(EncounterCondition lossCondition in curEncounterLossConditions)
        {
            if(lossCondition.IsCompleted())
            {
                loss = true;
                break;
            }
        }

        if(loss)
        {
            EncounterLost();
            return true;
        }

        return false;
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

    void InitializeRiver()
    {
        List<RiverCard> initialCards = new List<RiverCard>();
        for(int i = 0; i < riverSize; i++)
        {
            initialCards.Add(GetRandomRiverCard());
        }
        OnInitializeRiver?.Invoke(initialCards);
        riverCards = initialCards;
        OnUpdateRiver?.Invoke(riverCards);
    }

    RiverCard GetRandomRiverCard()
    {
        ElementData element = allElements[UnityEngine.Random.Range(0, allElements.Count)];
        return new RiverCard(element, false, false);
    }

    public void FlowRiver(int numCards = 1)
    {
        if(numCards > 0)
        {
            int numFound = 0;
            int lastRiverIndex = riverCards.Count - 1;
            for(int i = lastRiverIndex; i >= 0; i--)
            {
                if(riverCards[i].locked)
                {
                    continue;
                }
                riverCards.RemoveAt(i);
                numFound++;
                if(numFound == numCards)
                {
                    break;
                }
            }
        }

        RefillRiver();
        OnUpdateRiver?.Invoke(riverCards);
    }

    public void RandomizeRiver()
    {
        List<RiverCard> initialCards = new List<RiverCard>();

        foreach(RiverCard riverCard in riverCards)
        {
            if(riverCard.inactive || riverCard.locked)
            {
                initialCards.Add(riverCard);
            }
        }
        riverCards = initialCards;
        RefillRiver();
        OnUpdateRiver?.Invoke(riverCards);
    }

    void RefillRiver()
    {
        while(riverCards.Count < riverSize)
        {
            riverCards.Insert(0, GetRandomRiverCard());
        }
    }

    public int GetRiverMatches(List<ElementData> elementsToMatch)
    {
        int matches = 0;
        foreach(RiverCard riverCard in riverCards)
        {
            if(!riverCard.inactive && elementsToMatch.Contains(riverCard.element))
            {
                matches++;
            }
        }
        return matches;
    }
}
