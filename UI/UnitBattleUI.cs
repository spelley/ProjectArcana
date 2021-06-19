using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitBattleUI : MonoBehaviour
{
    BattleManager battleManager;

    [SerializeField]
    GameObject unitUI;
    [SerializeField]
    GameObject unitCommandsUI;
    [SerializeField]
    Image portrait;
    [SerializeField]
    ResourceBarUI hpUI;
    [SerializeField]
    ResourceBarUI mpUI;
    [SerializeField]
    ResourceBarUI ctUI;
    [SerializeField]
    GameObject skillsWindow;

    UnitData curUnit;

    bool hidingUI = false;

    void Awake()
    {
        unitUI.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        battleManager = BattleManager.Instance;
        battleManager.OnEncounterStart += OnEncounterStart;
        battleManager.OnEncounterEnd += OnEncounterEnd;
        battleManager.OnSkillConfirm += OnSkillConfirm;
        battleManager.OnSkillClear += OnSkillClear;
    }

    void OnDestroy()
    {
        battleManager.OnEncounterStart -= OnEncounterStart;
        battleManager.OnEncounterEnd -= OnEncounterEnd;
        battleManager.OnSkillConfirm -= OnSkillConfirm;
        battleManager.OnSkillClear -= OnSkillClear;
        if(battleManager.turnManager != null)
        {
            battleManager.turnManager.OnTurnStart -= OnTurnStart;
            battleManager.turnManager.OnTurnEnd -= OnTurnEnd;
        }
    }

    void Update()
    {
        if(hidingUI && Input.GetKeyDown(KeyCode.Escape))
        {
            unitUI.SetActive(true);
            MapManager.Instance.ClearAllTiles();
            hidingUI = false;
        }
    }

    void OnEncounterStart()
    {
        if(battleManager.turnManager != null)
        {
            battleManager.turnManager.OnTurnStart += OnTurnStart;
            battleManager.turnManager.OnTurnEnd += OnTurnEnd;
        }
    }

    void OnSkillConfirm(SkillData skillData, UnitData unitData, List<GridCell> targets)
    {
        unitUI.SetActive(false);
    }

    void OnSkillClear(SkillData skillData)
    {
        if(curUnit != null && curUnit.isPlayerControlled)
        {
            unitUI.SetActive(true);
            UpdateResources();
        }
    }

    public void OnMoveButton()
    {
        if(curUnit != null && curUnit.canMove)
        {
            MapManager.Instance.LoadWalkabilityZone(curUnit.unitGO, curUnit.unitGO.transform.position);
            unitUI.SetActive(false);
            hidingUI = true;
        }
    }

    public void OnSkillButton()
    {
        if(curUnit != null && curUnit.canAct)
        {
            skillsWindow.SetActive(true);
            skillsWindow.GetComponent<SkillList>().SetData(curUnit);
        }
    }

    public void OnSkillWindowClose()
    {
        skillsWindow.SetActive(false);
    }

    public void OnEndTurn()
    {
        if(battleManager.turnManager != null)
        {
            battleManager.turnManager.EndTurn();
        }
    }

    void OnTurnStart(ITurnTaker turnTaker)
    {
        UnitData unitData = turnTaker as UnitData;

        if(unitData == null || (!unitData.canAct && !unitData.canMove))
        {
            unitUI.SetActive(false);
            return;
        }

        curUnit = unitData;
        unitUI.SetActive(true);
        portrait.sprite = unitData.sprite;
        UpdateResources();
        MapManager.Instance.OnTravelPathEnd += OnTravelPathEnd;

        if(unitData.isPlayerControlled)
        {
            unitCommandsUI.SetActive(true);
        }
        else
        {
            unitCommandsUI.SetActive(false);
        }
    }

    void OnTurnEnd(ITurnTaker turnTaker)
    {
        curUnit = null;
        unitUI.SetActive(false);
    }

    void OnTravelPathEnd()
    {
        if(curUnit != null && curUnit.isPlayerControlled)
        {
            unitUI.SetActive(true);
        }
    }

    void OnEncounterEnd()
    {
        if(battleManager.turnManager != null)
        {
            battleManager.turnManager.OnTurnStart -= OnTurnStart;
        }
    }

    void UpdateResources()
    {
        curUnit.RecalculateResources();
        hpUI.UpdateResource(curUnit.hp, curUnit.maxHP);
        mpUI.UpdateResource(curUnit.mp, curUnit.maxMP);
        ctUI.UpdateResource(100, 100);
    }
}
