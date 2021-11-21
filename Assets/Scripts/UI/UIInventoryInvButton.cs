using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInventoryInvButton : UIInventoryActionButton, IPointerDownHandler
{
    public GameObject lootInventory, equipItemButton, descriptionWindow;
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
        player.visualInventory.SetActive(!player.visualInventory.activeSelf);
        player.connectedInventory.selectedSlotIndex = -1;
        descriptionWindow.SetActive(false);
        player.DeselectVisualSlots();
        lootInventory.SetActive(false);
        equipItemButton.SetActive(true);
        craftButton.gameObject.SetActive(true);
        craftButton.inCraftMode = false;
        craftButton.buttonFrame.SetActive(false);
        player.DisableSpellMode();

        player.RefreshStatsVisual();
    }
}