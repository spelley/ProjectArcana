using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public enum MenuState {
        EQUIPMENT,
        SKILLS,
        JOB
    }
    public MenuState curState {get; private set;}
    [SerializeField] ProfileUI profileUI;
    [SerializeField] CurrentJobUI currentJobUI;
    [SerializeField] WeaponViewUI weaponViewUI;
    [SerializeField] EquipmentInventoryUI equipmentInventoryUI;
    [SerializeField] SkillListUI skillListUI;
    [SerializeField] JobListUI jobListUI;
    UnitData curUnit;
    int unitIndex = 0;
    GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.OnGameManagerLoaded += OnGameManagerLoaded;
    }

    void OnGameManagerLoaded()
    {
        if(gameManager.party.Count > 0 && gameManager.party[unitIndex] != null)
        {
            ChangeUnit(gameManager.party[unitIndex], false);
            SetState(MenuState.EQUIPMENT);
        }
    }

    public void SetState(MenuState newState)
    {
        switch(newState)
        {
            case MenuState.EQUIPMENT:
                profileUI.gameObject.SetActive(true);
                currentJobUI.gameObject.SetActive(false);
                weaponViewUI.gameObject.SetActive(true);
                equipmentInventoryUI.gameObject.SetActive(true);
                skillListUI.gameObject.SetActive(false);
                jobListUI.gameObject.SetActive(false);
            break;
            case MenuState.SKILLS:
                profileUI.gameObject.SetActive(true);
                currentJobUI.gameObject.SetActive(false);
                weaponViewUI.gameObject.SetActive(false);
                equipmentInventoryUI.gameObject.SetActive(false);
                skillListUI.gameObject.SetActive(true);
                jobListUI.gameObject.SetActive(false);
            break;
            case MenuState.JOB:
                profileUI.gameObject.SetActive(true);
                currentJobUI.gameObject.SetActive(true);
                weaponViewUI.gameObject.SetActive(false);
                equipmentInventoryUI.gameObject.SetActive(false);
                skillListUI.gameObject.SetActive(false);
                jobListUI.gameObject.SetActive(true);
            break;
        }
        curState = newState;
        UpdateUI();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(unitIndex == (gameManager.party.Count - 1))
            {
                unitIndex = 0;
            }
            else
            {
                unitIndex++;
            }

            ChangeUnit(gameManager.party[unitIndex]);
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(unitIndex == 0)
            {
                unitIndex = gameManager.party.Count - 1;
            }
            else
            {
                unitIndex--;
            }

            ChangeUnit(gameManager.party[unitIndex]);
        }
    }

    void ChangeUnit(UnitData newUnit, bool updateUI = true)
    {
        if(curUnit != null)
        {
            curUnit.equipmentBlock.OnEquip -= OnEquipChange;
            curUnit.equipmentBlock.OnUnequip -= OnEquipChange;
            gameManager.playerInventory.OnInventoryChange -= OnInventoryChange;
        }
        curUnit = newUnit;

        curUnit.equipmentBlock.OnEquip += OnEquipChange;
        curUnit.equipmentBlock.OnUnequip += OnEquipChange;
        gameManager.playerInventory.OnInventoryChange += OnInventoryChange;

        if(updateUI)
        {
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        profileUI.SetUnitData(curUnit);

        switch(curState)
        {
            case MenuState.EQUIPMENT:
                weaponViewUI.SetUnitData(gameManager.playerInventory, curUnit);
                equipmentInventoryUI.SetInventory(gameManager.playerInventory, curUnit);
            break;
            case MenuState.SKILLS:
                skillListUI.SetData(curUnit);
            break;
            case MenuState.JOB:
                currentJobUI.SetUnitData(curUnit);
                jobListUI.SetData(curUnit);
            break;
        }
    }

    public void OnEquipChange(EquipmentData equipment, UnitData unitData, bool offhand = false)
    {
        if(equipment.equipmentSlot == EquipmentSlot.WEAPON)
        {
            if(offhand)
            {
                weaponViewUI.UpdateSlot(EquipmentSlot.OFFHAND);
            }
            else
            {
                weaponViewUI.UpdateSlot(EquipmentSlot.WEAPON);
            }
        }
        else
        {
            weaponViewUI.UpdateSlot(equipment.equipmentSlot);
        }
        profileUI.SetUnitData(curUnit);
    }

    public void OnInventoryChange(InventoryQuantity invItem)
    {
        equipmentInventoryUI.UpdateItem(invItem);
    }

    void OnDestroy()
    {
        gameManager.OnGameManagerLoaded -= OnGameManagerLoaded;
        if(curUnit != null)
        {
            curUnit.equipmentBlock.OnEquip -= OnEquipChange;
            curUnit.equipmentBlock.OnUnequip -= OnEquipChange;
            gameManager.playerInventory.OnInventoryChange -= OnInventoryChange;
        }
    }
}
