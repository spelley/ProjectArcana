using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillListUI : MonoBehaviour
{
    UnitData curUnit;
    List<SkillData> jobSkills = new List<SkillData>();
    [SerializeField] GameObject skillListItemPrefab;
    [SerializeField] GameObject learnedSkillListItemHolder;
    [SerializeField] GameObject assignedSkillListItemHolder;
    [SerializeField] GameObject jobSkillListItemHolder;
    [SerializeField] TextMeshProUGUI spRemainingText;

    public void SetData(UnitData unitData)
    {
        curUnit = unitData;
        jobSkills = curUnit.GetJobSkills();
        UpdateUI();
        PopulateJobSkillList();
    }

    void UpdateUI()
    {
        ClearLearnedList();
        ClearAssignedList();

        PopulateLearnedList();
        PopulateAssignedList();
    }

    void PopulateLearnedList()
    {
        ClearLearnedList();
        List<SkillData> skillItems = curUnit.GetLearnedSkills();
        List<SkillData> assignedSkills = curUnit.GetAssignedSkills();
        foreach(SkillData skillData in skillItems)
        {
            GameObject siGO = Instantiate(skillListItemPrefab, learnedSkillListItemHolder.transform);
            SkillListItemUI skillListItemUI = siGO.GetComponent<SkillListItemUI>();
            bool canAssign = (skillData.spCost <= (curUnit.stats.maxSP - curUnit.stats.sp)) 
                                && !assignedSkills.Contains(skillData) 
                                && !jobSkills.Contains(skillData);
            skillListItemUI.SetData(skillData, curUnit, AssignSkill, canAssign);
        }

        spRemainingText.text = "SP Used: "+curUnit.stats.sp.ToString() + "/" + curUnit.stats.maxSP;
    }

    void PopulateAssignedList()
    {
        ClearAssignedList();
        List<SkillData> skillItems = curUnit.GetAssignedSkills();
        foreach(SkillData skillData in skillItems)
        {
            GameObject siGO = Instantiate(skillListItemPrefab, assignedSkillListItemHolder.transform);
            SkillListItemUI skillListItemUI = siGO.GetComponent<SkillListItemUI>();
            skillListItemUI.SetData(skillData, curUnit, UnassignSkill, true);
        }
    }

    void PopulateJobSkillList()
    {
        ClearJobSkillList();
        
        foreach(SkillData skillData in jobSkills)
        {
            GameObject siGO = Instantiate(skillListItemPrefab, jobSkillListItemHolder.transform);
            SkillListItemUI skillListItemUI = siGO.GetComponent<SkillListItemUI>();
            skillListItemUI.SetData(skillData, curUnit, null, false);
        }
    }

    public void AssignSkill(SkillData skillData)
    {
        if(curUnit.AssignSkill(skillData))
        {
            UpdateUI();
        }
    }

    public void UnassignSkill(SkillData skillData)
    {
        curUnit.UnassignSkill(skillData);
        UpdateUI();
    }

    void ClearLearnedList()
    {
        foreach(Transform child in learnedSkillListItemHolder.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

    void ClearAssignedList()
    {
        foreach(Transform child in assignedSkillListItemHolder.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

    void ClearJobSkillList()
    {
        foreach(Transform child in jobSkillListItemHolder.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }
}
