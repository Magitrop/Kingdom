using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipesDatabase
{
    public static List<Recipe> registeredRecipes = new List<Recipe>();

    public static void InitializeDatabase()
    {
        registeredRecipes.Clear();

        // Wooden sword:
        // Components:
        // 2 sticks
        // Tools:
        // 1 knife
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(3),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(8, 2)
            },
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(9, 1)
            }));

        // Bone blade:
        // Components:
        // 1 stick
        // 2 old bones
        // Tools:
        // 1 knife
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(13),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(8, 1),
                new RecipeItemInformation(5, 2)
            },
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(9, 1)
            }));

        // Thickened leather:
        // Components:
        // 2 leather
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(15),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(14, 2)
            },
            null));

        // Left leather glove:
        // Components:
        // 1 thickened leather
        // Tools:
        // 1 knife
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(1),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(15, 1)
            },
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(9, 1)
            }));

        // Left leather glove:
        // Components:
        // 1 Right leather glove
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(1),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(2, 1)
            },
            null));

        // Right leather glove:
        // Components:
        // 1 Left leather glove
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(2),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(1, 1)
            },
            null));

        // Leather helmet:
        // Components:
        // 2 thickened leather
        // Tools:
        // 1 knife
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(16),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(15, 2)
            },
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(9, 1)
            }));

        // Leather breastplace:
        // Components:
        // 3 thickened leather
        // Tools:
        // 1 knife
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(17),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(15, 3)
            },
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(9, 1)
            }));

        // Spellful powder:
        // Components:
        // 1 spectral dust
        // 1 spellful essence vial
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(19),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(4, 1),
                new RecipeItemInformation(18, 1)
            },
            null));

        // Impregnated leather:
        // Components:
        // 1 leather
        // 1 spellful essence vial
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(20),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(14, 1),
                new RecipeItemInformation(18, 1)
            },
            null));

        // Thickened impregnated leather:
        // Components:
        // 2 impregnated leather
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(21),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(20, 2)
            },
            null));

        // Left impregnated leather glove:
        // Components:
        // 1 thickened impregnated leather
        // Tools:
        // 1 knife
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(22),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(21, 1)
            },
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(9, 1)
            }));

        // Left impregnated leather glove:
        // Components:
        // 1 Right impregnated leather glove
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(22),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(23, 1)
            },
            null));

        // Right impregnated leather glove:
        // Components:
        // 1 Left impregnated leather glove
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(23),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(22, 1)
            },
            null));

        // Impregnated leather helmet:
        // Components:
        // 2 thickened impregnated leather
        // Tools:
        // 1 knife
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(24),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(21, 2)
            },
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(9, 1)
            }));

        // Impregnated leather breastplace:
        // Components:
        // 3 thickened impregnated leather
        // Tools:
        // 1 knife
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(25),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(21, 3)
            },
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(9, 1)
            }));
        
        // Bone needle:
        // Components:
        // 1 old bones
        // Tools:
        // 1 knife
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(28),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(5, 1)
            },
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(9, 1)
            }));

        // Icy dirk:
        // Components:
        // 2 ice chunks
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(40),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(41, 2)
            },
            null));

        // Ice chunk:
        // Components:
        // 2 icicles
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(41),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(36, 2)
            },
            null));

        // Little inkwell:
        // Components:
        // 1 spellful essence vial
        // 1 spectral dust
        // 1 bone needle
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(27),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(4, 1),
                new RecipeItemInformation(18, 1),
                new RecipeItemInformation(28, 1)
            },
            null));

        // Comprehension book
        // Components:
        // 1 little inkwell
        // 2 old paper
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(39),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(27, 1),
                new RecipeItemInformation(35, 2)
            },
            null));

        // Brightening spell ink
        // Components:
        // 1 little inkwell
        // 2 glowing sprouts
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(31),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(27, 1),
                new RecipeItemInformation(34, 2)
            },
            null));

        // Brightening spell scroll
        // Components:
        // 1 Brightening spell ink
        // 1 old paper
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(30),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(31, 1),
                new RecipeItemInformation(35, 1)
            },
            null));

        // Ice Boulder spell ink
        // Components:
        // 1 little inkwell
        // 2 icicles
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(33),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(27, 1),
                new RecipeItemInformation(36, 2)
            },
            null));

        // Ice Boulder spell scroll
        // Components:
        // 1 Ice Boulder spell ink
        // 1 old paper
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(32),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(33, 1),
                new RecipeItemInformation(35, 1)
            },
            null));

        // Energy strike spell ink
        // Components:
        // 1 little inkwell
        // 
        /*
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(38),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(27, 1)
            },
            null));
            */

        // Energy strike spell scroll
        // Components:
        // 1 Energy strike spell ink
        // 1 old paper
        registeredRecipes.Add(
            new Recipe(ItemsDatabase.GetItemByID(37),
            new List<RecipeItemInformation>()
            {
                new RecipeItemInformation(38, 1),
                new RecipeItemInformation(35, 1)
            },
            null));
    }

    /*
    /// <summary>
    /// Returns recipe by given items. If mistakenly will be found more than one recipe with those items, will return the first one.
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    public static Recipe FindRecipeByItems(params InventoryItem[] items)
    {
        Recipe result = null;
        bool nextRecipe = false;
        Recipe currentRecipe = null;
        for (int i = 0; i < registeredRecipes.Count; i++)
        {
            currentRecipe = registeredRecipes[i];
            for (int j = 0; j < currentRecipe.itemsComponents.Count; j++)
            {
                if (GetItemsAmount(currentRecipe.itemsComponents[j].itemID) < currentRecipe.itemsComponents[j].itemsCount)
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
                if (GetItemsAmount(currentRecipe.itemsNeeded[j].itemID) < currentRecipe.itemsNeeded[j].itemsCount)
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
        }

        return result;
    }
    */
}

[System.Serializable]
public class Recipe
{
    /// <summary>
    /// Warning, itemsComponents cannot contain more than one type of each item. Cannot contain items from needed.
    /// </summary>
    public List<RecipeItemInformation> itemsComponents = new List<RecipeItemInformation>();
    /// <summary>
    /// Warning, itemsNeeded cannot contain more than one type of each item. Cannot contain items from components.
    /// </summary>
    public List<RecipeItemInformation> itemsNeeded = new List<RecipeItemInformation>();
    public InventoryItem itemResult;

    public Recipe(InventoryItem result, List<RecipeItemInformation> components, List<RecipeItemInformation> needed)
    {
        itemResult = result;
        for (int i = 0; i < components.Count; i++)
            itemsComponents.Add(components[i]);
        if (needed != null)
            for (int i = 0; i < needed.Count; i++)
                itemsNeeded.Add(needed[i]);
    }

    public int GetAmountOfComponent(int componentItemID)
    {
        return itemsComponents.Find(c => c.itemID == componentItemID).itemsCount;
    }

    public int GetAmountOfNeeded(int neededItemID)
    {
        return itemsNeeded.Find(c => c.itemID == neededItemID).itemsCount;
    }

    public int GetAmountOfAllComponents()
    {
        int result = 0;
        for (int i = 0; i < itemsComponents.Count; i++)
            result += itemsComponents[i].itemsCount;
        return result;
    }

    public int GetAmountOfAllNeeded()
    {
        int result = 0;
        for (int i = 0; i < itemsNeeded.Count; i++)
            result += itemsNeeded[i].itemsCount;
        return result;
    }
}

[System.Serializable]
public class RecipeItemInformation
{
    public int itemID;
    public int itemsCount;

    public RecipeItemInformation(int ID, int count)
    {
        itemID = ID;
        itemsCount = count;
    }
}