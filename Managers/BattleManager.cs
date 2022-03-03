using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    #region Initialization
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
    #endregion

    #region Events
    // encounter loading/starting
    public event Action OnBattleManagerLoad;
    public event Action OnEncounterAwake;
    public event Action OnEncounterStart;
    public event Action OnEncounterEnd;
    public event Action OnEncounterWon;
    public event Action OnEncounterLost;
    // prepared skill events
    public event Action<BattleSkill> OnPreparedSkillInit;
    public event Action<BattleSkill> OnPreparedSkillPreviewTarget;
    public event Action<BattleSkill> OnPreparedSkillSelectTarget;
    public event Action<BattleSkill> OnPreparedSkillCancelTarget;
    public event Action<BattleSkill> OnPreparedSkillConfirmPrep;
    public event Action<BattleSkill> OnPreparedSkillConfirm;
    public event Action OnPreparedSkillCancel;
    public event Action<BattleSkill> OnSkillPaidCosts;
    public event Action<BattleSkill> OnInterruptingSkillComplete;
    public event Action<BattleSkill> OnPreparedSkillComplete;

    // arcana events
    public event Action<DivinationData, UnitData> OnDivinationTarget;
    public event Action<DivinationData, UnitData> OnDivinationTargetCancel;
    public event Action<DivinationData, UnitData, List<RiverCard>> OnDivinationConfirm;
    public event Action<DivinationData> OnDivinationClear;
    public event Action<List<RiverCard>> OnInitializeRiver;
    public event Action<List<RiverCard>> OnUpdateRiver;
    #endregion

    #region Interrupts
    private Stack<Action<ModBool, Action<ModBool>>> interrupts = new Stack<Action<ModBool, Action<ModBool>>>();
    #endregion

    #region Encounter Handling
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
    #endregion

    #region River
    public List<RiverCard> riverCards { get; private set; }
    [SerializeField]
    int riverSize = 5;
    [SerializeField]
    List<ElementData> allElements = new List<ElementData>();
    #endregion
    
    #region Skill Handling
    public bool targeting { get; private set; }
    public bool previewingSkill { get; private set; }
    public SkillData curSkill { get; private set; }
    public List<SkillData> curSkills = new List<SkillData>();
    public List<GridCell> curTargets = new List<GridCell>();
    public bool executingSkill { get; private set; }
    public BattleSkill curBattleSkill { get; private set; }
    Stack<BattleSkill> battleSkills = new Stack<BattleSkill>();

    public BattleSkill curExecutingSkill { get; private set; }
    public BattleSkill preparedSkill { get; private set; }
    Stack<BattleSkill> interruptingSkills = new Stack<BattleSkill>();
    public bool targetLocked { get; private set; }
    #endregion

    #region Divination handling
    public DivinationData curDivination { get; private set; }
    public List<RiverCard> curDivinationTargets = new List<RiverCard>();
    public bool divinationTargeting { get; private set; }
    #endregion

    #region Traps
    [SerializeField] GameObject trapGameObject;
    #endregion
    
    #region State Variables
    public bool inCombat { get; private set; }
    public bool isCurrentUnitMoving 
    { 
        get
        {
            return curUnit != null && curUnit.unitGO != null && curUnit.unitGO.GetComponent<TacticsMotor>().isMoving;
        } 
    }

    // Player is in the process of selecting what to do on their turn
    public bool isManuallySelectingAction
    {
        get
        {
            return curUnit != null && curUnit.isPlayerControlled && !isCurrentUnitMoving && !executingSkill;
        }
    }
    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        mapManager = MapManager.Instance;

        OnBattleManagerLoad?.Invoke();
    }

    #region Encounter Lifecycle
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

        ModBool cancelledExecute = new ModBool(false);
        Action<ModBool> callback = (ModBool cancelled) => 
        {
            if(cancelled.GetCalculated())
            {
                EndEncounter();
            }
            else
            {
                InitializeRiver();
                OnEncounterStart?.Invoke();
                turnManager.StartNextTurn();
            }
        };
        ResolveInterrupts(cancelledExecute, callback);
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
        // TODO: Actual encounter rewards
        OnEncounterWon?.Invoke();
        EndEncounter();
    }

    public void EncounterLost()
    {
        // TODO: Actual encounter cleanup?
        OnEncounterLost?.Invoke();
        EndEncounter();
    }
    #endregion

    #region Divination Lifecycle
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
    #endregion

    #region River Lifecycle
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

        /**
         * TODO: determine if we want this
        if(curUnit != null)
        {
            ModInt modMatches = new ModInt(matches);
            curUnit.GetUnitMatches(modMatches, elementsToMatch);
            matches = modMatches.GetCalculated();
        }
        **/

        return matches;
    }
    #endregion

    #region Interrupts
    public void AddInterrupt(Action<ModBool, Action<ModBool>> interrupt)
    {
        interrupts.Push(interrupt);
    }

    public void ResolveInterrupts(ModBool cancelExecution, Action<ModBool> completedCallback)
    {
        if(interrupts.Count > 0) 
        {
            var interrupt = interrupts.Pop();
            interrupt.Invoke(cancelExecution, completedCallback);
        }
        else {
            completedCallback.Invoke(cancelExecution);
        }
    }

    public void ResolveInterrupt(ModBool cancelExecution, Action<ModBool> completedCallback)
    {
        ResolveInterrupts(cancelExecution, completedCallback);
    }

    public bool HasInterrupts()
    {
        return interrupts.Count > 0;
    }
    #endregion

    #region Helper Functions
    public List<UnitData> GetCombatants()
    {
        return combatants;
    }

    public GameObject GetTrap()
    {
        return trapGameObject;
    }
    #endregion

    #region Prepared Skills
    public BattleSkill PreparedSkillInit(SkillData skill, UnitData source, GridCell skillOrigin = null)
    {
        GridCell origin = (skillOrigin != null) ? skillOrigin : mapManager.GetCell(source.curPosition);
        BattleSkill battleSkill = new BattleSkill(skill, source, origin, true);
        return PreparedSkillInit(battleSkill);
    }

    public BattleSkill PreparedSkillInit(BattleSkill battleSkill)
    {
        preparedSkill = battleSkill;
        targeting = true;
        preparedSkill.StartTargeting();
        mapManager.RenderTargetableTiles(preparedSkill.targetableArea);
        targetLocked = false;

        OnPreparedSkillInit?.Invoke(preparedSkill);
        return preparedSkill;
    }

    public void PreparedSkillCancel()
    {
        mapManager.ClearTargetableTiles(preparedSkill.targetableArea);
        mapManager.ClearTargetedTiles(preparedSkill.targetedArea);
        preparedSkill.EndTargeting();
        targeting = false;
        preparedSkill = null;
        OnPreparedSkillCancel?.Invoke();
    }

    public void PreparedSkillPreviewTarget(GridCell target)
    {
        mapManager.ClearTargetedTiles(preparedSkill.targetedArea);
        mapManager.RenderTargetableTiles(preparedSkill.targetableArea);
        preparedSkill.SetTarget(target);
        mapManager.RenderTargetedTiles(preparedSkill.targetedArea);

        OnPreparedSkillPreviewTarget?.Invoke(preparedSkill);
    }

    public void PreparedSkillSelectTarget(GridCell target)
    {
        if(!targeting)
        {
            return;
        }

        mapManager.ClearTargetedTiles(preparedSkill.targetedArea);
        mapManager.RenderTargetableTiles(preparedSkill.targetableArea);
        preparedSkill.SetTarget(target);
        mapManager.RenderTargetedTiles(preparedSkill.targetedArea);
        targetLocked = true;
        OnPreparedSkillSelectTarget?.Invoke(preparedSkill);
    }

    public void PreparedSkillCancelTarget()
    {
        targetLocked = false;
        mapManager.ClearTargetedTiles(preparedSkill.targetedArea);
        preparedSkill.SetTarget(null);
        mapManager.ClearTargetableTiles(preparedSkill.targetableArea);
        mapManager.RenderTargetableTiles(preparedSkill.targetableArea);
        OnPreparedSkillCancelTarget?.Invoke(preparedSkill);
    }

    public void PreparedSkillConfirm()
    {
        OnPreparedSkillConfirmPrep?.Invoke(preparedSkill);
        preparedSkill.SetCompletionCallback(PreparedSkillComplete);
        mapManager.ClearTargetableTiles(preparedSkill.targetableArea);
        mapManager.ClearTargetableTiles(preparedSkill.targetedArea);
        OnPreparedSkillConfirm?.Invoke(preparedSkill);
        PerformSkill(preparedSkill);
    }

    public void AddSkill(BattleSkill battleSkill)
    {
        //  empty stack, this is likely an arbritrary, non-reactionary skill
        if(preparedSkill == null)
        {
            preparedSkill = battleSkill;
            PreparedSkillConfirm();
        }
        else // we are already doing something else, queue it up to use when available
        {
            interruptingSkills.Push(battleSkill);
        }
    }

    void PerformSkill(BattleSkill battleSkill)
    {
        Debug.Log("Performing Skill: "+ battleSkill.skill.skillName);
        Debug.Log("Target: " + battleSkill.targetOrigin.position.ToString());
        executingSkill = true;
        battleSkill.PayCosts();
        OnSkillPaidCosts?.Invoke(battleSkill);
        battleSkill.Confirm();
    }

    public void PerformSkillOnTarget(BattleSkill battleSkill, GridCell target, Action callback)
    {
        Action targetCallback = () =>
        {
            Debug.Log("Check for interrupt");
            ResolveNextInterruptSkill(callback);
        };
        Debug.Log("Executing " + battleSkill.skill.skillName + " on " + ((target.occupiedBy != null) ? target.occupiedBy.unitName : "Empty Tile") + " at " + target.position.ToString());
        battleSkill.ExecutePerTarget(target, targetCallback);
    }

    public void ResolveNextInterruptSkill(Action callback)
    {
        StartCoroutine(SkillInterruptRoutine(callback));
    }

    IEnumerator SkillInterruptRoutine(Action callback)
    {
        if(interruptingSkills.Count > 0)
        {
            BattleSkill interruptingSkill = interruptingSkills.Pop();
            Action interruptingSkillCallback = () =>
            {
                OnInterruptingSkillComplete.Invoke(interruptingSkill);
                ResolveNextInterruptSkill(callback);
            };
            interruptingSkill.SetCompletionCallback(callback);

            mapManager.RenderTargetedTiles(interruptingSkill.targetedArea);
            yield return new WaitForSeconds(1f);
            mapManager.ClearTargetedTiles(interruptingSkill.targetedArea);
            PerformSkill(interruptingSkill);
        }
        else
        {
            callback.Invoke();
        }
        yield return null;
    }

    public void PreparedSkillComplete()
    {
        executingSkill = false;
        targeting = false;
        targetLocked = false;
        OnPreparedSkillComplete?.Invoke(preparedSkill);
        preparedSkill = null;
    }
    #endregion
}