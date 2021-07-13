using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JobListItemUI : MonoBehaviour
{
    [SerializeField]
    Button jobButton;
    [SerializeField]
    TextMeshProUGUI buttonText;
    UnitJob unitJob;
    UnitData curUnit;
    Action<UnitJob> selectCallback;

    public void SetData(UnitJob jobData, UnitData unitData, Action<UnitJob> callback, bool interactable = true)
    {
        unitJob = jobData;
        curUnit = unitData;
        selectCallback = callback;
        UpdateUI(interactable);
    }

    public void OnJobButton()
    {
        selectCallback?.Invoke(unitJob);
    }

    public void UpdateUI(bool interactable = true)
    {
        if(curUnit != null)
        {
            buttonText.text = unitJob.jobData.jobName;
            jobButton.interactable = interactable;
        }
    }
}
