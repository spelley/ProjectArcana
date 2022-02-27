using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSetter : MonoBehaviour
{
    BattleSkill battleSkill;
    SkillData skillData;
    UnitData unitData;
    GridCell gridCell;
    BattleManager battleManager;
    MapManager mapManager;

    public void SetTrap(BattleSkill battleSkill, GridCell cell, Action callback)
    {
        battleManager = BattleManager.Instance;
        mapManager = MapManager.Instance;

        transform.position = cell.realWorldPosition;

        this.battleSkill = battleSkill;
        skillData = battleSkill.skill;
        unitData = battleSkill.source;
        gridCell = cell;

        BindEvents();
        callback.Invoke();
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
            Action<ModBool, Action<ModBool>> interrupt = (ModBool cancelExecution, Action<ModBool> completedCallback) => 
            {
                cancelExecution.baseValue = true;
                // clear the skill
                Action<BattleSkill> trapSkillClear = null;
                trapSkillClear = (BattleSkill clearedBattleSkill) =>
                { 
                    if(battleSkill == clearedBattleSkill)
                    {
                        battleManager.OnSkillClear -= trapSkillClear;
                        battleManager.ResolveInterrupts(cancelExecution, completedCallback);
                        GameObject.Destroy(thisGO);
                    }
                };

                battleManager.OnSkillClear += trapSkillClear;

                // battleManager.SkillTarget(skillData, unitData, gridCell);
                // battleManager.SkillSelectTarget(skillData, gridCell, true);
                StartCoroutine(SkillConfirmRoutine());
            };

            battleManager.AddInterrupt(interrupt);
        }
    }

    IEnumerator SkillConfirmRoutine()
    {
        yield return new WaitForSeconds(.8f);
        battleManager.SkillConfirm(battleSkill);
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
