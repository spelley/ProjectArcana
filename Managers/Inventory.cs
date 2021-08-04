using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Inventory
{
    Dictionary<string, InventoryQuantity> _items = new Dictionary<string, InventoryQuantity>();
    Dictionary<string, InventoryQuantity> _equipment = new Dictionary<string, InventoryQuantity>();

    public event Action<InventoryQuantity> OnInventoryChange;

    public void EquipFromInventory(EquipmentData itemData, UnitData unit, bool offhand = false)
    {
        EquipFromInventory(itemData.id, unit, offhand);
    }

    public void EquipFromInventory(string id, UnitData unit, bool offhand = false)
    {
        if(_equipment.ContainsKey(id))
        {
            InventoryQuantity quant = _equipment[id];
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
        foreach(KeyValuePair<string, InventoryQuantity> item in _items)
        {
            equipmentList.Add(item.Value.itemData as EquipmentData);
        }

        return equipmentList;
    }

    public List<ItemData> GetEquipment(EquipmentSlot slot)
    {
        List<ItemData> equipmentList = new List<ItemData>();
        foreach(KeyValuePair<string, InventoryQuantity> item in _items)
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
        foreach(KeyValuePair<string, InventoryQuantity> item in _equipment)
        {
            equipmentList.Add(item.Value);
        }

        return equipmentList;
    }

    public List<InventoryQuantity> GetEquipmentQuantities(EquipmentSlot slot)
    {
        List<InventoryQuantity> equipmentList = new List<InventoryQuantity>();
        foreach(KeyValuePair<string, InventoryQuantity> item in _equipment)
        {
            EquipmentData equip = item.Value.itemData as EquipmentData;
            if(equip.equipmentSlot == slot)
            {
                equipmentList.Add(item.Value);
            }
        }

        return equipmentList;
    }

    public InventoryQuantity CheckItem(string id, bool equipment = false)
    {
        if(!equipment && _items.ContainsKey(id))
        {
            return _items[id];
        }
        else if(equipment && _equipment.ContainsKey(id))
        {
            return _equipment[id];
        }

        return new InventoryQuantity(null, -1, -1);
    }

    public void AddItem(ItemData itemData, bool equipment = false)
    {
        if(!equipment && _items.ContainsKey(itemData.id))
        {
            _items[itemData.id] = new InventoryQuantity(itemData, 
                                                            _items[itemData.id].quantity + 1, 
                                                            _items[itemData.id].numEquipped);
            OnInventoryChange?.Invoke(_items[itemData.id]);
        }
        else if(equipment && _equipment.ContainsKey(itemData.id))
        {
            _equipment[itemData.id] = new InventoryQuantity(itemData, 
                                                                _equipment[itemData.id].quantity + 1, 
                                                                _equipment[itemData.id].numEquipped);
            OnInventoryChange?.Invoke(_equipment[itemData.id]);
        }
        else if(!equipment)
        {
            _items.Add(itemData.id, new InventoryQuantity(itemData, 1, 0));
            OnInventoryChange?.Invoke(_items[itemData.id]);
        }
        else
        {
            _equipment.Add(itemData.id, new InventoryQuantity(itemData, 1, 0));
            OnInventoryChange?.Invoke(_equipment[itemData.id]);
        }
    }

    public void RemoveItem(ItemData itemData, bool equipment = false)
    {
        if(!equipment && _items.ContainsKey(itemData.id))
        {
            InventoryQuantity quant = _items[itemData.id];
            if(quant.quantity <= 1 && quant.numEquipped == 0)
            {
                _items.Remove(itemData.id);
                OnInventoryChange?.Invoke(_items[itemData.id]);
            }
            else
            {
                _items[itemData.id] = new InventoryQuantity(itemData, quant.quantity - 1, quant.numEquipped);
            }
        }
        else if(equipment && _equipment.ContainsKey(itemData.id))
        {
            InventoryQuantity quant = _equipment[itemData.id];
            if(quant.quantity <= 1 && quant.numEquipped == 0)
            {
                _equipment.Remove(itemData.id);
                OnInventoryChange?.Invoke(new InventoryQuantity(itemData, 0, 0));
            }
            else
            {
                _equipment[itemData.id] = new InventoryQuantity(itemData, quant.quantity - 1, quant.numEquipped);
                OnInventoryChange?.Invoke(_equipment[itemData.id]);
            }
        }
    }

    void MoveFromInventoryToEquip(EquipmentData equipmentData)
    {
        if(_equipment.ContainsKey(equipmentData.id))
        {
            _equipment[equipmentData.id] = new InventoryQuantity(equipmentData, 
                                                                    _equipment[equipmentData.id].quantity - 1, 
                                                                    _equipment[equipmentData.id].numEquipped + 1);
            OnInventoryChange?.Invoke(_equipment[equipmentData.id]);
        }
    }

    void MoveFromEquipToInventory(EquipmentData equipmentData)
    {
        if(_equipment.ContainsKey(equipmentData.id))
        {
            _equipment[equipmentData.id] = new InventoryQuantity(equipmentData, 
                                                                    _equipment[equipmentData.id].quantity + 1, 
                                                                    _equipment[equipmentData.id].numEquipped - 1);
            OnInventoryChange?.Invoke(_equipment[equipmentData.id]);
        }
    }

    public InventorySaveData GetSaveData()
    {
        InventorySaveData saveData = new InventorySaveData();

        saveData.items = new InventoryQuantitySaveData[_items.Count];
        int i = 0;
        foreach(KeyValuePair<string, InventoryQuantity> invQuantity in _items)
        {
            saveData.items[i] = invQuantity.Value.GetSaveData();
            i++;
        }
        
        saveData.equipment = new InventoryQuantitySaveData[_equipment.Count];
        i = 0;
        foreach(KeyValuePair<string, InventoryQuantity> invQuantity in _equipment)
        {
            saveData.equipment[i] = invQuantity.Value.GetSaveData();
            i++;
        }

        return saveData;
    }

    public bool LoadFromSaveData(InventorySaveData saveData)
    {
        _items.Clear();
        foreach(InventoryQuantitySaveData itemQuantity in saveData.items)
        {
            ItemData itemData = SaveDataLoader.Instance.GetItemData(itemQuantity.itemID);
            if(itemData != null)
            {
                _items.Add(itemData.id, new InventoryQuantity(itemData, itemQuantity.quantity, itemQuantity.numEquipped));
            }
        }

        _equipment.Clear();
        foreach(InventoryQuantitySaveData itemQuantity in saveData.equipment)
        {
            ItemData itemData = SaveDataLoader.Instance.GetItemData(itemQuantity.itemID);
            if(itemData != null)
            {
                _equipment.Add(itemData.id, new InventoryQuantity(itemData, itemQuantity.quantity, itemQuantity.numEquipped));
            }
        }

        return true;
    }
}