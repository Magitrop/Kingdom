using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInventoryTakeButton : MonoBehaviour, IPointerDownHandler
{
    public PlayerInventory playerInventory;
    public UIInventoryLootSlot lootSlot;

    public GameObject descriptionWindow;
    public bool isInteractable;
    public UIInventoryCraftButton craftButton;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (playerInventory.inventoryOwner.isAlive == false || playerInventory.inventoryOwner.GetComponent<PlayerController>().inSpellMode == true || playerInventory.inventoryOwner.GetComponent<PlayerController>().inExploreMode == true)
            return;
        if (isInteractable == true && craftButton.inCraftMode == false)
        {
            if (lootSlot != null && lootSlot.connectedSlot.currentItem != null && lootSlot.connectedSlot.currentItem.itemID != 0)
            {
                if (playerInventory.HasEmptySlots() == true)
                {
                    playerInventory.AddItemToInventory(lootSlot.connectedSlot.currentItem);
                    lootSlot.connectedSlot.currentItem = ItemsDatabase.GetItemByID(0);
                    lootSlot.selectionVisual.gameObject.SetActive(false);
                    lootSlot.RefreshSlotSprite();
                    lootSlot = null;
                    gameObject.SetActive(false);
                    descriptionWindow.SetActive(false);
                }
            }
        }
    }
}