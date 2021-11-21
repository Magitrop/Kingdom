using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInventoryUseItemButton : UIInventoryActionButton, IPointerDownHandler
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
            if (player.GetStateValue("cur_energy") > 0)
            {
                player.connectedInventory.GetItemBySlotIndex(player.connectedInventory.selectedSlotIndex).Use(player, player.connectedInventory.selectedSlotIndex);
                if (player.connectedInventory.GetItemBySlotIndex(player.connectedInventory.selectedSlotIndex).destroyAfterUse == true)
                {
                    player.connectedInventory.PlaceItemInSlot(ItemsDatabase.GetItemByID(0), player.connectedInventory.selectedSlotIndex);
                    player.SetStateValue("cur_energy", player.GetStateValue("cur_energy") - 1);
                    player.RefreshEnergybar();
                    player.connectedInventory.selectedSlotIndex = -1;
                    descriptionPanel.SetActive(false);
                    tooltip.gameObject.SetActive(false);
                    player.DeselectVisualSlots();
                }
            }
            else player.map.ShowNotification("notifications_not_enough_energy");
        }

        player.RefreshStatsVisual();
    }
}