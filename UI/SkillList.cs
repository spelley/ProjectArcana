using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillList : MonoBehaviour
{
    UnitData unit;
    [SerializeField]
    GameObject skillListItemPrefab;
    [SerializeField]
    GameObject skillListItemHolder;

    public void SetData(UnitData unitData)
    {
        unit = unitData;
        LoadItems();
    }

    void LoadItems()
    {
        Clear();
        List<SkillData> skillItems = unit.GetAvailableSkills();
        foreach(SkillData skillData in skillItems)
        {
            GameObject siGO = Instantiate(skillListItemPrefab, skillListItemHolder.transform);
            siGO.GetComponent<SkillListItem>().SetData(skillData, unit, HideSelf);
        }
    }

    void HideSelf()
    {
        this.gameObject.SetActive(false);
    }

    void Clear()
    {
        foreach(Transform child in skillListItemHolder.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }
}
