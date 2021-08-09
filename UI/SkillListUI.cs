using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillListUI : MonoBehaviour
{
    UnitData curUnit;
    List<SkillData> jobSkills = new List<SkillData>();
    List<PassiveData> jobPassives = new List<PassiveData>();
    [SerializeField] GameObject skillListItemPrefab;
    [SerializeField] GameObject learnedSkillListItemHolder;
    [SerializeField] GameObject assignedSkillListItemHolder;
    [SerializeField] GameObject jobSkillListItemHolder;
    [SerializeField] TextMeshProUGUI spRemainingText;

    public void SetData(UnitData unitData)
    {
        curUnit = unitData;
        jobSkills = curUnit.GetJobSkills();
        jobPassives = curUnit.GetJobPassives();
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

        List<IAssignableSkill> learned = curUnit.GetAllLearnedSkills();
        List<IAssignableSkill> assigned = curUnit.GetAllAssignedSkills();

        foreach(IAssignableSkill skillData in learned)
        {
            GameObject siGO = Instantiate(skillListItemPrefab, learnedSkillListItemHolder.transform);
            SkillListItemUI skillListItemUI = siGO.GetComponent<SkillListItemUI>();
            bool canAssign = (skillData.spCost <= (curUnit.stats.maxSP - curUnit.stats.sp)) 
                                && !assigned.Contains(skillData);
            if(canAssign)
            {
                if(skillData is SkillData && jobSkills.Contains((SkillData)skillData))
                {
                    canAssign = false;
                }
                else if(skillData is PassiveData && jobPassives.Contains((PassiveData)skillData))
                {
                    canAssign = false;
                }
            }
            skillListItemUI.SetData(skillData, curUnit, AssignSkill, canAssign);
        }

        spRemainingText.text = "SP Used: "+curUnit.stats.sp.ToString() + "/" + curUnit.stats.maxSP;
    }

    void PopulateAssignedList()
    {
        ClearAssignedList();
        List<IAssignableSkill> assigned = curUnit.GetAllAssignedSkills();
        foreach(IAssignableSkill skillData in assigned)
        {
            GameObject siGO = Instantiate(skillListItemPrefab, assignedSkillListItemHolder.transform);
            SkillListItemUI skillListItemUI = siGO.GetComponent<SkillListItemUI>();
            skillListItemUI.SetData(skillData, curUnit, UnassignSkill, true);
        }
    }

    void PopulateJobSkillList()
    {
        ClearJobSkillList();

        foreach(PassiveData passiveData in jobPassives)
        {
            GameObject siGO = Instantiate(skillListItemPrefab, jobSkillListItemHolder.transform);
            SkillListItemUI skillListItemUI = siGO.GetComponent<SkillListItemUI>();
            skillListItemUI.SetData(passiveData, curUnit, null, false);
        }
        
        foreach(SkillData skillData in jobSkills)
        {
            GameObject siGO = Instantiate(skillListItemPrefab, jobSkillListItemHolder.transform);
            SkillListItemUI skillListItemUI = siGO.GetComponent<SkillListItemUI>();
            skillListItemUI.SetData(skillData, curUnit, null, false);
        }
    }

    public void AssignSkill(IAssignableSkill skillData)
    {
        if(curUnit.AssignSkill(skillData))
        {
            UpdateUI();
        }
    }

    public void UnassignSkill(IAssignableSkill skillData)
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
