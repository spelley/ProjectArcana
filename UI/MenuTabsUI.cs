
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuTabsUI : MonoBehaviour
{
    [SerializeField] MenuManager menuManager;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            switch(menuManager.curState)
            {
                case MenuManager.MenuState.EQUIPMENT:
                    OnSkillsTab();
                break;
                case MenuManager.MenuState.SKILLS:
                    OnJobTab();
                break;
                case MenuManager.MenuState.JOB:
                    OnEquipmentTab();
                break;
            }
        }
        else if(Input.GetKeyDown(KeyCode.Q))
        {
            switch(menuManager.curState)
            {
                case MenuManager.MenuState.EQUIPMENT:
                    OnJobTab();
                break;
                case MenuManager.MenuState.SKILLS:
                    OnEquipmentTab();
                break;
                case MenuManager.MenuState.JOB:
                    OnSkillsTab();
                break;
            }
        }
    }

    public void OnEquipmentTab()
    {
        menuManager.SetState(MenuManager.MenuState.EQUIPMENT);
    }

    public void OnSkillsTab()
    {
        menuManager.SetState(MenuManager.MenuState.SKILLS);
    }

    public void OnJobTab()
    {
        menuManager.SetState(MenuManager.MenuState.JOB);
    }
}