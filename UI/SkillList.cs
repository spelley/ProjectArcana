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

    public void SetData(UnitData unitData, List<SkillData> skillItems)
    {
        unit = unitData;
        LoadItems(skillItems);
    }

    void LoadItems(List<SkillData> skillItems)
    {
        Clear();
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
