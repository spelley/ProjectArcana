using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponViewUI : MonoBehaviour
{
    UnitData curUnit;
    Inventory curInventory;

    GameObject weapon;
    GameObject offhand;
    GameObject helmet;
    GameObject armor;
    GameObject accessory;

    [SerializeField] GameObject equipmentList;
    [SerializeField] GameObject equipmentItemPrefab;

    public void SetUnitData(Inventory inventory, UnitData unitData)
    {
        curInventory = inventory;
        curUnit = unitData;
        UpdateUI();
    }

    public void UpdateSlot(EquipmentSlot slot)
    {
        switch(slot)
        {
            case EquipmentSlot.WEAPON:
                weapon.GetComponent<EquipmentItemUI>().SetEquipment(curUnit.equipmentBlock.weapon, EquipmentSlot.WEAPON, this);
            break;
            case EquipmentSlot.OFFHAND:
                offhand.GetComponent<EquipmentItemUI>().SetEquipment(curUnit.equipmentBlock.offhand, EquipmentSlot.OFFHAND, this);
            break;
            case EquipmentSlot.HELMET:
                helmet.GetComponent<EquipmentItemUI>().SetEquipment(curUnit.equipmentBlock.helmet, EquipmentSlot.HELMET, this);
            break;
            case EquipmentSlot.ARMOR:
                armor.GetComponent<EquipmentItemUI>().SetEquipment(curUnit.equipmentBlock.armor, EquipmentSlot.ARMOR, this);
            break;
            case EquipmentSlot.ACCESSORY:
                accessory.GetComponent<EquipmentItemUI>().SetEquipment(curUnit.equipmentBlock.accessory, EquipmentSlot.ACCESSORY, this);
            break;
        }
    }

    public void UpdateUI()
    {
        ClearList();

        weapon = Instantiate(equipmentItemPrefab, equipmentList.transform);
        weapon.GetComponent<EquipmentItemUI>().SetEquipment(curUnit.equipmentBlock.weapon, EquipmentSlot.WEAPON, this);

        offhand = Instantiate(equipmentItemPrefab, equipmentList.transform);
        offhand.GetComponent<EquipmentItemUI>().SetEquipment(curUnit.equipmentBlock.offhand, EquipmentSlot.OFFHAND, this);

        helmet = Instantiate(equipmentItemPrefab, equipmentList.transform);
        helmet.GetComponent<EquipmentItemUI>().SetEquipment(curUnit.equipmentBlock.helmet, EquipmentSlot.HELMET, this);

        armor = Instantiate(equipmentItemPrefab, equipmentList.transform);
        armor.GetComponent<EquipmentItemUI>().SetEquipment(curUnit.equipmentBlock.armor, EquipmentSlot.ARMOR, this);

        accessory = Instantiate(equipmentItemPrefab, equipmentList.transform);
        accessory.GetComponent<EquipmentItemUI>().SetEquipment(curUnit.equipmentBlock.accessory, EquipmentSlot.ACCESSORY, this);
    }

    public void Unequip(EquipmentData equipment, EquipmentSlot equipmentSlot)
    {
        curInventory.UnequipToInventory(equipment, equipmentSlot, curUnit);
    }

    void ClearList()
    {
        foreach(Transform child in equipmentList.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }
}
