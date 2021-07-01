using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitBattleUI : MonoBehaviour
{
    BattleManager battleManager;
    enum UnitUIState
    {
        HIDDEN,
        UNIT,
        UNIT_WITH_COMMANDS,
        SKILL_LIST,
        SKILL_TARGET,
        PREVIEW_SKILL
    }

    UnitUIState curState = UnitUIState.HIDDEN;

    [SerializeField]
    GameObject unitUI;
    [SerializeField]
    GameObject unitCommandsUI;
    [SerializeField] 
    TextMeshProUGUI divinationCommandText;
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
    [SerializeField]
    GameObject previewWindow;
    [SerializeField]
    Button skillConfirmButton;
    [SerializeField]
    TextMeshProUGUI previewWindowText;
    [SerializeField]
    GameObject targetUI;
    [SerializeField]
    Image targetPortrait;
    [SerializeField]
    ResourceBarUI targetHPUI;
    [SerializeField]
    ResourceBarUI targetMPUI;
    [SerializeField]
    ResourceBarUI targetCTUI;

    UnitData curUnit;

    void Awake()
    {
        SetState(UnitUIState.HIDDEN);
    }

    // Start is called before the first frame update
    void Start()
    {
        battleManager = BattleManager.Instance;
        battleManager.OnEncounterStart += OnEncounterStart;
        battleManager.OnEncounterEnd += OnEncounterEnd;
        battleManager.OnSkillPreview += OnSkillPreview;
        battleManager.OnSkillPreviewCancel += OnSkillPreviewCancel;
        battleManager.OnSkillTargetCancel += OnSkillTargetCancel;
        battleManager.OnSkillConfirm += OnSkillConfirm;
        battleManager.OnSkillClear += OnSkillClear;
    }

    void OnDestroy()
    {
        battleManager.OnEncounterStart -= OnEncounterStart;
        battleManager.OnEncounterEnd -= OnEncounterEnd;
        battleManager.OnSkillPreview -= OnSkillPreview;
        battleManager.OnSkillPreviewCancel -= OnSkillPreviewCancel;
        battleManager.OnSkillTargetCancel -= OnSkillTargetCancel;
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
        // close the skill window if it is open
        if(Input.GetKeyDown(KeyCode.Escape) && curUnit != null && curUnit.isPlayerControlled)
        {
            Debug.Log(curState);
            if(curState == UnitUIState.SKILL_LIST)
            {
                SetState(UnitUIState.UNIT_WITH_COMMANDS);
            }
            else if(curState == UnitUIState.PREVIEW_SKILL)
            {
                battleManager.SkillPreviewCancel();
            }
        }
    }

    void SetState(UnitUIState newState)
    {
        switch(newState)
        {
            case UnitUIState.UNIT:
                unitUI.SetActive(true);
                unitCommandsUI.SetActive(false);
                skillsWindow.SetActive(false);
                previewWindow.SetActive(false);
                targetUI.SetActive(false);
            break;
            case UnitUIState.UNIT_WITH_COMMANDS:
                unitUI.SetActive(true);
                unitCommandsUI.SetActive(true);
                skillsWindow.SetActive(false);
                previewWindow.SetActive(false);
                targetUI.SetActive(false);
            break;
            case UnitUIState.SKILL_LIST:
                unitUI.SetActive(true);
                unitCommandsUI.SetActive(true);
                skillsWindow.SetActive(true);
                previewWindow.SetActive(false);
                targetUI.SetActive(false);
            break;
            case UnitUIState.SKILL_TARGET:
                unitUI.SetActive(true);
                unitCommandsUI.SetActive(false);
                skillsWindow.SetActive(false);
                previewWindow.SetActive(false);
                targetUI.SetActive(false);
            break;
            case UnitUIState.PREVIEW_SKILL:
                unitUI.SetActive(true);
                unitCommandsUI.SetActive(false);
                skillsWindow.SetActive(false);
                previewWindow.SetActive(true);
                targetUI.SetActive(true);
            break;
            case UnitUIState.HIDDEN:
                unitUI.SetActive(false);
                unitCommandsUI.SetActive(false);
                skillsWindow.SetActive(false);
                previewWindow.SetActive(false);
                targetUI.SetActive(false);
            break;
        }

        curState = newState;
    }

    void OnEncounterStart()
    {
        if(battleManager.turnManager != null)
        {
            battleManager.turnManager.OnTurnStart += OnTurnStart;
            battleManager.turnManager.OnTurnEnd += OnTurnEnd;
        }
    }

    void OnSkillTargetCancel(SkillData skillData, UnitData unitData)
    {
        SetState(UnitUIState.SKILL_LIST);
    }

    void OnSkillPreview(SkillData skillData, UnitData unitData, List<GridCell> targets)
    {
        SetState(UnitUIState.PREVIEW_SKILL);
        GridCell firstTarget = null;
        foreach(GridCell gridCell in targets)
        {
            if(gridCell.occupiedBy != null)
            {
                firstTarget = gridCell;
            }
        }
        SkillPreview skillPreview = skillData.GetPreview(unitData, firstTarget);
        if(firstTarget != null)
        {
            UpdateTargetWindow(firstTarget.occupiedBy);
        }
        previewWindowText.text = skillPreview.text + " (" + skillPreview.hitChance.ToString() + "%)";
        skillConfirmButton.onClick.AddListener(OnSkillConfirmButton);
    }

    void OnSkillConfirmButton()
    {
        skillConfirmButton.onClick.RemoveListener(OnSkillConfirmButton);
        battleManager.SkillConfirm(battleManager.curSkill, battleManager.curUnit, battleManager.curTargets);
    }

    void OnSkillPreviewCancel()
    {
        SetState(UnitUIState.SKILL_TARGET);
    }

    void OnSkillConfirm(SkillData skillData, UnitData unitData, List<GridCell> targets)
    {
        SetState(UnitUIState.HIDDEN);
    }

    void OnSkillClear(SkillData skillData)
    {
        if(curUnit != null)
        {
            UpdateResources();
            if(curUnit.isPlayerControlled)
            {
                SetState(UnitUIState.UNIT_WITH_COMMANDS);
            }
            else
            {
                SetState(UnitUIState.UNIT);
            }
        }
    }

    public void OnMoveButton()
    {
        if(curUnit != null && curUnit.canMove)
        {
            MapManager.Instance.LoadWalkabilityZone(curUnit.unitGO, curUnit.unitGO.transform.position);
            SetState(UnitUIState.HIDDEN);
        }
    }

    public void OnSkillButton()
    {
        if(curUnit != null && curUnit.canAct)
        {
            SetState(UnitUIState.SKILL_LIST);
            skillsWindow.GetComponent<SkillList>().SetData(curUnit, curUnit.GetAvailableSkills());
        }
    }

    public void OnAttackButton()
    {
        if(curUnit != null && curUnit.canAct)
        {
            SetState(UnitUIState.SKILL_LIST);
            skillsWindow.GetComponent<SkillList>().SetData(curUnit, curUnit.GetAttackSkills());
        }
    }

    public void OnDivinationButton()
    {
        if(curUnit != null && curUnit.canUseBonus)
        {
            SetState(UnitUIState.UNIT_WITH_COMMANDS);
            battleManager.DivinationTarget(curUnit.divinationSkill);
        }
    }

    public void OnSkillWindowClose()
    {
        SetState(UnitUIState.UNIT_WITH_COMMANDS);
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
            SetState(UnitUIState.HIDDEN);
            return;
        }

        if(unitData.isPlayerControlled)
        {
            SetState(UnitUIState.UNIT_WITH_COMMANDS);
        }
        else
        {
            SetState(UnitUIState.UNIT);
        }

        curUnit = unitData;
        portrait.sprite = unitData.sprite;
        divinationCommandText.text = unitData.divinationSkill.divinationName;
        UpdateResources();
        MapManager.Instance.OnTravelPathEnd += OnTravelPathEnd;
    }

    void OnTurnEnd(ITurnTaker turnTaker)
    {
        curUnit = null;
        SetState(UnitUIState.HIDDEN);
    }

    void OnTravelPathEnd()
    {
        if(curUnit != null && curUnit.isPlayerControlled)
        {
           SetState(UnitUIState.UNIT_WITH_COMMANDS);
        }
    }

    void OnEncounterEnd()
    {
        SetState(UnitUIState.HIDDEN);

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

    void UpdateTargetWindow(UnitData target)
    {
        targetPortrait.sprite = target.sprite;
        target.RecalculateResources();
        targetHPUI.UpdateResource(target.hp, target.maxHP);
        targetMPUI.UpdateResource(target.mp, target.maxMP);
        targetCTUI.UpdateResource(100, 100);
    }
}
