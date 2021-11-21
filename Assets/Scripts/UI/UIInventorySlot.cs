using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventorySlot : MonoBehaviour, IPointerDownHandler
{
    public Inventory inventory;
    public int slotIndex;
    public Image slotSprite;

    public Sprite standardSlotSprite;
    public ItemSlot connectedSlot;

    public UIInventoryTakeButton takeButton;
    public GameObject descriptionWindow;
    public GameObject removeItemButton;
    public GameObject useItemButton;
    public Image selectionVisual;
    public Text itemNameText, itemDescriptionText, itemStatsText, slotKeyText;

    public bool isInteractable;
    public UIInventoryCraftButton craftButton;

    private PlayerController player;

    private void Start()
    {
        slotKeyText.text = (slotIndex + 1).ToString();
        if (inventory.inventoryOwner.GetComponent<PlayerController>() != null)
            player = inventory.inventoryOwner.GetComponent<PlayerController>();
    }

    public virtual void RefreshSlotSprite()
    {
        if (inventory.selectedSlotIndex != -1 && connectedSlot.currentItem != null && connectedSlot.currentItem.itemID != 0)
            descriptionWindow.SetActive(true);
        else
            descriptionWindow.SetActive(false);
        //selectionVisual.gameObject.SetActive(false);
        if (connectedSlot != null && connectedSlot.currentItem != null)
            slotSprite.sprite = connectedSlot.currentItem.itemIcon;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (isInteractable == true)
        {
            if (player.inExploreMode == true)
            {
                if (connectedSlot.currentItem.itemID != 0)
                {
                    player.ExploreItem(connectedSlot.currentItem, slotIndex);
                }

                player.inExploreMode = false;
                player.DeselectVisualSlots();
            }
            else if (craftButton.inCraftMode == false)
            {
                /*
                if (player != null)
                    player.DeselectVisualSlots();
                    */
                if (inventory.selectedSlotIndex == -1)
                {
                    if (connectedSlot.currentItem.itemID != 0)
                    {
                        descriptionWindow.SetActive(true);
                        removeItemButton.SetActive(player.lootInventory.activeSelf == false);
                        useItemButton.SetActive(player.lootInventory.activeSelf == false);
                        inventory.selectedSlotIndex = slotIndex;
                        selectionVisual.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (inventory.selectedSlotIndex != slotIndex)
                        inventory.DisposeItem(inventory.selectedSlotIndex, slotIndex);
                    //selectionVisual.gameObject.SetActive(false);
                    descriptionWindow.SetActive(false);
                    inventory.selectedSlotIndex = -1;
                    if (player != null)
                        player.DeselectVisualSlots();
                }

                if (takeButton.lootSlot != null)
                {
                    takeButton.lootSlot.selectionVisual.gameObject.SetActive(false);
                    takeButton.lootSlot = null;
                }
                takeButton.gameObject.SetActive(false);

                //if (inventory.selectedSlotIndex != -1)
                StartCoroutine(ShowItemDescription());
            }
            else
            {
                if (craftButton.recipeSelectedItems.Any(i => i.slotIndex == connectedSlot.slotIndex) == false)
                {
                    if (connectedSlot.currentItem != null && connectedSlot.currentItem.itemID != 0)
                    {
                        craftButton.recipeSelectedItems.Add(connectedSlot);
                        selectionVisual.gameObject.SetActive(true);
                    }
                }
                else
                {
                    for (int i = 0; i < craftButton.recipeSelectedItems.Count; i++)
                        if (craftButton.recipeSelectedItems[i].slotIndex == connectedSlot.slotIndex)
                        {
                            craftButton.recipeSelectedItems.RemoveAt(i);
                            selectionVisual.gameObject.SetActive(false);
                        }
                }
            }
            RefreshSlotSprite();
        }
    }

    public IEnumerator ShowItemDescription()
    {
        InventoryItem item = connectedSlot.currentItem;
        if (item.itemID != 0)
        {
            string[] parts = item.CalculateItemDescriptionParts(item.owner);
            itemNameText.text = Translate.TranslateText(item.itemNameRaw);
            itemDescriptionText.text = Translate.TranslateText(item.itemDescription).Replace("@", System.Environment.NewLine);
            string stats = Translate.TranslateText(item.itemStats).Replace("@", System.Environment.NewLine);
            while (stats.Contains("{") && stats.Contains("}"))
            {
                int firstBracket = stats.IndexOf("{") + 1;
                int secondBracket = stats.IndexOf("}") + 1;
                int descriptionPart;
                if (int.TryParse(stats.Substring(firstBracket, secondBracket - firstBracket - 1), out descriptionPart) == true)
                    stats = stats.Replace(stats.Substring(firstBracket - 1, secondBracket - firstBracket + 1), parts[descriptionPart]);
                else if (stats.Substring(firstBracket, secondBracket - firstBracket - 1).EndsWith("_owner") == true)
                {
                    string s = stats.Substring(firstBracket, secondBracket - firstBracket - 1);
                    CreatureState bonus = item.owner.GetStateByName(s.Replace("_owner", string.Empty));
                    if (bonus != null)
                        stats = stats.Replace(stats.Substring(firstBracket - 1, secondBracket - firstBracket + 1), bonus.totalStateValue.ToString());
                    else stats = stats.Replace(stats.Substring(firstBracket - 1, secondBracket - firstBracket + 1), "NOT FOUND");
                }
                else
                {
                    ItemStateBonus bonus = item.statesBonus.Find(b => b.stateName == stats.Substring(firstBracket, secondBracket - firstBracket - 1));
                    if (bonus != null)
                        stats = stats.Replace(stats.Substring(firstBracket - 1, secondBracket - firstBracket + 1), bonus.stateValue.ToString());
                    else stats = stats.Replace(stats.Substring(firstBracket - 1, secondBracket - firstBracket + 1), "NOT FOUND");
                }
                yield return new WaitForEndOfFrame();
            }
            itemStatsText.text = stats;
            float newHeight = itemDescriptionText.preferredHeight + itemStatsText.preferredHeight;
            descriptionWindow.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight + 150);
        }
    }
}