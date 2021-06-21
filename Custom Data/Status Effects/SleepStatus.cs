using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sleep", menuName = "Custom Data/Status Effect/Sleep")]
public class SleepStatus : StatusEffect
{
    [SerializeField]
    int sleepTimer = 2;
    [SerializeField]
    int curTimer = 0;
    UnitData appliedTo;
    
    public override void Apply(UnitData unitData)
    {
        appliedTo = unitData;
        unitData.OnCalculateCanAct += OnMoveAct;
        unitData.OnCalculateCanMove += OnMoveAct;
        if(BattleManager.Instance != null)
        {
            BattleManager.Instance.OnEncounterStart += OnEncounterStart;
            BattleManager.Instance.OnEncounterEnd += OnEncounterEnd;
        }
    }

    public void OnMoveAct(ModBool modBool)
    {
        // while we are asleep, we can't move or act
        modBool.baseValue = false;
    }

    public void OnEncounterStart()
    {
        BattleManager.Instance.turnManager.OnTurnStart += OnTurnStart;
    }

    public void OnTurnStart(ITurnTaker turnTaker)
    {
        UnitData unitData = turnTaker as UnitData;
        if(unitData == null)
        {
            return;
        }

        if(unitData != appliedTo)
        {
            return;
        }

        if(curTimer < sleepTimer)
        {
            curTimer++;
            // TODO: play animation
            appliedTo.unitGO.GetComponent<CharacterUI>().ShowPopUp("ZZz...", new Color32(255, 255, 255, 200));
            GameManager.Instance.StartCoroutine(EndTurn());
            return;
        }
        unitData.RemoveStatus(this); // expire
    }

    IEnumerator EndTurn()
    {
        yield return new WaitForSeconds(1f);
        BattleManager.Instance.turnManager.EndTurn();
    }

    public void OnEncounterEnd()
    {
        if(BattleManager.Instance.turnManager != null)
        {
            BattleManager.Instance.turnManager.OnTurnStart -= OnTurnStart;
        }
    }

    public override void Remove(UnitData unitData)
    {
        unitData.OnCalculateCanAct -= OnMoveAct;
        unitData.OnCalculateCanMove -= OnMoveAct;
        BattleManager.Instance.OnEncounterStart -= OnEncounterStart;
        BattleManager.Instance.OnEncounterEnd -= OnEncounterEnd;
        if(BattleManager.Instance.turnManager != null)
        {
            BattleManager.Instance.turnManager.OnTurnStart -= OnTurnStart;
        }
    }
}