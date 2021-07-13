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
    SkillData skill;
    UnitData curUnit;
    Action<SkillData> selectCallback;

    public void SetData(SkillData skillData, UnitData unitData, Action<SkillData> callback, bool interactable = true)
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
            buttonText.text = GetCostString() + " / Action: " + skill.skillName;
            ClearMeta();
            foreach(ElementData element in skill.elements)
            {
                GameObject mGO = Instantiate(elementIconPrefab, skillMeta.transform);
                Image mGOImage = mGO.GetComponent<Image>();
                mGOImage.sprite = element.icon;
                mGOImage.color = element.color;
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
        return skill.spCost.ToString();
    }

    void ClearMeta()
    {
        foreach(Transform child in skillMeta.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }
}
