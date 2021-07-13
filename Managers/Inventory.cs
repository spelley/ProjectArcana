using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Inventory
{
    Dictionary<int, InventoryQuantity> _items = new Dictionary<int, InventoryQuantity>();
    Dictionary<int, InventoryQuantity> _equipment = new Dictionary<int, InventoryQuantity>();

    public event Action<InventoryQuantity> OnInventoryChange;

    public void EquipFromInventory(EquipmentData itemData, UnitData unit, bool offhand = false)
    {
        EquipFromInventory(itemData.itemID, unit, offhand);
    }

    public void EquipFromInventory(int itemID, UnitData unit, bool offhand = false)
    {
        if(_equipment.ContainsKey(itemID))
        {
            InventoryQuantity quant = _equipment[itemID];
            if(quant.quantity > 0)
            {
                EquipmentData unequipped = unit.equipmentBlock.Equip(quant.itemData as EquipmentData, unit, offhand);
                MoveFromInventoryToEquip(quant.itemData as EquipmentData);
                if(unequipped != null)
                {
                    MoveFromEquipToInventory(unequipped);
                }
            }
        }
    }

    public void UnequipToInventory(EquipmentData equipment, EquipmentSlot slot, UnitData unit)
    {
        unit.equipmentBlock.Unequip(equipment, unit, (slot == EquipmentSlot.OFFHAND));
        MoveFromEquipToInventory(equipment);
    }

    public List<ItemData> GetEquipment()
    {
        List<ItemData> equipmentList = new List<ItemData>();
        foreach(KeyValuePair<int, InventoryQuantity> item in _items)
        {
            equipmentList.Add(item.Value.itemData as EquipmentData);
        }

        return equipmentList;
    }

    public List<ItemData> GetEquipment(EquipmentSlot slot)
    {
        List<ItemData> equipmentList = new List<ItemData>();
        foreach(KeyValuePair<int, InventoryQuantity> item in _items)
        {
            EquipmentData equip = item.Value.itemData as EquipmentData;
            if(equip.equipmentSlot == slot)
            {
                equipmentList.Add(equip);
            }
        }

        return equipmentList;
    }

    public List<InventoryQuantity> GetEquipmentQuantities()
    {
        List<InventoryQuantity> equipmentList = new List<InventoryQuantity>();
        foreach(KeyValuePair<int, InventoryQuantity> item in _equipment)
        {
            equipmentList.Add(item.Value);
        }

        return equipmentList;
    }

    public List<InventoryQuantity> GetEquipmentQuantities(EquipmentSlot slot)
    {
        List<InventoryQuantity> equipmentList = new List<InventoryQuantity>();
        foreach(KeyValuePair<int, InventoryQuantity> item in _equipment)
        {
            EquipmentData equip = item.Value.itemData as EquipmentData;
            if(equip.equipmentSlot == slot)
            {
                equipmentList.Add(item.Value);
            }
        }

        return equipmentList;
    }

    public InventoryQuantity CheckItem(int itemID, bool equipment = false)
    {
        if(!equipment && _items.ContainsKey(itemID))
        {
            return _items[itemID];
        }
        else if(equipment && _equipment.ContainsKey(itemID))
        {
            return _equipment[itemID];
        }

        return new InventoryQuantity(null, -1, -1);
    }

    public void AddItem(ItemData itemData, bool equipment = false)
    {
        if(!equipment && _items.ContainsKey(itemData.itemID))
        {
            _items[itemData.itemID] = new InventoryQuantity(itemData, 
                                                            _items[itemData.itemID].quantity + 1, 
                                                            _items[itemData.itemID].numEquipped);
            OnInventoryChange?.Invoke(_items[itemData.itemID]);
        }
        else if(equipment && _equipment.ContainsKey(itemData.itemID))
        {
            _equipment[itemData.itemID] = new InventoryQuantity(itemData, 
                                                                _equipment[itemData.itemID].quantity + 1, 
                                                                _equipment[itemData.itemID].numEquipped);
            OnInventoryChange?.Invoke(_equipment[itemData.itemID]);
        }
        else if(!equipment)
        {
            _items.Add(itemData.itemID, new InventoryQuantity(itemData, 1, 0));
            OnInventoryChange?.Invoke(_items[itemData.itemID]);
        }
        else
        {
            _equipment.Add(itemData.itemID, new InventoryQuantity(itemData, 1, 0));
            OnInventoryChange?.Invoke(_equipment[itemData.itemID]);
        }
    }

    public void RemoveItem(ItemData itemData, bool equipment = false)
    {
        if(!equipment && _items.ContainsKey(itemData.itemID))
        {
            InventoryQuantity quant = _items[itemData.itemID];
            if(quant.quantity <= 1 && quant.numEquipped == 0)
            {
                _items.Remove(itemData.itemID);
                OnInventoryChange?.Invoke(_items[itemData.itemID]);
            }
            else
            {
                _items[itemData.itemID] = new InventoryQuantity(itemData, quant.quantity - 1, quant.numEquipped);
            }
        }
        else if(equipment && _equipment.ContainsKey(itemData.itemID))
        {
            InventoryQuantity quant = _equipment[itemData.itemID];
            if(quant.quantity <= 1 && quant.numEquipped == 0)
            {
                _equipment.Remove(itemData.itemID);
                OnInventoryChange?.Invoke(new InventoryQuantity(itemData, 0, 0));
            }
            else
            {
                _equipment[itemData.itemID] = new InventoryQuantity(itemData, quant.quantity - 1, quant.numEquipped);
                OnInventoryChange?.Invoke(_equipment[itemData.itemID]);
            }
        }
    }

    void MoveFromInventoryToEquip(EquipmentData equipmentData)
    {
        if(_equipment.ContainsKey(equipmentData.itemID))
        {
            _equipment[equipmentData.itemID] = new InventoryQuantity(equipmentData, 
                                                                    _equipment[equipmentData.itemID].quantity - 1, 
                                                                    _equipment[equipmentData.itemID].numEquipped + 1);
            OnInventoryChange?.Invoke(_equipment[equipmentData.itemID]);
        }
    }

    void MoveFromEquipToInventory(EquipmentData equipmentData)
    {
        if(_equipment.ContainsKey(equipmentData.itemID))
        {
            _equipment[equipmentData.itemID] = new InventoryQuantity(equipmentData, 
                                                                    _equipment[equipmentData.itemID].quantity + 1, 
                                                                    _equipment[equipmentData.itemID].numEquipped - 1);
            OnInventoryChange?.Invoke(_equipment[equipmentData.itemID]);
        }
    }
}