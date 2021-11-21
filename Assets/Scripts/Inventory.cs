using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Creature inventoryOwner;

    public int inventorySlotsCount;
    public ItemSlot[] inventorySlots;

    public int selectedSlotIndex;

    public virtual void Initialize()
    {
        selectedSlotIndex = -1;
        //inventorySlots = new ItemSlot[inventorySlotsCount];
        for (int i = 0; i < inventorySlotsCount; i++)
        {
            if (inventorySlots[i].currentItem == null)
            {
                //inventorySlots[i] = new ItemSlot();
                PlaceItemInSlot(ItemsDatabase.GetItemByID(0), i);
            }
            inventorySlots[i].slotIndex = i;
        }
    }

    /// <summary>
    /// Returns true if inventory has item with this ID.
    /// </summary>
    /// <param name="itemID"></param>
    /// <returns></returns>
    public bool CheckInventoryToItemByID(int itemID)
    {
        for (int i = 0; i < inventorySlotsCount; i++)
            if (inventorySlots[i].currentItem.itemID == itemID)
                return true;
        return false;
    }

    /// <summary>
    /// Returns item-none if failed (not a null!).
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <returns></returns>
    public InventoryItem GetItemBySlotIndex(int slotIndex)
    {
        if (slotIndex != -1 && inventorySlots[slotIndex].currentItem != null && inventorySlots[slotIndex].currentItem.itemID != 0)
            return inventorySlots[slotIndex].currentItem;
        else return ItemsDatabase.GetItemByID(0);
    }

    public virtual void PlaceItemInSlot(InventoryItem item, int slotIndex)
    {
        if (item != null)
        {
            if (inventorySlots[slotIndex].currentItem != null)
                inventorySlots[slotIndex].currentItem.OnItemLose(inventoryOwner, slotIndex);
            inventorySlots[slotIndex].currentItem = item;
            inventorySlots[slotIndex].currentItem.owner = inventoryOwner;
            inventorySlots[slotIndex].currentItem.OnItemReceive(inventoryOwner, slotIndex);
        }
    }

    /// <summary>
    /// Adds an item to a first empty slot.
    /// </summary>
    /// <param name="item"></param>
    public virtual void AddItemToInventory(InventoryItem item)
    {
        if (item != null)
        {
            int slot = FindFirstEmptySlot();
            if (slot != -1)
            {
                inventorySlots[slot].currentItem = item;
                inventorySlots[slot].currentItem.owner = inventoryOwner;
                inventorySlots[slot].currentItem.OnItemReceive(inventoryOwner, slot);
            }
        }
    }

    public void ClearInventory()
    {
        for (int i = 0; i < inventorySlotsCount; i++)
            PlaceItemInSlot(ItemsDatabase.GetItemByID(0), i);
    }

    public void InventoryTick()
    {
        if (inventorySlotsCount > 0)
            for (int i = 0; i < inventorySlotsCount; i++)
                if (inventorySlots[i].currentItem != null)
                    inventorySlots[i].currentItem.OnItemTick(inventoryOwner, inventorySlots[i].slotIndex);
    }

    /// <summary>
    /// Swaps items or disposes item in selected empty slot.
    /// </summary>
    /// <param name="slotFrom"></param>
    /// <param name="slotTo"></param>
    public virtual void DisposeItem(int slotFrom, int slotTo)
    {
        InventoryItem item = inventorySlots[slotFrom].currentItem;
        inventorySlots[slotFrom].currentItem = inventorySlots[slotTo].currentItem;
        inventorySlots[slotTo].currentItem = item;
    }

    public int FindFirstEmptySlot()
    {
        for (int i = 0; i < inventorySlotsCount; i++)
            if (inventorySlots[i].currentItem == null || inventorySlots[i].currentItem.itemID == 0)
                return i;
        return -1;
    }

    public bool HasEmptySlots()
    {
        for (int i = 0; i < inventorySlotsCount; i++)
            if (inventorySlots[i].currentItem == null || inventorySlots[i].currentItem.itemID == 0)
                return true;
        return false;
    }

    public string ItemToString(int slotIndex)
    {
        if (inventorySlots[slotIndex].currentItem != null)
            return inventorySlots[slotIndex].currentItem.ToString();
        else return string.Empty;
    }

    /// <summary>
    /// Returns recipe item information by given item ID. Returns 0 if inventory does not contain item with that ID or itemID equals to 0.
    /// </summary>
    /// <param name="itemID"></param>
    /// <returns></returns>
    public RecipeItemInformation GetRecipeItemInformation(int itemID)
    {
        RecipeItemInformation result = new RecipeItemInformation(itemID, 0);
        if (itemID != 0)
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].currentItem != null && inventorySlots[i].currentItem.itemID == itemID)
                    result.itemsCount++;
            }
        }
        return result;
    }

    /// <summary>
    /// Returns items amount by given item ID. Returns 0 if inventory does not contain item with that ID or itemID equals to 0.
    /// </summary>
    /// <param name="itemID"></param>
    /// <returns></returns>
    public int GetItemsAmount(int itemID)
    {
        int result = 0;
        if (itemID != 0)
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].currentItem != null && inventorySlots[i].currentItem.itemID == itemID)
                    result++;
            }
        }
        return result;
    }
}

public enum ItemType
{
    Equipment,
    Weapon,
    Consumable,
    Component,
    Miscellaneous
}

[System.Serializable]
public class ItemStateBonus
{
    /*
    public enum ItemStateBonusType
    {
        Add,
        Multiply
    }
    */

    public string stateName;
    public int stateValue;
    //public ItemStateBonusType addOrMultiply;

    public ItemStateBonus(string _stateName, int _stateValue)
    {
        stateName = _stateName;
        stateValue = _stateValue;
        //addOrMultiply = _addOrMultiply;
    }
}

[System.Serializable]
public abstract class InventoryItem
{
    public int itemID { get; protected set; }
    public string itemName, itemNameRaw;
    public Sprite itemIcon;
    public string itemDescription, itemStats;

    public bool destroyAfterUse;

    /// <summary>
    /// Can item be crafted without recipe exploration?
    /// </summary>
    public bool recipeShouldBeExplored;
    /// <summary>
    /// Recipe of which item you will learn after exploration?
    /// </summary>
    public int recipeExplorationResultID;

    public ItemType itemType;
    public DamageType itemDamageType;

    public List<ItemStateBonus> statesBonus = new List<ItemStateBonus>();
    public Creature owner;

    public InventoryItem(int ID)
    {
        itemID = ID;
    }

    /// <summary>
    /// Triggers every time owner of this item starts its turn (note: first trigger will happen only next turn after receiving).
    /// </summary>
    public virtual void OnItemTick(Creature owner, int slotIndex) { }
    public virtual void OnItemReceive(Creature owner, int slotIndex) { }
    public virtual void OnItemLose(Creature owner, int slotIndex) { }
    public virtual void Use(Creature owner, int slotIndex) { }
    public virtual void OnAttack(Creature owner, Creature attackedCreature, int slotIndex) { }
    public virtual void OnMove(Creature owner, int slotIndex) { }

    public virtual void CalculateBonusStates(Creature owner) { }
    public virtual void CancelBonusStates(Creature owner) { }

    public virtual string[] CalculateItemDescriptionParts(Creature owner) { return null; }

    public override string ToString()
    {
        return itemID.ToString();
    }
}

[System.Serializable]
public class ItemSlot
{
    public InventoryItem currentItem;
    public int slotIndex;
}