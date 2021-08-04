using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentInventoryUI : MonoBehaviour
{
    GameManager gameManager;
    Inventory curInventory;
    UnitData curUnit;
    EquipmentSlot curSlot;

    Dictionary<string, InventoryItemUI> itemDictionary = new Dictionary<string, InventoryItemUI>();

    [SerializeField] GameObject inventoryList;
    [SerializeField] GameObject inventoryItemPrefab;
    [SerializeField] List<GameObject> inventoryTabs;
    [SerializeField] Sprite activeTabImage;
    [SerializeField] Sprite inactiveTabImage;

    void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.OnGameManagerLoaded += OnGameManagerLoaded;
    }

    void OnDestroy()
    {
        gameManager.OnGameManagerLoaded -= OnGameManagerLoaded;
    }

    void OnGameManagerLoaded()
    {
        if(inventoryTabs.Count == 0)
        {
            return;
        }
    }

    public void SetInventory(Inventory inventory, UnitData unitData)
    {
        curInventory = inventory;
        curUnit = unitData;

        UpdateUI(curSlot);
    }

    public void UpdateUI()
    {
        UpdateUI(curSlot);
    }

    public void UpdateUI(EquipmentSlot slot)
    {
        curSlot = slot;
        List<InventoryQuantity> equipmentQuantities = curInventory.GetEquipmentQuantities(slot);
        ClearList();
        PopulateList(equipmentQuantities);
    }

    public void UpdateItem(InventoryQuantity inventoryQuantity)
    {
        if(itemDictionary.ContainsKey(inventoryQuantity.itemData.id))
        {
            itemDictionary[inventoryQuantity.itemData.id].SetInventoryItem(inventoryQuantity, curUnit, this);
        }
    }

    void PopulateList(List<InventoryQuantity> items)
    {
        foreach(InventoryQuantity invItem in items)
        {
            GameObject itemGO = Instantiate(inventoryItemPrefab, inventoryList.transform);
            InventoryItemUI itemUI = itemGO.GetComponent<InventoryItemUI>();
            itemUI.SetInventoryItem(invItem, gameManager.activePlayer, this);
            itemDictionary.Add(invItem.itemData.id, itemUI);
        }
    }

    public InventoryQuantity Equip(EquipmentData equipment)
    {
        curInventory.EquipFromInventory(equipment, curUnit, curSlot == EquipmentSlot.OFFHAND);
        return curInventory.CheckItem(equipment.id, true);
    }

    void ClearList()
    {
        itemDictionary.Clear();
        foreach(Transform child in inventoryList.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }
}