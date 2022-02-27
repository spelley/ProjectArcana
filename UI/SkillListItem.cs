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
    [SerializeField]
    GameObject elementIconPrefab;
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
            battleManager.PreparedSkillInit(skill, unit);
            //battleManager.SkillTarget(skill, unit, mapManager.GetCell(unit.curPosition));
            previewing = true;
            selectCallback?.Invoke();
        }
    }

    void UpdateUI()
    {
        if(skill != null && unit != null)
        {
            buttonText.text = GetActionTypeText() + " " + skill.skillName + GetCostString();
            ClearMeta();
            foreach(ElementData element in skill.elements)
            {
                GameObject mGO = Instantiate(elementIconPrefab, skillMeta.transform);
                Image mGOImage = mGO.GetComponent<Image>();
                mGOImage.sprite = element.icon;
                mGOImage.color = element.color;
            }

            skillButton.interactable = skill.IsUsable(unit);
        }
    }

    string GetActionTypeText()
    {
        if(skill == null)
        {
            return "[-]";
        }
        if(skill.actionType == ActionType.MOVE)
        {
            return "[M]";
        }
        if(skill.actionType == ActionType.STANDARD)
        {
            return "[S]";
        }
        if(skill.actionType == ActionType.BONUS)
        {
            return "[B]";
        }
        return "[F]";
    }

    string GetCostString()
    {
        string output = "";
        if(skill == null)
        {
            return output;
        }
        if(skill.hpCost > 0)
        {
            output += " - HP: -" + skill.hpCost;
            if(skill.mpCost > 0)
            {
                output += ", MP: " + skill.mpCost;
            }
        }
        else if(skill.mpCost > 0)
        {
            output += " - MP: " + skill.mpCost;
        }

        return output;
    }

    void ClearMeta()
    {
        foreach(Transform child in skillMeta.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }
}
