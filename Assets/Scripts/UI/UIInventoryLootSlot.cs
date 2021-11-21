using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventoryLootSlot : UIInventorySlot, IPointerDownHandler
{
    public override void RefreshSlotSprite()
    {
        if (connectedSlot != null && connectedSlot.currentItem != null)
            slotSprite.sprite = connectedSlot.currentItem.itemIcon;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (isInteractable == true)
        {
            if (connectedSlot.currentItem != null && connectedSlot.currentItem.itemID != 0)
            {
                descriptionWindow.SetActive(true);
                selectionVisual.gameObject.SetActive(true);
                takeButton.lootSlot = this;
                takeButton.gameObject.SetActive(true);
                removeItemButton.SetActive(false);
                useItemButton.SetActive(false);
            }

            StartCoroutine(ShowItemDescription());

            /*
            InventoryItem item = connectedSlot.currentItem;
            if (item != null && item.itemID != 0)
            {
                itemNameText.text = Translate.TranslateText(item.itemNameRaw);
                itemDescriptionText.text = Translate.TranslateText(item.itemDescription).Replace("@", System.Environment.NewLine);
                string stats = Translate.TranslateText(item.itemStats).Replace("@", System.Environment.NewLine);
                while (stats.Contains("{") && stats.Contains("}"))
                {
                    int firstBracket = stats.IndexOf("{") + 1;
                    int secondBracket = stats.IndexOf("}") + 1;
                    ItemStateBonus bonus = item.statesBonus.Find(b => b.stateName == stats.Substring(firstBracket, secondBracket - firstBracket - 1));
                    if (bonus != null)
                        stats = stats.Replace(stats.Substring(firstBracket - 1, secondBracket - firstBracket + 1), bonus.stateValue.ToString());
                    else stats = stats.Replace(stats.Substring(firstBracket - 1, secondBracket - firstBracket + 1), "NOT FOUND");
                }
                itemStatsText.text = stats;
            }
            RefreshSlotSprite();
            */
        }
    }
}