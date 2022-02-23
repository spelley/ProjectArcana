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
    public event Action<SkillData, UnitData, List<GridCell>> OnSkillConfirmPrep;
    public event Action<SkillData, UnitData, List<GridCell>> OnSkillExecute;
    public event Action<SkillData> OnSkillClear;
    // arcana events
    public event Action<DivinationData, UnitData> OnDivinationTarget;
    public event Action<DivinationData, UnitData> OnDivinationTargetCancel;
    public event Action<DivinationData, UnitData, List<RiverCard>> OnDivinationConfirm;
    public event Action<DivinationData> OnDivinationClear;
    public event Action<List<RiverCard>> OnInitializeRiver;
    public event Action<List<RiverCard>> OnUpdateRiver;
    // interrupts
    private Stack<Action<ModBool, Action<ModBool>>> interrupts = new Stack<Action<ModBool, Action<ModBool>>>();

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
    public bool targeting { get; private set; }
    public bool previewingSkill { get; private set; }
    public SkillData curSkill { get; private set; }
    public List<GridCell> curTargets = new List<GridCell>();
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
    public BattleSkill curBattleSkill { get; private set; }
    Stack<BattleSkill> battleSkills = new Stack<BattleSkill>();

    // Divination handling
    public DivinationData curDivination { get; private set; }
    public List<RiverCard> curDivinationTargets = new List<RiverCard>();
    public bool divinationTargeting { get; private set; }

    // TODO: remove me
    public SkillData testSkill;

    // Trap handling
    [SerializeField] GameObject trapGameObject;
    

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
        // Debug.Log"Skill Confirm");
        previewingSkill = false;
        unitData.hp -= skillData.hpCost;
        unitData.mp -= skillData.mpCost;
        
        switch(skillData.actionType)
        {
            case ActionType.MOVE:
                unitData.moved = true;
            break;
            case ActionType.STANDARD:
                unitData.acted = true;
            break;
            case ActionType.BONUS:
                unitData.usedBonus = true;
            break;
        }
        targeting = false;
        
        // add the starting battle skill
        AddBattleSkill(skillData, unitData, targets);

        OnSkillConfirmPrep?.Invoke(skillData, unitData, targets);

        ModBool cancelExecution = new ModBool(false);
        Action<ModBool> confirmCallback = (ModBool cancelled) => {
            if(cancelled.GetCalculated()) {
                SkillClear();
            }
            else {
                StartCoroutine(BattleSkillStackRoutine());
            }
        };
        ResolveInterrupts(cancelExecution, confirmCallback);   
    }

    public void AddBattleSkill(SkillData skill, UnitData unit, List<GridCell> targets)
    {
        BattleSkill battleSkill = new BattleSkill(skill, unit, targets);
        battleSkills.Push(battleSkill);
    }

    IEnumerator BattleSkillStackRoutine()
    {
        while(battleSkills.Count > 0)
        {
            if(curBattleSkill == null)
            {
                curBattleSkill = battleSkills.Pop();
                Action<ModBool> interruptCheck = (ModBool cancelled) => {
                    if(!cancelled.GetCalculated())
                    {
                        OnSkillConfirm?.Invoke(curBattleSkill.skillData, curBattleSkill.unitData, curBattleSkill.targets);
                        curUnit.unitGO.GetComponent<TacticsMotor>().UseSkill(curBattleSkill.skillData, curBattleSkill.unitData, curBattleSkill.targets);
                    }
                    else {
                        Debug.Log("BattleSkillStackRoutine Cancelled");
                        SkillClear();
                    }
                };

                ModBool cancelExecution = new ModBool(false);
                BattleManager.Instance.ResolveInterrupts(cancelExecution, interruptCheck);
            }
            yield return null;
        }
    }

    public void SkillExecute()
    {
        SkillExecute(curBattleSkill.skillData, curBattleSkill.unitData, curBattleSkill.targets);
    }

    public void SkillExecute(SkillData skill, UnitData unit, List<GridCell> targets)
    {
        skill.executedOn.Clear();
        Debug.Log(skill.skillName);
        if(skill.executeAnimation != null)
        {
            Debug.Log("Executed with animation - "+skill.skillName);
            Debug.Log(unit.unitName);
            Debug.Log(targets[0].position.ToString());
            GameObject skillAnimGO = Instantiate(skill.executeAnimation);
            skillAnimGO.GetComponent<SkillAnimation>().SetSkill(skill, unit, targets);
        }
        else
        {
            Debug.Log("executed without animation");
            skill.Execute(unit, targets);
        }
        OnSkillExecute?.Invoke(skill, unit, targets);
        skill.executedOn.Clear();
    }

    public void SkillClear()
    {
        // process river changes per skill
        List<RiverCard> updatedRiver = new List<RiverCard>();
        foreach(RiverCard riverCard in riverCards)
        {
            bool matched = false;
            if(!riverCard.locked)
            {
                foreach(ElementData element in curBattleSkill.skillData.elements)
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

        // the stack is clear, this is the last skill to be cleared
        if(battleSkills.Count == 0)
        {
            // clean up our initial starting skill
            curSkill = null;
            curTargets.Clear();
            previewingSkill = false;

            // let the game know we are clearing this skill
            OnSkillClear?.Invoke(curBattleSkill.skillData);
            curBattleSkill = null;

            /**
            if(!curUnit.incapacitated)
            {
                // TODO: make EXP more refined
                curUnit.stats.AddExperience(10, curUnit);
                UnitJob unitJob = curUnit.GetUnitJob(curUnit.activeJob);
                unitJob.AddExperience(10, curUnit);
            }
            **/
            
            Action<ModBool> cleanupSkill = (ModBool cancelled) => {
                IsEncounterResolved();
            };
            ModBool cancelExecution = new ModBool(false);
            BattleManager.Instance.ResolveInterrupts(cancelExecution, cleanupSkill);
        }

        // null out the current battle skill
        curBattleSkill = null;
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
        // Debug.Log"Battle has been won!");
        // TODO: Actual encounter rewards
        OnEncounterWon?.Invoke();
        EndEncounter();
    }

    public void EncounterLost()
    {
        // Debug.Log"Battle has been lost!");
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

        if(curUnit != null)
        {
            ModInt modMatches = new ModInt(matches);
            curUnit.GetUnitMatches(modMatches, elementsToMatch);
            matches = modMatches.GetCalculated();
        }

        return matches;
    }

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

    public GameObject GetTrap()
    {
        return trapGameObject;
    }
}
