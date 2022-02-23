using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSetter : MonoBehaviour
{
    SkillData skillData;
    UnitData unitData;
    GridCell gridCell;
    BattleManager battleManager;
    MapManager mapManager;

    public void SetTrap(SkillData skill, UnitData source, GridCell cell)
    {
        battleManager = BattleManager.Instance;
        mapManager = MapManager.Instance;

        transform.position = cell.realWorldPosition;

        skillData = skill;
        unitData = source;
        gridCell = cell;

        BindEvents();
    }

    void BindEvents()
    {
        mapManager.OnTravelEnter += OnTravelEnter;
        battleManager.OnEncounterEnd += OnEncounterEnd;
    }

    void UnbindEvents()
    {
        mapManager.OnTravelEnter -= OnTravelEnter;
        battleManager.OnEncounterEnd -= OnEncounterEnd;
    }

    public void OnTravelEnter(UnitData unit, GridCell cell)
    {
        GameObject thisGO = this.gameObject;
        if(cell == gridCell)
        {
            Action<ModBool, Action<ModBool>> interrupt = (ModBool cancelExecution, Action<ModBool> completedCallback) => {
                cancelExecution.baseValue = true;
                // clear the skill
                Action<SkillData> trapSkillClear = null;
                trapSkillClear = (SkillData skill) => {
                    if(skill == skillData)
                    {
                        battleManager.OnSkillClear -= trapSkillClear;
                        battleManager.ResolveInterrupts(cancelExecution, completedCallback);
                        GameObject.Destroy(thisGO);
                    }
                };

                battleManager.OnSkillClear += trapSkillClear;

                List<GridCell> targets = new List<GridCell>();
                targets.Add(gridCell);
                Debug.Log("Trap Setter: Skill Confirm - "+skillData.skillName);
                battleManager.SkillTarget(skillData, unitData, gridCell);
                battleManager.SkillSelectTarget(skillData, gridCell);
                StartCoroutine(SkillConfirmRoutine());
            };

            battleManager.AddInterrupt(interrupt);
        }
    }

    IEnumerator SkillConfirmRoutine()
    {
        yield return new WaitForSeconds(.8f);
        battleManager.SkillConfirm(skillData, unitData, mapManager.GetTargetedCells());
    }

    public void OnEncounterEnd()
    {
        GameObject.Destroy(this.gameObject);
    }

    void OnDestroy()
    {
        UnbindEvents();
    }
}
