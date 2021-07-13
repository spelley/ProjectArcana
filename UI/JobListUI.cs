using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JobListUI : MonoBehaviour
{
    UnitData curUnit;
    [SerializeField] GameObject jobListItemPrefab;
    [SerializeField] GameObject jobListItemHolder;

    [SerializeField] CurrentJobUI currentJobUI;

    public void SetData(UnitData unitData)
    {
        curUnit = unitData;
        UpdateUI();
    }

    void UpdateUI()
    {
        PopulateJobList();
        currentJobUI.SetUnitData(curUnit);
    }

    void PopulateJobList()
    {
        ClearJobList();

        List<UnitJob> jobsList = curUnit.availableJobs;
        
        foreach(UnitJob unitJob in jobsList)
        {
            GameObject jlGO = Instantiate(jobListItemPrefab, jobListItemHolder.transform);
            JobListItemUI jobListItemUI = jlGO.GetComponent<JobListItemUI>();
            jobListItemUI.SetData(unitJob, curUnit, AssignJob, (unitJob.jobData != curUnit.activeJob));
        }
    }

    public void AssignJob(UnitJob unitJob)
    {
        curUnit.activeJob = unitJob.jobData;
        UpdateUI();
    }

    void ClearJobList()
    {
        foreach(Transform child in jobListItemHolder.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }
}
