using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrentJobUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI jobTitle;
    [SerializeField] TextMeshProUGUI jobDescription;
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
        jobTitle.text = activeJob.jobName;
        jobDescription.text = activeJob.description;
        hpValue.text = activeJob.hpMult.ToString()+'X';
        mpValue.text = activeJob.mpMult.ToString()+'X';
        bodyValue.text = activeJob.bodyMult.ToString()+'X';
        mindValue.text = activeJob.mindMult.ToString()+'X';
        spiritValue.text = activeJob.spiritMult.ToString()+'X';
        speedValue.text = activeJob.speedMult.ToString()+'X';
        moveValue.text = activeJob.baseMove.ToString();
        jumpValue.text = activeJob.baseJump.ToString();
        evadeValue.text = activeJob.baseEvasion.ToString();
    }
}
