using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillListItem : MonoBehaviour
{
    BattleManager battleManager;
    MapManager mapManager;

    [SerializeField]
    Button skillButton;
    [SerializeField]
    TextMeshProUGUI buttonText;
    [SerializeField]
    GameObject skillMeta;
    SkillData skill;
    UnitData unit;
    Action selectCallback;
    bool previewing = false;

    void OnEnable()
    {
        previewing = false;
    }

    void Start()
    {
        battleManager = BattleManager.Instance;
        mapManager = MapManager.Instance;
        skillButton.onClick.AddListener(OnClickSkill);
    }

    public void SetData(SkillData skillData, UnitData unitData, Action callback = null)
    {
        skill = skillData;
        unit = unitData;
        selectCallback = callback;
        UpdateUI();
    }

    void OnClickSkill()
    {
        if(skill != null && unit != null && !previewing)
        {
            battleManager.SkillTarget(skill, unit, mapManager.GetCell(unit.curPosition));
            previewing = true;
            selectCallback?.Invoke();
        }
    }

    void UpdateUI()
    {
        if(skill != null && unit != null)
        {
            buttonText.text = skill.skillName;
        }
    }
}
