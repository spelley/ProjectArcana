using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillListItemUI : MonoBehaviour
{
    [SerializeField]
    Button skillButton;
    [SerializeField]
    TextMeshProUGUI buttonText;
    [SerializeField]
    GameObject skillMeta;
    [SerializeField]
    GameObject elementIconPrefab;
    IAssignableSkill skill;
    UnitData curUnit;
    Action<IAssignableSkill> selectCallback;

    public void SetData(IAssignableSkill skillData, UnitData unitData, Action<IAssignableSkill> callback, bool interactable = true)
    {
        skill = skillData;
        curUnit = unitData;
        selectCallback = callback;
        UpdateUI(interactable);
    }

    public void OnSkillItemButton()
    {
        selectCallback?.Invoke(skill);
    }

    public void UpdateUI(bool interactable = true)
    {
        if(skill != null && curUnit != null)
        {
            buttonText.text = GetCostString() + " " + GetActionTypeText() + " " + skill.skillName;
            ClearMeta();
            if(skill is SkillData)
            {
                foreach(ElementData element in ((SkillData)skill).elements)
                {
                    GameObject mGO = Instantiate(elementIconPrefab, skillMeta.transform);
                    Image mGOImage = mGO.GetComponent<Image>();
                    mGOImage.sprite = element.icon;
                    mGOImage.color = element.color;
                }
            }

            skillButton.interactable = interactable;
        }
    }

    string GetActionTypeText()
    {
        if(skill == null)
        {
            return "[-]";
        }
        if(skill is SkillData)
        {
            return "[S]";
        }
        else
        {
            return "[P]";
        }
    }

    string GetCostString()
    {
        return skill.spCost.ToString();
    }

    void ClearMeta()
    {
        foreach(Transform child in skillMeta.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }
}
