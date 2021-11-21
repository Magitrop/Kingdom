using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventoryCraftButton : UIInventoryActionButton, IPointerDownHandler
{
    public GameObject buttonFrame;
    public GameObject descriptionWindow;
    public List<ItemSlot> recipeSelectedItems;
    public bool inCraftMode;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (isUnavailable == true)
            return;
        /*
        if (player.isAlive == false || player.inSpellMode == true || player.inExploreMode == true)
            return;
            */
        inCraftMode = !inCraftMode;
        buttonFrame.SetActive(inCraftMode);
        descriptionWindow.SetActive(false);
        player.connectedInventory.selectedSlotIndex = -1;
        player.DeselectVisualSlots();

        if (inCraftMode == false)
        {
            Recipe currentRecipe = null;
            for (int i = 0; i < RecipesDatabase.registeredRecipes.Count; i++)
            {
                currentRecipe = RecipesDatabase.registeredRecipes[i];
                if (currentRecipe.itemResult.recipeShouldBeExplored == true && player.exploredRecipes.Contains(currentRecipe.itemResult.itemID) == false)
                {
                    currentRecipe = null;
                    continue;
                }

                if (currentRecipe.GetAmountOfAllComponents() + currentRecipe.GetAmountOfAllNeeded() != recipeSelectedItems.Count)
                {
                    currentRecipe = null;
                    continue;
                }
                else
                {
                    for (int j = 0; j < currentRecipe.itemsComponents.Count; j++)
                    {
                        if (recipeSelectedItems.Where(it => it.currentItem.itemID == currentRecipe.itemsComponents[j].itemID).ToList().Count != currentRecipe.GetAmountOfComponent(currentRecipe.itemsComponents[j].itemID))
                        {
                            currentRecipe = null;
                            break;
                        }
                    }
                    if (currentRecipe == null) continue;
                    for (int j = 0; j < currentRecipe.itemsNeeded.Count; j++)
                        if (recipeSelectedItems.Where(it => it.currentItem.itemID == currentRecipe.itemsNeeded[j].itemID).ToList().Count != currentRecipe.GetAmountOfNeeded(currentRecipe.itemsNeeded[j].itemID))
                        {
                            currentRecipe = null;
                            break;
                        }
                    if (currentRecipe == null) continue;
                }

                break;

                /*
                for (int j = 0; j < currentRecipe.itemsComponents.Count; j++)
                {
                    if (recipeSelectedItems.Where(r => r.currentItem.itemID == currentRecipe.itemsComponents[j].itemID).ToList().Count < currentRecipe.itemsComponents[j].itemsCount)
                    {
                        nextRecipe = true;
                        break;
                    }
                }

                if (nextRecipe == true)
                {
                    nextRecipe = false;
                    currentRecipe = null;
                    continue;
                }

                for (int j = 0; j < currentRecipe.itemsNeeded.Count; j++)
                {
                    if (recipeSelectedItems.Where(r => r.currentItem.itemID == currentRecipe.itemsNeeded[j].itemID).ToList().Count < currentRecipe.itemsNeeded[j].itemsCount)
                    {
                        nextRecipe = true;
                        break;
                    }
                }

                if (nextRecipe == true)
                {
                    nextRecipe = false;
                    currentRecipe = null;
                    continue;
                }
                else break;
                */
            }

            if (currentRecipe != null)
            {
                int totalComponentsConsumed = 0;
                int componentsConsumed = 0;
                for (int i = 0; i < currentRecipe.itemsComponents.Count; i++)
                {
                    for (int j = 0; j < recipeSelectedItems.Count; j++)
                    {
                        if (recipeSelectedItems[j].currentItem.itemID == currentRecipe.itemsComponents[i].itemID)
                        {
                            player.connectedInventory.PlaceItemInSlot(ItemsDatabase.GetItemByID(0), recipeSelectedItems[j].slotIndex);
                            componentsConsumed++;
                            totalComponentsConsumed++;
                            if (componentsConsumed >= currentRecipe.itemsComponents[i].itemsCount)
                            {
                                componentsConsumed = 0;
                                break;
                            }
                        }
                    }
                }

                if (totalComponentsConsumed == currentRecipe.GetAmountOfAllComponents())
                {
                    player.connectedInventory.AddItemToInventory(currentRecipe.itemResult);
                }
            }

            player.connectedInventory.selectedSlotIndex = -1;
            player.DeselectVisualSlots();
            recipeSelectedItems.Clear();
            currentRecipe = null;
        }

        player.RefreshStatsVisual();
    }
}