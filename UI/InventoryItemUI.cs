using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemUI : MonoBehaviour
{
    InventoryQuantity invItem;
    UnitData curUnit;
    EquipmentInventoryUI equipmentInventoryUI;

    [SerializeField] Button itemButton;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemQuantityText;

    void Start()
    {
        itemButton.onClick.AddListener(OnItemButtonClick);
    }

    public void SetInventoryItem(InventoryQuantity inventoryItem, UnitData unitData, EquipmentInventoryUI inventoryUI)
    {
        invItem = inventoryItem;
        curUnit = unitData;
        equipmentInventoryUI = inventoryUI;

        UpdateUI();
    }

    public void UpdateUI()
    {
        itemName.text = invItem.itemData.itemName;
        itemQuantityText.text = invItem.quantity.ToString() + "/" + (invItem.quantity + invItem.numEquipped).ToString();
        if(invItem.itemData is EquipmentData)
        {
            itemButton.interactable = (invItem.quantity > 0);
        }
    }

    public void OnItemButtonClick()
    {
        EquipmentData equipment = invItem.itemData as EquipmentData;
        if(equipment != null)
        {
            if(invItem.quantity > 0)
            {
                invItem = equipmentInventoryUI.Equip(equipment);
            }
        }
    }
}