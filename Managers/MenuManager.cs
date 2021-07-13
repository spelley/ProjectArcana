using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
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
            ChangeUnit(gameManager.party[unitIndex]);
        }
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

    void ChangeUnit(UnitData newUnit)
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

        UpdateUI();
    }

    void UpdateUI()
    {
        profileUI.SetUnitData(curUnit);
        currentJobUI.SetUnitData(curUnit);
        weaponViewUI.SetUnitData(gameManager.playerInventory, curUnit);
        equipmentInventoryUI.SetInventory(gameManager.playerInventory, curUnit);
        skillListUI.SetData(curUnit);
        jobListUI.SetData(curUnit);
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
