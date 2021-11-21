using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInventoryEquipItemButton : UIInventoryActionButton, IPointerDownHandler
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
            InventoryItem item = player.connectedInventory.GetItemBySlotIndex(player.connectedInventory.selectedSlotIndex);
            if (item.itemType == ItemType.Weapon)
            {
                //player.GetStateByName("attack_damage").valueAddends = 1;
                for (int i = 0; i < player.connectedInventory.inventorySlotsCount; i++)
                    player.connectedInventory.GetItemBySlotIndex(i).CancelBonusStates(player);
                player.autoattackDamageType = item.itemDamageType;
                for (int i = 0; i < player.connectedInventory.inventorySlotsCount; i++)
                    player.connectedInventory.GetItemBySlotIndex(i).CalculateBonusStates(player);
                //player.RecalculateStates();
            }
            descriptionPanel.SetActive(false);
            player.connectedInventory.selectedSlotIndex = -1;
            player.DeselectVisualSlots();
        }

        player.RefreshStatsVisual();
    }
}