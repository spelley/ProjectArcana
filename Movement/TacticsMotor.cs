using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMotor : MonoBehaviour
{
    // cached references
    BattleManager battleManager;
    MapManager mapManager;
    CameraController cameraController;
    CharacterMotor characterMotor;

    public Animator anim { get; private set; }

    public GridCell curCell { get; private set; }
    
    // state variables
    public bool isMoving { get; private set; }
    
    void Awake()
    {
        characterMotor = GetComponent<CharacterMotor>();
        anim = GetComponent<Animator>();
        cameraController = Camera.main.GetComponent<CameraController>();

        mapManager = MapManager.Instance;
        battleManager = BattleManager.Instance;
        if(battleManager != null)
        {
            battleManager.OnEncounterAwake += OnEncounterAwake;
            battleManager.OnEncounterStart += OnEncounterStart;
            battleManager.OnEncounterEnd += OnEncounterEnd;
        }
    }

    void Start()
    {
        if(mapManager == null)
        {
            mapManager = MapManager.Instance;
        }

        if(battleManager == null)
        {
            battleManager = BattleManager.Instance;
            if(battleManager != null)
            {
                battleManager.OnEncounterAwake += OnEncounterAwake;
                battleManager.OnEncounterStart += OnEncounterStart;
                battleManager.OnEncounterEnd += OnEncounterEnd;
            }
        }
    }

    void OnDestroy()
    {
        if(battleManager != null)
        {
            battleManager.OnEncounterAwake -= OnEncounterAwake;
            battleManager.OnEncounterStart -= OnEncounterStart;
            battleManager.OnEncounterEnd -= OnEncounterEnd;

            battleManager.OnSkillConfirm -= OnSkillConfirm;
            battleManager.OnSkillExecute -= OnSkillExecute;
            battleManager.OnSkillClear -= OnSkillClear;
            mapManager.OnTravelPath -= OnTravelPath;
        }
        characterMotor.unitData.OnUnitTurnStart -= OnUnitTurnStart;
        characterMotor.unitData.OnUnitTurnEnd -= OnUnitTurnEnd;
    }

    public void OnEncounterAwake()
    {
        characterMotor.locked = true;
        SnapToGrid();
    }

    public void OnEncounterStart()
    {
        characterMotor.unitData.OnUnitTurnStart += OnUnitTurnStart;
        characterMotor.unitData.OnUnitTurnEnd += OnUnitTurnEnd;
        characterMotor.unitData.OnUnitPush += OnUnitPush;
    }

    public void OnEncounterEnd()
    {
        characterMotor.locked = false;

        characterMotor.unitData.OnUnitTurnStart -= OnUnitTurnStart;
        characterMotor.unitData.OnUnitTurnEnd -= OnUnitTurnEnd;
        characterMotor.unitData.OnUnitPush -= OnUnitPush;

        mapManager.ClearAllTiles();
    }

    void OnUnitTurnStart()
    {
        battleManager.OnSkillConfirm += OnSkillConfirm;
        battleManager.OnSkillExecute += OnSkillExecute;
        battleManager.OnSkillClear += OnSkillClear;
        mapManager.OnTravelPath += OnTravelPath;

        cameraController.SetFocus(this.gameObject);

        AIBrain aiBrain = characterMotor.unitData.aiBrain;
        if(aiBrain != null)
        {
            AI(aiBrain);
        }
    }

    void AI(AIBrain aiBrain)
    {
        mapManager.LoadWalkabilityZone(this.gameObject, this.transform.position);
        aiBrain.DetermineAction(characterMotor.unitData, ExecuteAI);
    }

    public void ExecuteAI(AIInstruction instruction)
    {
        StartCoroutine(AICoroutine(instruction));
    }

    IEnumerator AICoroutine(AIInstruction instruction)
    {
        yield return new WaitForSeconds(1f);
        Stack<GridCell> path = mapManager.GetAndRenderPath(instruction.walkCell);
        yield return new WaitForSeconds(.2f);
        if(path.Count > 0)
        {
            mapManager.TravelPath(path);
        }
        float cutoff = 10f;
        while(isMoving && cutoff > 0f)
        {
            cutoff -= Time.deltaTime;
            yield return null;
        }
        if(instruction.skill != null)
        {
            battleManager.SkillTarget(instruction.skill, characterMotor.unitData, mapManager.GetCell(characterMotor.unitData.curPosition));
            yield return new WaitForSeconds(.2f);
            battleManager.SkillSelectTarget(instruction.skill, instruction.targetCell);
            yield return new WaitForSeconds(.5f);
            battleManager.SkillPreview(instruction.skill, characterMotor.unitData, mapManager.GetTargetedCells());
            battleManager.SkillConfirm(instruction.skill, characterMotor.unitData, mapManager.GetTargetedCells());
        }
        else
        {
            StartCoroutine(DelayEndTurn());
        }
    }

    public void OnTravelPath(Stack<GridCell> path)
    {
        if(isMoving)
        {
            return;
        }
        
        mapManager.ClearAllTiles();
        StartCoroutine(TravelPathRoutine(path));
        characterMotor.unitData.moved = true;
    }

    IEnumerator TravelPathRoutine(Stack<GridCell> path)
    {
        float travelSpeed = .25f;
        float jumpSpeed = .125f;

        isMoving = true;
        anim.SetBool("Moving", true);

        Vector3 startPosition = this.transform.position;
        foreach(GridCell cell in path)
        {
            if(cell.position != startPosition)
            {
                float curTravelTime = 0f;
                float curJumpTime = 0f;
                Vector3 lookPos = cell.realWorldPosition - startPosition;
                lookPos.y = 0;
                if(lookPos != Vector3.zero)
                {
                    this.transform.rotation = Quaternion.LookRotation(lookPos);
                }
                while(curTravelTime < travelSpeed)
                {
                    curTravelTime += Time.deltaTime;
                    curJumpTime += Time.deltaTime;

                    Vector3 gridMove = Vector3.Lerp(startPosition, cell.realWorldPosition, (curTravelTime / travelSpeed));
                    if(startPosition.y < cell.realWorldPosition.y && (cell.realWorldPosition.y - startPosition.y) >= 1f)
                    {
                        float jumpHeight = Mathf.Lerp(startPosition.y, cell.realWorldPosition.y, (curJumpTime / jumpSpeed));
                        gridMove = new Vector3(gridMove.x, jumpHeight, gridMove.z);
                    }
                    this.transform.position = gridMove;
                    
                    yield return null;
                }
            }
            curCell = cell;
            startPosition = cell.realWorldPosition;
        }
        mapManager.UpdateUnitPosition(curCell.position, characterMotor.unitData);

        isMoving = false;
        anim.SetBool("Moving", false);
        mapManager.TravelPathEnd();
    }

    void OnUnitTurnEnd()
    {
        battleManager.OnSkillConfirm -= OnSkillConfirm;
        battleManager.OnSkillExecute -= OnSkillExecute;
        battleManager.OnSkillClear -= OnSkillClear;
        mapManager.OnTravelPath -= OnTravelPath;
        mapManager.ClearAllTiles();
    }

    void OnSkillConfirm(SkillData skillData, UnitData unitData, List<GridCell> targets)
    {
        Vector3 direction = (targets[0].realWorldPosition - transform.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);

        switch(skillData.castAnimation)
        {
            case BattleAnimation.BASIC_ATTACK:
                anim.SetTrigger("BasicAttack");
            break;
            default:
                anim.SetTrigger("BasicAttack");
            break;
        }
    }

    void OnSkillExecute(SkillData skillData, UnitData unitData, List<GridCell> targets)
    {

    }

    void OnSkillClear(SkillData skillData)
    {
        if(characterMotor.unitData.aiBrain != null)
        {
            StartCoroutine(DelayEndTurn());
        }
    }

    IEnumerator DelayEndTurn()
    {
        yield return null;
        yield return null;
        yield return new WaitForSeconds(.5f);
        battleManager.turnManager.EndTurn();
    }

    void OnUnitPush(GridCell gridCell, Action<Vector3Int> callback)
    {
        StartCoroutine(PushCoroutine(gridCell, .5f, callback));
    }

    IEnumerator PushCoroutine(GridCell gridCell, float lerpDuration, Action<Vector3Int> callback)
    {
        // TODO: push animation
        float timeElapsed = 0;
        Vector3 startPosition = this.transform.position;

        while (timeElapsed < lerpDuration)
        {
            this.transform.position = Vector3.Lerp(startPosition, gridCell.realWorldPosition, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        
        this.transform.position = gridCell.realWorldPosition;
        callback?.Invoke(gridCell.position);
    }

    public void SnapToGrid()
    {
        GridCell closestCell = mapManager.GetClosestUnoccupiedGridCell(this.transform.position, characterMotor.unitData);
        Debug.Log(closestCell.position);
        if(closestCell != null)
        {
            this.transform.position = closestCell.realWorldPosition;
            curCell = closestCell;
            mapManager.UpdateUnitPosition(curCell.position, characterMotor.unitData);
        }
        else {
            Debug.Log("uh oh");
        }
    }

    public void OnMouseEnter()
    {
        if(battleManager.curUnit == null || battleManager.curUnit.faction != Faction.ALLY || battleManager.previewingSkill)
        {
            return;
        }
        if(battleManager.curSkill)
        {
            battleManager.SkillSelectTarget(battleManager.curSkill, mapManager.GetCell(characterMotor.unitData.curPosition));
        }
    }

    public void OnMouseExit()
    {
        if(battleManager.curUnit == null || battleManager.curUnit.faction != Faction.ALLY)
        {
            return;
        }
        if(battleManager.curSkill != null)
        {
            //battleManager.SkillSelectTargetCancel(battleManager.curSkill, mapManager.GetCell(characterMotor.unitData.curPosition));
        }
    }

    public void OnMouseDown()
    {
        if(battleManager.curUnit == null || battleManager.curUnit.faction != Faction.ALLY)
        {
            return;
        }
        
        if(battleManager.targeting)
        {
            UnitData unitData = battleManager.curUnit;
            if(unitData == null)
            {
                unitData = GameObject.FindWithTag("Player").GetComponent<CharacterMotor>().unitData;
            }
            battleManager.SkillPreview(battleManager.curSkill, unitData, mapManager.GetTargetedCells());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(battleManager.turnManager == null || battleManager.curUnit != characterMotor.unitData || battleManager.curUnit.faction != Faction.ALLY)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Escape) && battleManager.targeting && battleManager.curSkill != null && !isMoving && !battleManager.previewingSkill)
        {
            battleManager.SkillTargetCancel(battleManager.curSkill, characterMotor.unitData);
        }
    }
}
