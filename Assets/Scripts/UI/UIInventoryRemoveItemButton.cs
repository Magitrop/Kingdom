using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInventoryRemoveItemButton : UIInventoryActionButton, IPointerDownHandler
{
    public GameObject descriptionPanel;
    public UIInventoryCraftButton craftButton;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (isUnavailable == true)
            return;
        /*
        if (player.isAlive == false || player.inSpellMode == true || player.inExploreMode == true)
            return;
            */
        if (craftButton.inCraftMode == false)
        {
            player.connectedInventory.PlaceItemInSlot(ItemsDatabase.GetItemByID(0), player.connectedInventory.selectedSlotIndex);
            player.connectedInventory.selectedSlotIndex = -1;
            descriptionPanel.SetActive(false);
            tooltip.gameObject.SetActive(false);
            player.DeselectVisualSlots();
        }

        player.RefreshStatsVisual();
    }
}