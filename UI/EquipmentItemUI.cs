using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentItemUI : MonoBehaviour
{
    EquipmentData curEquipment;
    EquipmentSlot equipmentSlot = EquipmentSlot.WEAPON;
    WeaponViewUI equipmentViewUI;

    [Header("Icons For Equipment")]
    [SerializeField] Sprite weaponIcon;
    [SerializeField] Sprite offhandIcon;
    [SerializeField] Sprite helmetIcon;
    [SerializeField] Sprite armorIcon;
    [SerializeField] Sprite accessoryIcon;

    [Header("Item Display")]
    [SerializeField] Button unequipButton;
    [SerializeField] Image equipmentIcon;
    [SerializeField] TextMeshProUGUI equipmentName;
    [SerializeField] GameObject skillButton;

    public void SetEquipment(EquipmentData equip, EquipmentSlot slot, WeaponViewUI equipmentUI)
    {
        curEquipment = equip;
        equipmentSlot = slot;
        equipmentViewUI = equipmentUI;
        UpdateUI();
    }

    public void UpdateUI()
    {
        switch(equipmentSlot)
        {
            case EquipmentSlot.WEAPON:
                equipmentIcon.sprite = weaponIcon;
            break;
            case EquipmentSlot.OFFHAND:
                equipmentIcon.sprite = offhandIcon;
            break;
            case EquipmentSlot.HELMET:
                equipmentIcon.sprite = helmetIcon;
            break;
            case EquipmentSlot.ARMOR:
                equipmentIcon.sprite = armorIcon;
            break;
            case EquipmentSlot.ACCESSORY:
                equipmentIcon.sprite = accessoryIcon;
            break;
        }

        if(curEquipment == null)
        {
            equipmentName.text = "(Empty)";
            unequipButton.interactable = false;
            skillButton.SetActive(false);
            return;
        }
        unequipButton.interactable = true;
        equipmentName.text = curEquipment.itemName;
    }

    public void OnUnequipButtonClick()
    {
        equipmentViewUI.Unequip(curEquipment, equipmentSlot);
    }
}