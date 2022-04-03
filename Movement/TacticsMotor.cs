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
        mapManager.OnTravelPath -= OnTravelPath;

        mapManager.ClearAllTiles();
    }

    void OnUnitTurnStart()
    {
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
            BattleSkill aiBattleSkill = new BattleSkill(instruction.skill, characterMotor.unitData, mapManager.GetCell(characterMotor.unitData.curPosition), true);
            battleManager.PreparedSkillInit(aiBattleSkill);
            yield return new WaitForSeconds(.5f);
            battleManager.PreparedSkillPreviewTarget(instruction.targetCell);
            battleManager.PreparedSkillSelectTarget(instruction.targetCell);
            yield return new WaitForSeconds(.8f);
            battleManager.PreparedSkillConfirm();
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
        bool processingInterrupt = false;

        isMoving = true;
        anim.SetBool("Moving", true);

        Vector3 startPosition = this.transform.position;
        bool continueWalking = true;

        if(path.Count > 0)
        {
            while(path.Count > 0 || processingInterrupt)
            {
                if(continueWalking && !processingInterrupt)
                {
                    GridCell cell = path.Pop();
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
                    continueWalking = false;
                    if(curCell.occupiedBy == null) 
                    {
                        mapManager.UpdateUnitPosition(curCell.position, characterMotor.unitData);
                    }

                    mapManager.TravelEnter(characterMotor.unitData, curCell);

                    if(battleManager.HasInterrupts())
                    {
                        anim.SetBool("Moving", false);
                        processingInterrupt = true;
                        yield return new WaitForSeconds(.4f);
                    }

                    Action<ModBool> checkIfCancelled = (ModBool cancelled) => 
                    {
                        continueWalking = !cancelled.GetCalculated();
                        if(!continueWalking)
                        {
                            path.Clear();
                        }
                        else
                        {
                            anim.SetBool("Moving", true);
                        }
                        processingInterrupt = false;
                    };
                    ModBool cancelledMovement = new ModBool(false);
                    battleManager.ResolveInterrupts(cancelledMovement, checkIfCancelled);
                    yield return null;
                }
                yield return null;
            }
        }
        yield return null;
        mapManager.UpdateUnitPosition(curCell.position, characterMotor.unitData);

        anim.SetBool("Moving", false);
        mapManager.TravelPathEnd();
        isMoving = false;
    }

    void OnUnitTurnEnd()
    {
        mapManager.OnTravelPath -= OnTravelPath;
        mapManager.ClearAllTiles();
    }

    public void UseSkill(BattleSkill battleSkill)
    {
        Debug.Log("isMoving: " + isMoving.ToString());
        // using a skill on our own turn, do a proper animation
        if(!isMoving )
        {
            Vector3 direction = (battleSkill.targets[0].realWorldPosition - transform.position).normalized;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);

            switch(battleSkill.skillData.castAnimation)
            {
                case BattleAnimation.BASIC_ATTACK:
                    anim.SetTrigger("BasicAttack");
                break;
                default:
                    anim.SetTrigger("BasicAttack");
                break;
            }
        }
        else
        {
            battleManager.PreparedSkillConfirm();
        }
    }

    void OnPreparedSkillComplete(BattleSkill battleSkill)
    {
        Action<ModBool> interruptCheck = (ModBool cancelled) => {
            if(!cancelled.GetCalculated() && characterMotor.unitData.aiBrain != null)
            {
                StartCoroutine(DelayEndTurn());
            }
        };

        ModBool cancelExecution = new ModBool(false);
        BattleManager.Instance.ResolveInterrupts(cancelExecution, interruptCheck);
    }

    IEnumerator DelayEndTurn()
    {
        yield return null;
        yield return null;
        yield return new WaitForSeconds(.5f);
        battleManager.turnManager.EndTurn();
    }

    void OnUnitPush(List<GridCell> pushPath, Action callback)
    {
        StartCoroutine(PushCoroutine(pushPath, .2f, callback));
    }

    IEnumerator PushCoroutine(List<GridCell> pushPath, float lerpDuration, Action callback)
    {
        int i = 0;
        float timeElapsed = 0;
        Vector3 startPosition = this.transform.position;
        bool processingInterrupt = false;
        bool continuePushing = true;
        isMoving = true;

        while(i < pushPath.Count || processingInterrupt)
        {
            if(continuePushing && !processingInterrupt)
            {
                GridCell destinationCell = pushPath[i];
                i++;

                while (timeElapsed < lerpDuration)
                {
                    this.transform.position = Vector3.Lerp(startPosition, destinationCell.realWorldPosition, timeElapsed / lerpDuration);
                    timeElapsed += Time.deltaTime;

                    yield return null;
                }

                if(destinationCell.occupiedBy == null)
                {
                    mapManager.UpdateUnitPosition(destinationCell.position, characterMotor.unitData);
                    mapManager.TravelEnter(characterMotor.unitData, destinationCell);
                }
                continuePushing = false;

                mapManager.TravelEnter(characterMotor.unitData, curCell);
                
                if(battleManager.HasInterrupts())
                {
                    processingInterrupt = true;
                }

                Action<ModBool> checkIfCancelled = (ModBool cancelled) => 
                {
                    continuePushing = !cancelled.GetCalculated();
                    if(!continuePushing)
                    {
                        pushPath.Clear();
                    }
                    processingInterrupt = false;
                };
                ModBool cancelledMovement = new ModBool(false);
                battleManager.ResolveInterrupts(cancelledMovement, checkIfCancelled);
                yield return null;
            }
            yield return null;
        } 
        yield return null;

        Debug.Log("Travel complete");
        mapManager.TravelPathEnd();
        isMoving = false;
        Debug.Log("Push complete");
        callback.Invoke();
    }

    public void SnapToGrid()
    {
        GridCell closestCell = mapManager.GetClosestUnoccupiedGridCell(transform.position, characterMotor.unitData);
        if(closestCell != null)
        {
            this.transform.position = closestCell.realWorldPosition;
            curCell = closestCell;
            mapManager.UpdateUnitPosition(closestCell.position, characterMotor.unitData);
        }
        else {
            Debug.Log("uh oh");
        }
    }

    public void OnMouseEnter()
    {
        if(battleManager.curUnit == null || battleManager.curUnit.faction != Faction.ALLY || battleManager.targetLocked)
        {
            return;
        }

        if(battleManager.preparedSkill != null)
        {
            GridCell unitPosition = mapManager.GetCell(characterMotor.unitData.curPosition);
            if(battleManager.preparedSkill.targetableArea.Contains(unitPosition))
            {
                battleManager.PreparedSkillPreviewTarget(unitPosition);
            }
        }
    }

    public void OnMouseExit()
    {
        if(battleManager.curUnit == null || battleManager.curUnit.faction != Faction.ALLY)
        {
            return;
        }
    }

    public void OnMouseDown()
    {
        if(battleManager.curUnit == null || battleManager.curUnit.faction != Faction.ALLY || battleManager.preparedSkill == null || battleManager.targetLocked)
        {
            return;
        }
        
        if(battleManager.targeting)
        {
            battleManager.PreparedSkillSelectTarget(mapManager.GetCell(characterMotor.unitData.curPosition));
        }
    }
}
