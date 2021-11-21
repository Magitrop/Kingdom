using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : Inventory
{
    public UIInventorySlot[] visualSlots;
         
    public override void Initialize()
    {
        base.Initialize();
        for (int i = 0; i < inventorySlotsCount; i++)
            visualSlots[i].connectedSlot = inventorySlots[i];
        RefreshVisualSlots();
    }

    public void RefreshVisualSlots()
    {
        for (int i = 0; i < inventorySlotsCount; i++)
            visualSlots[i].RefreshSlotSprite();
    }

    public override void PlaceItemInSlot(InventoryItem item, int slotIndex)
    {
        base.PlaceItemInSlot(item, slotIndex);
        RefreshVisualSlots();
    }

    public override void DisposeItem(int slotFrom, int slotTo)
    {
        base.DisposeItem(slotFrom, slotTo);
        RefreshVisualSlots();
    }

    public override void AddItemToInventory(InventoryItem item)
    {
        base.AddItemToInventory(item);
        RefreshVisualSlots();
    }
}