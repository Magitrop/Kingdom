using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventoryLootButton : UIInventoryActionButton, IPointerDownHandler
{
    public GameObject playerInventory, lootInventory, descriptionWindow, removeItemButton, equipItemButton, craftItemButton, useItemButton;
    public UIInventoryTakeButton takeButton;
    public UIInventoryCraftButton craftButton;
    public Text itemNameText, itemDescriptionText, itemStatsText;
    public Transform lootSlotsContainer;
    public UIInventoryLootSlot lootSlotExemplar;
    public UIFloatingText floatingText;

    public List<UIInventoryLootSlot> instantiatedLootSlots = new List<UIInventoryLootSlot>();

    public bool isInteractable;

    public void Loot()
    {
        if (playerInventory.activeSelf == false)
        {
            Creature creature = player.GetCreatureInFront();
            if (creature != null)
            {
                if (creature.isAlive == false || creature.canBeLootedWhileAlive == true)
                {
                    craftButton.inCraftMode = false;
                    craftButton.buttonFrame.SetActive(false);
                    for (int i = 0; i < instantiatedLootSlots.Count; i++)
                        if (instantiatedLootSlots[i] != null)
                            Destroy(instantiatedLootSlots[i].gameObject);
                    instantiatedLootSlots.Clear();

                    playerInventory.SetActive(true);
                    player.connectedInventory.selectedSlotIndex = -1;
                    descriptionWindow.SetActive(false);
                    player.DeselectVisualSlots();
                    lootInventory.SetActive(true);
                    equipItemButton.SetActive(false);
                    craftItemButton.SetActive(false);
                    useItemButton.SetActive(false);
                    for (int i = 0; i < creature.connectedInventory.inventorySlotsCount; i++)
                    {
                        var item = creature.connectedInventory.GetItemBySlotIndex(i);
                        if (item.itemID != 0)
                        {
                            UIInventoryLootSlot lootSlot = Instantiate(lootSlotExemplar);
                            lootSlot.transform.SetParent(lootSlotsContainer);
                            lootSlot.inventory = creature.connectedInventory;
                            lootSlot.slotIndex = i;
                            lootSlot.connectedSlot = lootSlot.inventory.inventorySlots[i];
                            lootSlot.takeButton = takeButton;
                            lootSlot.removeItemButton = removeItemButton;
                            lootSlot.useItemButton = useItemButton;
                            lootSlot.descriptionWindow = descriptionWindow;
                            lootSlot.itemNameText = itemNameText;
                            lootSlot.itemDescriptionText = itemDescriptionText;
                            lootSlot.itemStatsText = itemStatsText;

                            lootSlot.RefreshSlotSprite();
                            instantiatedLootSlots.Add(lootSlot);
                        }
                    }
                }
            }
            else
            {
                UIFloatingText text = Instantiate(floatingText, player.transform.position + new Vector3(0, 0.5f, -2), Quaternion.identity);
                text.textComponent.text = Translate.TranslateText("float_no_creature_to_loot");
            }
        }
        else
        {
            playerInventory.SetActive(false);
            lootInventory.SetActive(false);
        }

        player.RefreshStatsVisual();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (isInteractable == true)
        {
            base.OnPointerDown(eventData);
            player.DisableSpellMode();
            if (isUnavailable == true)
                return;
            /*
            if (player.isAlive == false || player.inSpellMode == true || player.inExploreMode == true)
                return;
                */
            Loot();
        }
    }
}