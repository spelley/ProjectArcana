using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfileUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI unitName;
    [SerializeField] Image portrait;
    [SerializeField] TextMeshProUGUI level;
    [SerializeField] TextMeshProUGUI exp;
    [SerializeField] ResourceBarUI hpBar;
    [SerializeField] ResourceBarUI mpBar;
    [SerializeField] TextMeshProUGUI hpValue;
    [SerializeField] TextMeshProUGUI mpValue;
    [SerializeField] TextMeshProUGUI bodyValue;
    [SerializeField] TextMeshProUGUI mindValue;
    [SerializeField] TextMeshProUGUI spiritValue;
    [SerializeField] TextMeshProUGUI speedValue;
    [SerializeField] TextMeshProUGUI moveValue;
    [SerializeField] TextMeshProUGUI jumpValue;
    [SerializeField] TextMeshProUGUI evadeValue;

    UnitData unitData;
    
    public void SetUnitData(UnitData newUnit)
    {
        unitData = newUnit;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if(unitData == null || unitData.activeJob == null)
        {
            return;
        }

        JobData activeJob = unitData.activeJob;

        int maxHP = unitData.maxHP;
        int maxMP = unitData.maxMP;

        unitName.text = unitData.unitName;
        portrait.sprite = unitData.sprite;
        level.text = "Level: " + unitData.stats.level.ToString();
        exp.text = "Experience: " + unitData.stats.experience.ToString() + "/100";
        hpValue.text = unitData.hp.ToString() + "/" + maxHP.ToString();
        mpValue.text = unitData.mp.ToString() + "/" + maxMP.ToString();
        bodyValue.text = unitData.stats.GetByStat(Stat.BODY).ToString();
        mindValue.text = unitData.stats.GetByStat(Stat.MIND).ToString();
        spiritValue.text = unitData.stats.GetByStat(Stat.SPIRIT).ToString();
        speedValue.text = unitData.stats.GetByStat(Stat.SPEED).ToString();
        moveValue.text = unitData.stats.GetByStat(Stat.MOVE).ToString();
        jumpValue.text = unitData.stats.GetByStat(Stat.JUMP).ToString();
        evadeValue.text = unitData.stats.GetByStat(Stat.EVASION).ToString();

        hpBar.UpdateResource(unitData.hp, maxHP);
        mpBar.UpdateResource(unitData.mp, maxMP);
    }
}