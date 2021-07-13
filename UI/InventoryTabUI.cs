using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryTabUI : MonoBehaviour
{
    [SerializeField] Button tabButton;
    [SerializeField] EquipmentSlot slot;
    [SerializeField] EquipmentInventoryUI equipmentInventoryUI;

    void Start()
    {
        tabButton.onClick.AddListener(OnTabButton);
    }

    void OnTabButton()
    {
        equipmentInventoryUI.UpdateUI(slot);
    }
}