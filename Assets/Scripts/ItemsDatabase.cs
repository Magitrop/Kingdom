using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemsDatabase
{
    public static List<InventoryItem> registeredItems = new List<InventoryItem>();
    public static string itemsIconsDirectory;

    public static void InitializeDatabase()
    {
        itemsIconsDirectory = "ItemsSprites/";

        registeredItems.Clear();
        registeredItems.Add(new ItemNone(0));
        registeredItems.Add(new ItemLeatherGloveLeft(1));
        registeredItems.Add(new ItemLeatherGloveRight(2));
        registeredItems.Add(new ItemWoodenSword(3));
        registeredItems.Add(new ItemSpectralDust(4));
        registeredItems.Add(new ItemOldBone(5));
        registeredItems.Add(new ItemEvilEyeScales(6));
        registeredItems.Add(new ItemEvilEyeEyeball(7));
        registeredItems.Add(new ItemStick(8));
        registeredItems.Add(new ItemKnife(9));
        registeredItems.Add(new ItemSmallHealingPotion(10));
        registeredItems.Add(new ItemMediumHealingPotion(11));
        registeredItems.Add(new ItemBigHealingPotion(12));
        registeredItems.Add(new ItemBoneBlade(13));
        registeredItems.Add(new ItemLeather(14));
        registeredItems.Add(new ItemThickenedLeather(15));
        registeredItems.Add(new ItemLeatherHelmet(16));
        registeredItems.Add(new ItemLeatherBreastplate(17));
        registeredItems.Add(new ItemSpellfulEssenceVial(18));
        registeredItems.Add(new ItemSpellfulPowder(19));
        registeredItems.Add(new ItemImpregnatedLeather(20));
        registeredItems.Add(new ItemThickenedImpregnatedLeather(21));
        registeredItems.Add(new ItemImpregnatedLeatherGloveLeft(22));
        registeredItems.Add(new ItemImpregnatedLeatherGloveRight(23));
        registeredItems.Add(new ItemImpregnatedLeatherHelmet(24));
        registeredItems.Add(new ItemImpregnatedLeatherBreastplate(25));
        registeredItems.Add(new ItemTorch(26));
        registeredItems.Add(new ItemLittleInkwell(27));
        registeredItems.Add(new ItemBoneNeedle(28));
        registeredItems.Add(new ItemGlassLens(29));
        registeredItems.Add(new ItemScrollSpellBrightening(30));
        registeredItems.Add(new ItemInkSpellBrightening(31));
        registeredItems.Add(new ItemScrollSpellIceBoulder(32));
        registeredItems.Add(new ItemInkSpellIceBoulder(33));
        registeredItems.Add(new ItemGlowingSprout(34));
        registeredItems.Add(new ItemOldPaper(35));
        registeredItems.Add(new ItemIcicle(36));
        registeredItems.Add(new ItemScrollSpellEnergyStrike(37));
        registeredItems.Add(new ItemInkSpellEnergyStrike(38));
        registeredItems.Add(new ItemComprehensionBook(39));
        registeredItems.Add(new ItemIcyDirk(40));
        registeredItems.Add(new ItemIceChunk(41));
        registeredItems.Add(new ItemScrollSpellIceRing(42));
        registeredItems.Add(new ItemInkSpellIceRing(43));
        registeredItems.Add(new ItemScrollSpellIcing(44));
        registeredItems.Add(new ItemInkSpellIcing(45));
        registeredItems.Add(new ItemScrollSpellFrostbite(46));
        registeredItems.Add(new ItemInkSpellFrostbite(47));
        registeredItems.Add(new ItemScrollSpellSummonIceGolem(48));
        registeredItems.Add(new ItemInkSpellSummonIceGolem(49));
        registeredItems.Add(new ItemScrollSpellFrozenShield(50));
        registeredItems.Add(new ItemInkSpellFrozenShield(51));
        registeredItems.Add(new ItemScrollSpellSummonIceCrystal(52));
        registeredItems.Add(new ItemInkSpellSummonIceCrystal(53));
        registeredItems.Add(new ItemScrollSpellChillingWind(54));
        registeredItems.Add(new ItemInkSpellChillingWind(55));
        registeredItems.Add(new ItemScrollSpellChills(56));
        registeredItems.Add(new ItemInkSpellChills(57));
        registeredItems.Add(new ItemScrollSpellGlacialWisdom(58));
        registeredItems.Add(new ItemInkSpellGlacialWisdom(59));
        registeredItems.Add(new ItemScrollSpellSummonFieryMine(60));
        registeredItems.Add(new ItemInkSpellSummonFieryMine(61));
        registeredItems.Add(new ItemScrollSpellFlamingBlast(62));
        registeredItems.Add(new ItemInkSpellFlamingBlast(63));
    }

    public static InventoryItem GetItemByID(int ID)
    {
        for (int i = 0; i < registeredItems.Count; i++)
            if (registeredItems[i].itemID == ID)
                return registeredItems[i];
        return registeredItems[0];
    }
}

[System.Serializable]
public class ItemNone : InventoryItem
{
    public ItemNone(int ID) : base(ID)
    {
        itemNameRaw = "empty";
        itemDescription = "none";
        itemStats = " ";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Miscellaneous;
    }
}
[System.Serializable]
public class ItemLeatherGloveLeft : InventoryItem
{
    public ItemLeatherGloveLeft(int ID) : base(ID)
    {
        itemNameRaw = "leather_glove_left";
        itemDescription = "leather_glove_left_desc";
        itemStats = "leather_glove_left_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Equipment;

        statesBonus.Add(new ItemStateBonus("protection_blunt", 1));
        statesBonus.Add(new ItemStateBonus("protection_lashing", 1));
        statesBonus.Add(new ItemStateBonus("protection_freezing", 1));
    }

    public override void OnItemReceive(Creature owner, int slotIndex)
    {
        base.OnItemReceive(owner, slotIndex);
        if (owner != null && statesBonus != null && statesBonus.Count > 0 && owner.itemsGivingBonusesIDs.Contains(itemID) == false)
        {
            owner.itemsGivingBonusesIDs.Add(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                owner.ChangeStateBonus(statesBonus[i].stateName, statesBonus[i].stateValue);
        }
    }

    public override void OnItemLose(Creature owner, int slotIndex)
    {
        base.OnItemLose(owner, slotIndex);
        if (statesBonus != null && statesBonus.Count > 0 && owner.connectedInventory.GetItemsAmount(itemID) == 1)
        {
            owner.itemsGivingBonusesIDs.Remove(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                owner.ChangeStateBonus(statesBonus[i].stateName, -statesBonus[i].stateValue);
        }
    }
}
[System.Serializable]
public class ItemLeatherGloveRight : InventoryItem
{
    public ItemLeatherGloveRight(int ID) : base(ID)
    {
        itemNameRaw = "leather_glove_right";
        itemDescription = "leather_glove_right_desc";
        itemStats = "leather_glove_right_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Equipment;

        statesBonus.Add(new ItemStateBonus("protection_blunt", 1));
        statesBonus.Add(new ItemStateBonus("protection_lashing", 1));
        statesBonus.Add(new ItemStateBonus("protection_freezing", 1));
    }

    public override void OnItemReceive(Creature owner, int slotIndex)
    {
        base.OnItemReceive(owner, slotIndex);
        if (owner != null && statesBonus != null && statesBonus.Count > 0 && owner.itemsGivingBonusesIDs.Contains(itemID) == false)
        {
            owner.itemsGivingBonusesIDs.Add(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                owner.ChangeStateBonus(statesBonus[i].stateName, statesBonus[i].stateValue);
        }
    }

    public override void OnItemLose(Creature owner, int slotIndex)
    {
        base.OnItemLose(owner, slotIndex);
        if (statesBonus != null && statesBonus.Count > 0 && owner.connectedInventory.GetItemsAmount(itemID) == 1)
        {
            owner.itemsGivingBonusesIDs.Remove(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                owner.ChangeStateBonus(statesBonus[i].stateName, -statesBonus[i].stateValue);
        }
    }
}
[System.Serializable]
public class ItemWoodenSword : InventoryItem
{
    public ItemWoodenSword(int ID) : base(ID)
    {
        itemNameRaw = "wooden_sword";
        itemDescription = "wooden_sword_desc";
        itemStats = "wooden_sword_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemDamageType = DamageType.cutting;
        itemType = ItemType.Weapon;

        statesBonus.Add(new ItemStateBonus("attack_damage", 20));
    }

    public override void CalculateBonusStates(Creature owner)
    {
        base.CalculateBonusStates(owner);
        if (owner != null && statesBonus != null && statesBonus.Count > 0 && owner.itemsGivingBonusesIDs.Contains(itemID) == false)
        {
            owner.itemsGivingBonusesIDs.Add(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                if ((statesBonus[i].stateName == "attack_damage" && owner.autoattackDamageType == itemDamageType) || statesBonus[i].stateName != "attack_damage")
                    owner.ChangeStateBonus(statesBonus[i].stateName, statesBonus[i].stateValue);
        }
    }

    public override void CancelBonusStates(Creature owner)
    {
        base.CalculateBonusStates(owner);
        if (owner != null && statesBonus != null && statesBonus.Count > 0 && owner.connectedInventory.GetItemsAmount(itemID) == 1)
        {
            owner.itemsGivingBonusesIDs.Remove(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                if ((statesBonus[i].stateName == "attack_damage" && owner.autoattackDamageType == itemDamageType) || statesBonus[i].stateName != "attack_damage")
                    owner.ChangeStateBonus(statesBonus[i].stateName, -statesBonus[i].stateValue);
        }
    }

    public override void OnItemReceive(Creature owner, int slotIndex)
    {
        base.OnItemReceive(owner, slotIndex);
        CalculateBonusStates(owner);
    }

    public override void OnItemLose(Creature owner, int slotIndex)
    {
        base.OnItemLose(owner, slotIndex);
        CancelBonusStates(owner);
    }
}
[System.Serializable]
public class ItemSpectralDust : InventoryItem
{
    public ItemSpectralDust(int ID) : base(ID)
    {
        itemNameRaw = "spectral_dust";
        itemDescription = "spectral_dust_desc";
        itemStats = "spectral_dust_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Component;
    }
}
[System.Serializable]
public class ItemOldBone : InventoryItem
{
    public ItemOldBone(int ID) : base(ID)
    {
        itemNameRaw = "old_bone";
        itemDescription = "old_bone_desc";
        itemStats = "old_bone_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Component;
    }
}
[System.Serializable]
public class ItemEvilEyeScales : InventoryItem
{
    public ItemEvilEyeScales(int ID) : base(ID)
    {
        itemNameRaw = "evil_eye_scales";
        itemDescription = "evil_eye_scales_desc";
        itemStats = "evil_eye_scales_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Component;
    }
}
[System.Serializable]
public class ItemEvilEyeEyeball : InventoryItem
{
    public ItemEvilEyeEyeball(int ID) : base(ID)
    {
        itemNameRaw = "evil_eye_eyeball";
        itemDescription = "evil_eye_eyeball_desc";
        itemStats = "evil_eye_eyeball_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Component;
    }
}
[System.Serializable]
public class ItemKnife : InventoryItem
{
    public ItemKnife(int ID) : base(ID)
    {
        itemNameRaw = "knife";
        itemDescription = "knife_desc";
        itemStats = "knife_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemDamageType = DamageType.pricking;
        itemType = ItemType.Weapon;

        statesBonus.Add(new ItemStateBonus("attack_damage", 10));
    }

    public override void CalculateBonusStates(Creature owner)
    {
        base.CalculateBonusStates(owner);
        if (owner != null && statesBonus != null && statesBonus.Count > 0 && owner.itemsGivingBonusesIDs.Contains(itemID) == false)
        {
            owner.itemsGivingBonusesIDs.Add(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                if ((statesBonus[i].stateName == "attack_damage" && owner.autoattackDamageType == itemDamageType) || statesBonus[i].stateName != "attack_damage")
                    owner.ChangeStateBonus(statesBonus[i].stateName, statesBonus[i].stateValue);
        }
    }

    public override void CancelBonusStates(Creature owner)
    {
        base.CalculateBonusStates(owner);
        if (owner != null && statesBonus != null && statesBonus.Count > 0 && owner.connectedInventory.GetItemsAmount(itemID) == 1)
        {
            owner.itemsGivingBonusesIDs.Remove(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                if ((statesBonus[i].stateName == "attack_damage" && owner.autoattackDamageType == itemDamageType) || statesBonus[i].stateName != "attack_damage")
                    owner.ChangeStateBonus(statesBonus[i].stateName, -statesBonus[i].stateValue);
        }
    }

    public override void OnItemReceive(Creature owner, int slotIndex)
    {
        base.OnItemReceive(owner, slotIndex);
        CalculateBonusStates(owner);
    }

    public override void OnItemLose(Creature owner, int slotIndex)
    {
        base.OnItemLose(owner, slotIndex);
        CancelBonusStates(owner);
    }

    public override void OnAttack(Creature owner, Creature attackedCreature, int slotIndex)
    {
        base.OnAttack(owner, attackedCreature, slotIndex);
        if (owner.autoattackDamageType == itemDamageType)
        {
            if (Random.Range(0f, 100f) < 10f)
                Effect.ApplyEffect(EffectsDatabase.GetEffectByID(1), attackedCreature, owner, 1, 1);
        }
    }
}
[System.Serializable]
public class ItemStick : InventoryItem
{
    public ItemStick(int ID) : base(ID)
    {
        itemNameRaw = "stick";
        itemDescription = "stick_desc";
        itemStats = "stick_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Component;
    }
}
[System.Serializable]
public class ItemSmallHealingPotion : InventoryItem
{
    public ItemSmallHealingPotion(int ID) : base(ID)
    {
        itemNameRaw = "small_healing_potion";
        itemDescription = "small_healing_potion_desc";
        itemStats = "small_healing_potion_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Consumable;
        destroyAfterUse = true;
    }

    public override void Use(Creature owner, int slotIndex)
    {
        base.Use(owner, slotIndex);
        owner.GetHealing(Mathf.RoundToInt(owner.GetStateValue("max_health") * 0.2f));
    }
}
[System.Serializable]
public class ItemMediumHealingPotion : InventoryItem
{
    public ItemMediumHealingPotion(int ID) : base(ID)
    {
        itemNameRaw = "medium_healing_potion";
        itemDescription = "medium_healing_potion_desc";
        itemStats = "medium_healing_potion_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Consumable;
        destroyAfterUse = true;
    }

    public override void Use(Creature owner, int slotIndex)
    {
        base.Use(owner, slotIndex);
        owner.GetHealing(Mathf.RoundToInt(owner.GetStateValue("max_health") * 0.325f));
    }
}
[System.Serializable]
public class ItemBigHealingPotion : InventoryItem
{
    public ItemBigHealingPotion(int ID) : base(ID)
    {
        itemNameRaw = "big_healing_potion";
        itemDescription = "big_healing_potion_desc";
        itemStats = "big_healing_potion_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Consumable;
        destroyAfterUse = true;
    }

    public override void Use(Creature owner, int slotIndex)
    {
        base.Use(owner, slotIndex);
        owner.GetHealing(Mathf.RoundToInt(owner.GetStateValue("max_health") * 0.45f));
    }
}
[System.Serializable]
public class ItemBoneBlade : InventoryItem
{
    public ItemBoneBlade(int ID) : base(ID)
    {
        itemNameRaw = "bone_blade";
        itemDescription = "bone_blade_desc";
        itemStats = "bone_blade_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemDamageType = DamageType.cutting;
        itemType = ItemType.Weapon;

        statesBonus.Add(new ItemStateBonus("attack_damage", 30));
    }

    public override void CalculateBonusStates(Creature owner)
    {
        base.CalculateBonusStates(owner);
        if (owner != null && statesBonus != null && statesBonus.Count > 0 && owner.itemsGivingBonusesIDs.Contains(itemID) == false)
        {
            owner.itemsGivingBonusesIDs.Add(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                if ((statesBonus[i].stateName == "attack_damage" && owner.autoattackDamageType == itemDamageType) || statesBonus[i].stateName != "attack_damage")
                    owner.ChangeStateBonus(statesBonus[i].stateName, statesBonus[i].stateValue);
        }
    }

    public override void CancelBonusStates(Creature owner)
    {
        base.CalculateBonusStates(owner);
        if (owner != null && statesBonus != null && statesBonus.Count > 0 && owner.connectedInventory.GetItemsAmount(itemID) == 1)
        {
            owner.itemsGivingBonusesIDs.Remove(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                if ((statesBonus[i].stateName == "attack_damage" && owner.autoattackDamageType == itemDamageType) || statesBonus[i].stateName != "attack_damage")
                    owner.ChangeStateBonus(statesBonus[i].stateName, -statesBonus[i].stateValue);
        }
    }

    public override void OnItemReceive(Creature owner, int slotIndex)
    {
        base.OnItemReceive(owner, slotIndex);
        CalculateBonusStates(owner);
    }

    public override void OnItemLose(Creature owner, int slotIndex)
    {
        base.OnItemLose(owner, slotIndex);
        CancelBonusStates(owner);
    }

    public override void OnAttack(Creature owner, Creature attackedCreature, int slotIndex)
    {
        base.OnAttack(owner, attackedCreature, slotIndex);
        if (owner.autoattackDamageType == itemDamageType)
        {
            if (Random.Range(0f, 100f) < 20f)
                Effect.ApplyEffect(EffectsDatabase.GetEffectByID(1), attackedCreature, owner, 1, 1);
        }
    }
}
[System.Serializable]
public class ItemLeather : InventoryItem
{
    public ItemLeather(int ID) : base(ID)
    {
        itemNameRaw = "leather";
        itemDescription = "leather_desc";
        itemStats = "leather_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Component;
    }
}
[System.Serializable]
public class ItemThickenedLeather : InventoryItem
{
    public ItemThickenedLeather(int ID) : base(ID)
    {
        itemNameRaw = "thickened_leather";
        itemDescription = "thickened_leather_desc";
        itemStats = "thickened_leather_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Component;
    }
}
[System.Serializable]
public class ItemLeatherHelmet : InventoryItem
{
    public ItemLeatherHelmet(int ID) : base(ID)
    {
        itemNameRaw = "leather_helmet";
        itemDescription = "leather_helmet_desc";
        itemStats = "leather_helmet_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Equipment;

        statesBonus.Add(new ItemStateBonus("protection_blunt", 2));
        statesBonus.Add(new ItemStateBonus("protection_lashing", 1));
        statesBonus.Add(new ItemStateBonus("protection_freezing", 2));
    }

    public override void OnItemReceive(Creature owner, int slotIndex)
    {
        base.OnItemReceive(owner, slotIndex);
        if (owner != null && statesBonus != null && statesBonus.Count > 0 && owner.itemsGivingBonusesIDs.Contains(itemID) == false)
        {
            owner.itemsGivingBonusesIDs.Add(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                owner.ChangeStateBonus(statesBonus[i].stateName, statesBonus[i].stateValue);
        }
    }

    public override void OnItemLose(Creature owner, int slotIndex)
    {
        base.OnItemLose(owner, slotIndex);
        if (statesBonus != null && statesBonus.Count > 0 && owner.connectedInventory.GetItemsAmount(itemID) == 1)
        {
            owner.itemsGivingBonusesIDs.Remove(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                owner.ChangeStateBonus(statesBonus[i].stateName, -statesBonus[i].stateValue);
        }
    }
}
[System.Serializable]
public class ItemLeatherBreastplate : InventoryItem
{
    public ItemLeatherBreastplate(int ID) : base(ID)
    {
        itemNameRaw = "leather_breastplate";
        itemDescription = "leather_breastplate_desc";
        itemStats = "leather_breastplate_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Equipment;

        statesBonus.Add(new ItemStateBonus("protection_blunt", 3));
        statesBonus.Add(new ItemStateBonus("protection_lashing", 2));
        statesBonus.Add(new ItemStateBonus("protection_freezing", 2));
    }

    public override void OnItemReceive(Creature owner, int slotIndex)
    {
        base.OnItemReceive(owner, slotIndex);
        if (owner != null && statesBonus != null && statesBonus.Count > 0 && owner.itemsGivingBonusesIDs.Contains(itemID) == false)
        {
            owner.itemsGivingBonusesIDs.Add(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                owner.ChangeStateBonus(statesBonus[i].stateName, statesBonus[i].stateValue);
        }
    }

    public override void OnItemLose(Creature owner, int slotIndex)
    {
        base.OnItemLose(owner, slotIndex);
        if (statesBonus != null && statesBonus.Count > 0 && owner.connectedInventory.GetItemsAmount(itemID) == 1)
        {
            owner.itemsGivingBonusesIDs.Remove(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                owner.ChangeStateBonus(statesBonus[i].stateName, -statesBonus[i].stateValue);
        }
    }
}
[System.Serializable]
public class ItemSpellfulEssenceVial : InventoryItem
{
    public ItemSpellfulEssenceVial(int ID) : base(ID)
    {
        itemNameRaw = "spellful_essence_vial";
        itemDescription = "spellful_essence_vial_desc";
        itemStats = "spellful_essence_vial_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Component;
    }
}
[System.Serializable]
public class ItemSpellfulPowder : InventoryItem
{
    public ItemSpellfulPowder(int ID) : base(ID)
    {
        itemNameRaw = "spellful_powder";
        itemDescription = "spellful_powder_desc";
        itemStats = "spellful_powder_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Component;
    }
}
[System.Serializable]
public class ItemImpregnatedLeather : InventoryItem
{
    public ItemImpregnatedLeather(int ID) : base(ID)
    {
        itemNameRaw = "impregnated_leather";
        itemDescription = "impregnated_leather_desc";
        itemStats = "impregnated_leather_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Component;
    }
}
[System.Serializable]
public class ItemThickenedImpregnatedLeather : InventoryItem
{
    public ItemThickenedImpregnatedLeather(int ID) : base(ID)
    {
        itemNameRaw = "thickened_impregnated_leather";
        itemDescription = "thickened_impregnated_leather_desc";
        itemStats = "thickened_impregnated_leather_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Component;
    }
}
[System.Serializable]
public class ItemImpregnatedLeatherHelmet : InventoryItem
{
    public ItemImpregnatedLeatherHelmet(int ID) : base(ID)
    {
        itemNameRaw = "impregnated_leather_helmet";
        itemDescription = "impregnated_leather_helmet_desc";
        itemStats = "impregnated_leather_helmet_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Equipment;

        statesBonus.Add(new ItemStateBonus("protection_blunt", 2));
        statesBonus.Add(new ItemStateBonus("protection_lashing", 1));
        statesBonus.Add(new ItemStateBonus("protection_freezing", 2));
        statesBonus.Add(new ItemStateBonus("protection_electric", 1));
        statesBonus.Add(new ItemStateBonus("protection_scalding", 1));
        statesBonus.Add(new ItemStateBonus("protection_poison", 1));
        statesBonus.Add(new ItemStateBonus("spell_power", 1));
    }

    public override void OnItemReceive(Creature owner, int slotIndex)
    {
        base.OnItemReceive(owner, slotIndex);
        if (owner != null && statesBonus != null && statesBonus.Count > 0 && owner.itemsGivingBonusesIDs.Contains(itemID) == false)
        {
            owner.itemsGivingBonusesIDs.Add(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                owner.ChangeStateBonus(statesBonus[i].stateName, statesBonus[i].stateValue);
        }
    }

    public override void OnItemLose(Creature owner, int slotIndex)
    {
        base.OnItemLose(owner, slotIndex);
        if (statesBonus != null && statesBonus.Count > 0 && owner.connectedInventory.GetItemsAmount(itemID) == 1)
        {
            owner.itemsGivingBonusesIDs.Remove(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                owner.ChangeStateBonus(statesBonus[i].stateName, -statesBonus[i].stateValue);
        }
    }
}
[System.Serializable]
public class ItemImpregnatedLeatherBreastplate : InventoryItem
{
    public ItemImpregnatedLeatherBreastplate(int ID) : base(ID)
    {
        itemNameRaw = "impregnated_leather_breastplate";
        itemDescription = "impregnated_leather_breastplate_desc";
        itemStats = "impregnated_leather_breastplate_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Equipment;

        statesBonus.Add(new ItemStateBonus("protection_blunt", 3));
        statesBonus.Add(new ItemStateBonus("protection_lashing", 2));
        statesBonus.Add(new ItemStateBonus("protection_freezing", 2));
        statesBonus.Add(new ItemStateBonus("protection_electric", 1));
        statesBonus.Add(new ItemStateBonus("protection_scalding", 1));
        statesBonus.Add(new ItemStateBonus("protection_poison", 1));
        statesBonus.Add(new ItemStateBonus("spell_power", 2));
    }

    public override void OnItemReceive(Creature owner, int slotIndex)
    {
        base.OnItemReceive(owner, slotIndex);
        if (owner != null && statesBonus != null && statesBonus.Count > 0 && owner.itemsGivingBonusesIDs.Contains(itemID) == false)
        {
            owner.itemsGivingBonusesIDs.Add(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                owner.ChangeStateBonus(statesBonus[i].stateName, statesBonus[i].stateValue);
        }
    }

    public override void OnItemLose(Creature owner, int slotIndex)
    {
        base.OnItemLose(owner, slotIndex);
        if (statesBonus != null && statesBonus.Count > 0 && owner.connectedInventory.GetItemsAmount(itemID) == 1)
        {
            owner.itemsGivingBonusesIDs.Remove(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                owner.ChangeStateBonus(statesBonus[i].stateName, -statesBonus[i].stateValue);
        }
    }
}
[System.Serializable]
public class ItemImpregnatedLeatherGloveLeft : InventoryItem
{
    public ItemImpregnatedLeatherGloveLeft(int ID) : base(ID)
    {
        itemNameRaw = "impregnated_leather_glove_left";
        itemDescription = "impregnated_leather_glove_left_desc";
        itemStats = "impregnated_leather_glove_left_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Equipment;

        statesBonus.Add(new ItemStateBonus("protection_blunt", 1));
        statesBonus.Add(new ItemStateBonus("protection_lashing", 1));
        statesBonus.Add(new ItemStateBonus("protection_freezing", 1));
        statesBonus.Add(new ItemStateBonus("protection_electric", 1));
        statesBonus.Add(new ItemStateBonus("protection_scalding", 1));
        statesBonus.Add(new ItemStateBonus("protection_poison", 1));
        statesBonus.Add(new ItemStateBonus("spell_power", 1));
    }

    public override void OnItemReceive(Creature owner, int slotIndex)
    {
        base.OnItemReceive(owner, slotIndex);
        if (owner != null && statesBonus != null && statesBonus.Count > 0 && owner.itemsGivingBonusesIDs.Contains(itemID) == false)
        {
            owner.itemsGivingBonusesIDs.Add(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                owner.ChangeStateBonus(statesBonus[i].stateName, statesBonus[i].stateValue);
        }
    }

    public override void OnItemLose(Creature owner, int slotIndex)
    {
        base.OnItemLose(owner, slotIndex);
        if (statesBonus != null && statesBonus.Count > 0 && owner.connectedInventory.GetItemsAmount(itemID) == 1)
        {
            owner.itemsGivingBonusesIDs.Remove(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                owner.ChangeStateBonus(statesBonus[i].stateName, -statesBonus[i].stateValue);
        }
    }
}
[System.Serializable]
public class ItemImpregnatedLeatherGloveRight : InventoryItem
{
    public ItemImpregnatedLeatherGloveRight(int ID) : base(ID)
    {
        itemNameRaw = "impregnated_leather_glove_right";
        itemDescription = "impregnated_leather_glove_right_desc";
        itemStats = "impregnated_leather_glove_right_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Equipment;

        statesBonus.Add(new ItemStateBonus("protection_blunt", 1));
        statesBonus.Add(new ItemStateBonus("protection_lashing", 1));
        statesBonus.Add(new ItemStateBonus("protection_freezing", 1));
        statesBonus.Add(new ItemStateBonus("protection_electric", 1));
        statesBonus.Add(new ItemStateBonus("protection_scalding", 1));
        statesBonus.Add(new ItemStateBonus("protection_poison", 1));
        statesBonus.Add(new ItemStateBonus("spell_power", 1));
    }

    public override void OnItemReceive(Creature owner, int slotIndex)
    {
        base.OnItemReceive(owner, slotIndex);
        if (owner != null && statesBonus != null && statesBonus.Count > 0 && owner.itemsGivingBonusesIDs.Contains(itemID) == false)
        {
            owner.itemsGivingBonusesIDs.Add(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                owner.ChangeStateBonus(statesBonus[i].stateName, statesBonus[i].stateValue);
        }
    }

    public override void OnItemLose(Creature owner, int slotIndex)
    {
        base.OnItemLose(owner, slotIndex);
        if (statesBonus != null && statesBonus.Count > 0 && owner.connectedInventory.GetItemsAmount(itemID) == 1)
        {
            owner.itemsGivingBonusesIDs.Remove(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                owner.ChangeStateBonus(statesBonus[i].stateName, -statesBonus[i].stateValue);
        }
    }
}
[System.Serializable]
public class ItemTorch : InventoryItem
{
    public ItemTorch(int ID) : base(ID)
    {
        itemNameRaw = "torch";
        itemDescription = "torch_desc";
        itemStats = "torch_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemDamageType = DamageType.scalding;
        itemType = ItemType.Weapon;

        statesBonus.Add(new ItemStateBonus("attack_damage", 8));
        statesBonus.Add(new ItemStateBonus("view_dist", 2));
    }

    public override void CalculateBonusStates(Creature owner)
    {
        base.CalculateBonusStates(owner);
        if (owner != null && statesBonus != null && statesBonus.Count > 0 && owner.itemsGivingBonusesIDs.Contains(itemID) == false)
        {
            owner.itemsGivingBonusesIDs.Add(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                if ((statesBonus[i].stateName == "attack_damage" && owner.autoattackDamageType == itemDamageType) || statesBonus[i].stateName != "attack_damage")
                    owner.ChangeStateBonus(statesBonus[i].stateName, statesBonus[i].stateValue);
        }
    }

    public override void CancelBonusStates(Creature owner)
    {
        base.CalculateBonusStates(owner);
        if (owner != null && statesBonus != null && statesBonus.Count > 0 && owner.connectedInventory.GetItemsAmount(itemID) == 1)
        {
            owner.itemsGivingBonusesIDs.Remove(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                if ((statesBonus[i].stateName == "attack_damage" && owner.autoattackDamageType == itemDamageType) || statesBonus[i].stateName != "attack_damage")
                    owner.ChangeStateBonus(statesBonus[i].stateName, -statesBonus[i].stateValue);
        }
    }

    public override void OnItemReceive(Creature owner, int slotIndex)
    {
        base.OnItemReceive(owner, slotIndex);
        CalculateBonusStates(owner);
    }

    public override void OnItemLose(Creature owner, int slotIndex)
    {
        base.OnItemLose(owner, slotIndex);
        CancelBonusStates(owner);
    }

    public override void OnAttack(Creature owner, Creature attackedCreature, int slotIndex)
    {
        base.OnAttack(owner, attackedCreature, slotIndex);
        if (owner.autoattackDamageType == itemDamageType)
        {
            if (Random.Range(0f, 100f) < 15f)
                Effect.ApplyEffect(EffectsDatabase.GetEffectByID(3), attackedCreature, owner, 2, 1);
        }
    }
}
[System.Serializable]
public class ItemLittleInkwell : InventoryItem
{
    public ItemLittleInkwell(int ID) : base(ID)
    {
        itemNameRaw = "little_inkwell";
        itemDescription = "little_inkwell_desc";
        itemStats = "little_inkwell_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "little_inkwell_empty");
        itemType = ItemType.Component;
    }
}
[System.Serializable]
public class ItemBoneNeedle : InventoryItem
{
    public ItemBoneNeedle(int ID) : base(ID)
    {
        itemNameRaw = "bone_needle";
        itemDescription = "bone_needle_desc";
        itemStats = "bone_needle_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Component;
    }
}
[System.Serializable]
public class ItemGlassLens : InventoryItem
{
    public ItemGlassLens(int ID) : base(ID)
    {
        itemNameRaw = "glass_lens";
        itemDescription = "glass_lens_desc";
        itemStats = "glass_lens_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Component;
    }

    public override void Use(Creature owner, int slotIndex)
    {
        base.Use(owner, slotIndex);
        if (owner.GetComponent<PlayerController>() != null)
        {
            PlayerController player = owner.GetComponent<PlayerController>();
            //player.DeselectVisualSlots();
            player.inExploreMode = true;
            player.visualSlots[0].descriptionWindow.SetActive(false);
            player.visualSlots[0].removeItemButton.SetActive(false);
            player.visualSlots[0].useItemButton.SetActive(false);
        }
    }
}
[System.Serializable]
public class ItemScrollSpellBrightening : InventoryItem
{
    public ItemScrollSpellBrightening(int ID) : base(ID)
    {
        itemNameRaw = "scroll_spell_brightening";
        itemDescription = "scroll_spell_brightening_desc";
        itemStats = "spell_brightening_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "light_spell_scroll");
        itemType = ItemType.Component;

        recipeExplorationResultID = 31;
    }

    public override void Use(Creature owner, int slotIndex)
    {
        base.Use(owner, slotIndex);
        if (owner.GetComponent<PlayerController>() != null)
        {
            PlayerController player = owner.GetComponent<PlayerController>();
            player.DeselectVisualSlots();
            Spell spellToCast = SpellsDatabase.GetSpellByID(0);
            spellToCast.spellLevel = owner.GetStateValue("spell_power");
            player.StartCastingSpell(spellToCast, slotIndex);
        }
    }
}
[System.Serializable]
public class ItemInkSpellBrightening : InventoryItem
{
    public ItemInkSpellBrightening(int ID) : base(ID)
    {
        itemNameRaw = "ink_spell_brightening";
        itemDescription = "ink_spell_brightening_desc";
        itemStats = "ink_spell_brightening_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "little_inkwell");
        itemType = ItemType.Component;

        recipeShouldBeExplored = true;
    }
}
[System.Serializable]
public class ItemScrollSpellIceBoulder : InventoryItem
{
    int spellID = 1;
    public ItemScrollSpellIceBoulder(int ID) : base(ID)
    {
        itemNameRaw = "scroll_spell_ice_boulder";
        itemDescription = "scroll_spell_ice_boulder_desc";
        itemStats = "spell_ice_boulder_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "ice_spell_scroll");
        itemType = ItemType.Component;

        recipeExplorationResultID = 33;
    }

    public override void Use(Creature owner, int slotIndex)
    {
        base.Use(owner, slotIndex);
        if (owner.GetComponent<PlayerController>() != null)
        {
            PlayerController player = owner.GetComponent<PlayerController>();
            player.DeselectVisualSlots();
            Spell spellToCast = SpellsDatabase.GetSpellByID(spellID);
            spellToCast.spellLevel = owner.GetStateValue("spell_power");
            player.StartCastingSpell(spellToCast, slotIndex);
        }
    }

    public override string[] CalculateItemDescriptionParts(Creature owner)
    {
        base.CalculateItemDescriptionParts(owner);
        int spellPowerBonus = 0;
        for (int i = 0; i < SpellsDatabase.GetSpellByID(spellID).spellTypes.Length; i++)
            spellPowerBonus += owner.GetStateValue("spell_power_bonus_" + SpellsDatabase.GetSpellByID(spellID).spellTypes[i]);
        return new string[1] { (9 + 35 * (owner.GetStateValue("spell_power") + spellPowerBonus)).ToString() };
    }
}
[System.Serializable]
public class ItemInkSpellIceBoulder : InventoryItem
{
    public ItemInkSpellIceBoulder(int ID) : base(ID)
    {
        itemNameRaw = "ink_spell_ice_boulder";
        itemDescription = "ink_spell_ice_boulder_desc";
        itemStats = "ink_spell_ice_boulder_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "little_inkwell");
        itemType = ItemType.Component;

        recipeShouldBeExplored = true;
    }
}
[System.Serializable]
public class ItemGlowingSprout : InventoryItem
{
    public ItemGlowingSprout(int ID) : base(ID)
    {
        itemNameRaw = "glowing_sprout";
        itemDescription = "glowing_sprout_desc";
        itemStats = "glowing_sprout_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Component;
    }
}
[System.Serializable]
public class ItemOldPaper : InventoryItem
{
    public ItemOldPaper(int ID) : base(ID)
    {
        itemNameRaw = "old_paper";
        itemDescription = "old_paper_desc";
        itemStats = "old_paper_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Component;
    }
}
[System.Serializable]
public class ItemIcicle : InventoryItem
{
    public ItemIcicle(int ID) : base(ID)
    {
        itemNameRaw = "icicle";
        itemDescription = "icicle_desc";
        itemStats = "icicle_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Component;
    }
}
[System.Serializable]
public class ItemScrollSpellEnergyStrike : InventoryItem
{
    int spellID = 2;
    public ItemScrollSpellEnergyStrike(int ID) : base(ID)
    {
        itemNameRaw = "scroll_spell_energy_strike";
        itemDescription = "scroll_spell_energy_strike_desc";
        itemStats = "spell_energy_strike_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "energy_spell_scroll");
        itemType = ItemType.Component;

        recipeExplorationResultID = 38;
    }

    public override void Use(Creature owner, int slotIndex)
    {
        base.Use(owner, slotIndex);
        if (owner.GetComponent<PlayerController>() != null)
        {
            PlayerController player = owner.GetComponent<PlayerController>();
            player.DeselectVisualSlots();
            Spell spellToCast = SpellsDatabase.GetSpellByID(spellID);
            spellToCast.spellLevel = owner.GetStateValue("spell_power");
            player.StartCastingSpell(spellToCast, slotIndex);
        }
    }

    public override string[] CalculateItemDescriptionParts(Creature owner)
    {
        base.CalculateItemDescriptionParts(owner);
        int spellPowerBonus = 0;
        for (int i = 0; i < SpellsDatabase.GetSpellByID(spellID).spellTypes.Length; i++)
            spellPowerBonus += owner.GetStateValue("spell_power_bonus_" + SpellsDatabase.GetSpellByID(spellID).spellTypes[i]);
        return new string[1] { (5 + Mathf.RoundToInt(4.5f * (owner.GetStateValue("spell_power") + spellPowerBonus))).ToString() };
    }
}
[System.Serializable]
public class ItemInkSpellEnergyStrike : InventoryItem
{
    public ItemInkSpellEnergyStrike(int ID) : base(ID)
    {
        itemNameRaw = "ink_spell_energy_strike";
        itemDescription = "ink_spell_energy_strike_desc";
        itemStats = "ink_spell_energy_strike_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "little_inkwell");
        itemType = ItemType.Component;

        recipeShouldBeExplored = true;
    }
}
[System.Serializable]
public class ItemComprehensionBook : InventoryItem
{
    public ItemComprehensionBook(int ID) : base(ID)
    {
        itemNameRaw = "comprehension_book";
        itemDescription = "comprehension_book_desc";
        itemStats = "comprehension_book_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Component;

        statesBonus.Add(new ItemStateBonus("spell_power", 10));
    }

    public override void OnItemReceive(Creature owner, int slotIndex)
    {
        base.OnItemReceive(owner, slotIndex);
        if (owner != null && statesBonus != null && statesBonus.Count > 0 && owner.itemsGivingBonusesIDs.Contains(itemID) == false)
        {
            owner.itemsGivingBonusesIDs.Add(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                owner.ChangeStateBonus(statesBonus[i].stateName, statesBonus[i].stateValue);
        }
    }

    public override void OnItemLose(Creature owner, int slotIndex)
    {
        base.OnItemLose(owner, slotIndex);
        if (statesBonus != null && statesBonus.Count > 0 && owner.connectedInventory.GetItemsAmount(itemID) == 1)
        {
            owner.itemsGivingBonusesIDs.Remove(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                owner.ChangeStateBonus(statesBonus[i].stateName, -statesBonus[i].stateValue);
        }
    }
}
[System.Serializable]
public class ItemIcyDirk : InventoryItem
{
    public ItemIcyDirk(int ID) : base(ID)
    {
        itemNameRaw = "icy_dirk";
        itemDescription = "icy_dirk_desc";
        itemStats = "icy_dirk_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemDamageType = DamageType.freezing;
        itemType = ItemType.Weapon;

        statesBonus.Add(new ItemStateBonus("attack_damage", 20));
    }

    public override void CalculateBonusStates(Creature owner)
    {
        base.CalculateBonusStates(owner);
        if (owner != null && statesBonus != null && statesBonus.Count > 0 && owner.itemsGivingBonusesIDs.Contains(itemID) == false)
        {
            owner.itemsGivingBonusesIDs.Add(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                if ((statesBonus[i].stateName == "attack_damage" && owner.autoattackDamageType == itemDamageType) || statesBonus[i].stateName != "attack_damage")
                    owner.ChangeStateBonus(statesBonus[i].stateName, statesBonus[i].stateValue);
        }
    }

    public override void CancelBonusStates(Creature owner)
    {
        base.CalculateBonusStates(owner);
        if (owner != null && statesBonus != null && statesBonus.Count > 0 && owner.connectedInventory.GetItemsAmount(itemID) == 1)
        {
            owner.itemsGivingBonusesIDs.Remove(itemID);
            for (int i = 0; i < statesBonus.Count; i++)
                if ((statesBonus[i].stateName == "attack_damage" && owner.autoattackDamageType == itemDamageType) || statesBonus[i].stateName != "attack_damage")
                    owner.ChangeStateBonus(statesBonus[i].stateName, -statesBonus[i].stateValue);
        }
    }

    public override void OnItemReceive(Creature owner, int slotIndex)
    {
        base.OnItemReceive(owner, slotIndex);
        CalculateBonusStates(owner);
    }

    public override void OnItemLose(Creature owner, int slotIndex)
    {
        base.OnItemLose(owner, slotIndex);
        CancelBonusStates(owner);
    }

    public override void OnAttack(Creature owner, Creature attackedCreature, int slotIndex)
    {
        base.OnAttack(owner, attackedCreature, slotIndex);
        if (owner.autoattackDamageType == itemDamageType)
        {
            Effect.ApplyEffect(EffectsDatabase.GetEffectByID(5), attackedCreature, owner, 1, 1);
        }
    }
}
[System.Serializable]
public class ItemIceChunk : InventoryItem
{
    public ItemIceChunk(int ID) : base(ID)
    {
        itemNameRaw = "ice_chunk";
        itemDescription = "ice_chunk_desc";
        itemStats = "ice_chunk_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + itemNameRaw);
        itemType = ItemType.Component;
    }
}
[System.Serializable]
public class ItemScrollSpellIceRing : InventoryItem
{
    int spellID = 3;
    public ItemScrollSpellIceRing(int ID) : base(ID)
    {
        itemNameRaw = "scroll_spell_ice_ring";
        itemDescription = "scroll_spell_ice_ring_desc";
        itemStats = "spell_ice_ring_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "ice_spell_scroll");
        itemType = ItemType.Component;

        recipeExplorationResultID = 43;
    }

    public override void Use(Creature owner, int slotIndex)
    {
        base.Use(owner, slotIndex);
        if (owner.GetComponent<PlayerController>() != null)
        {
            PlayerController player = owner.GetComponent<PlayerController>();
            player.DeselectVisualSlots();
            Spell spellToCast = SpellsDatabase.GetSpellByID(spellID);
            spellToCast.spellLevel = owner.GetStateValue("spell_power");
            player.StartCastingSpell(spellToCast, slotIndex);
        }
    }

    public override string[] CalculateItemDescriptionParts(Creature owner)
    {
        base.CalculateItemDescriptionParts(owner);
        int spellPowerBonus = 0;
        for (int i = 0; i < SpellsDatabase.GetSpellByID(spellID).spellTypes.Length; i++)
            spellPowerBonus += owner.GetStateValue("spell_power_bonus_" + SpellsDatabase.GetSpellByID(spellID).spellTypes[i]);
        return new string[1] { (5 + 19 * (owner.GetStateValue("spell_power") + spellPowerBonus)).ToString() };
    }
}
[System.Serializable]
public class ItemInkSpellIceRing : InventoryItem
{
    public ItemInkSpellIceRing(int ID) : base(ID)
    {
        itemNameRaw = "ink_spell_ice_ring";
        itemDescription = "ink_spell_ice_ring_desc";
        itemStats = "ink_spell_ice_ring_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "little_inkwell");
        itemType = ItemType.Component;

        recipeShouldBeExplored = true;
    }
}
[System.Serializable]
public class ItemScrollSpellIcing : InventoryItem
{
    int spellID = 4;
    public ItemScrollSpellIcing(int ID) : base(ID)
    {
        itemNameRaw = "scroll_spell_icing";
        itemDescription = "scroll_spell_icing_desc";
        itemStats = "spell_icing_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "ice_spell_scroll");
        itemType = ItemType.Component;

        recipeExplorationResultID = 45;
    }

    public override void Use(Creature owner, int slotIndex)
    {
        base.Use(owner, slotIndex);
        if (owner.GetComponent<PlayerController>() != null)
        {
            PlayerController player = owner.GetComponent<PlayerController>();
            player.DeselectVisualSlots();
            Spell spellToCast = SpellsDatabase.GetSpellByID(spellID);
            spellToCast.spellLevel = owner.GetStateValue("spell_power");
            player.StartCastingSpell(spellToCast, slotIndex);
        }
    }

    public override string[] CalculateItemDescriptionParts(Creature owner)
    {
        base.CalculateItemDescriptionParts(owner);
        int spellPowerBonus = 0;
        for (int i = 0; i < SpellsDatabase.GetSpellByID(spellID).spellTypes.Length; i++)
            spellPowerBonus += owner.GetStateValue("spell_power_bonus_" + SpellsDatabase.GetSpellByID(spellID).spellTypes[i]);
        return new string[2] { (7 + 13 * (owner.GetStateValue("spell_power") + spellPowerBonus)).ToString(), ((owner.GetStateValue("spell_power") + spellPowerBonus) / 23 + 1).ToString() };
    }
}
[System.Serializable]
public class ItemInkSpellIcing : InventoryItem
{
    public ItemInkSpellIcing(int ID) : base(ID)
    {
        itemNameRaw = "ink_spell_icing";
        itemDescription = "ink_spell_icing_desc";
        itemStats = "ink_spell_icing_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "little_inkwell");
        itemType = ItemType.Component;

        recipeShouldBeExplored = true;
    }
}
[System.Serializable]
public class ItemScrollSpellFrostbite : InventoryItem
{
    int spellID = 5;
    public ItemScrollSpellFrostbite(int ID) : base(ID)
    {
        itemNameRaw = "scroll_spell_frostbite";
        itemDescription = "scroll_spell_frostbite_desc";
        itemStats = "spell_frostbite_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "ice_spell_scroll");
        itemType = ItemType.Component;

        recipeExplorationResultID = 47;
    }

    public override void Use(Creature owner, int slotIndex)
    {
        base.Use(owner, slotIndex);
        if (owner.GetComponent<PlayerController>() != null)
        {
            PlayerController player = owner.GetComponent<PlayerController>();
            player.DeselectVisualSlots();
            Spell spellToCast = SpellsDatabase.GetSpellByID(spellID);
            spellToCast.spellLevel = owner.GetStateValue("spell_power");
            player.StartCastingSpell(spellToCast, slotIndex);
        }
    }

    public override string[] CalculateItemDescriptionParts(Creature owner)
    {
        base.CalculateItemDescriptionParts(owner);
        int spellPowerBonus = 0;
        for (int i = 0; i < SpellsDatabase.GetSpellByID(spellID).spellTypes.Length; i++)
            spellPowerBonus += owner.GetStateValue("spell_power_bonus_" + SpellsDatabase.GetSpellByID(spellID).spellTypes[i]);
        return new string[3] { ((owner.GetStateValue("spell_power") + spellPowerBonus) / 17 + 1).ToString(), (10 + 8 * (owner.GetStateValue("spell_power") + spellPowerBonus)).ToString(), ((owner.GetStateValue("spell_power") + spellPowerBonus) / 21 + 1).ToString() };
    }
}
[System.Serializable]
public class ItemInkSpellFrostbite : InventoryItem
{
    public ItemInkSpellFrostbite(int ID) : base(ID)
    {
        itemNameRaw = "ink_spell_frostbite";
        itemDescription = "ink_spell_frostbite_desc";
        itemStats = "ink_spell_frostbite_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "little_inkwell");
        itemType = ItemType.Component;

        recipeShouldBeExplored = true;
    }
}
[System.Serializable]
public class ItemScrollSpellSummonIceGolem : InventoryItem
{
    int spellID = 6;
    public ItemScrollSpellSummonIceGolem(int ID) : base(ID)
    {
        itemNameRaw = "scroll_spell_summon_ice_golem";
        itemDescription = "scroll_spell_summon_ice_golem_desc";
        itemStats = "spell_summon_ice_golem_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "ice_spell_scroll");
        itemType = ItemType.Component;

        recipeExplorationResultID = 49;
    }

    public override void Use(Creature owner, int slotIndex)
    {
        base.Use(owner, slotIndex);
        if (owner.GetComponent<PlayerController>() != null)
        {
            PlayerController player = owner.GetComponent<PlayerController>();
            player.DeselectVisualSlots();
            Spell spellToCast = SpellsDatabase.GetSpellByID(spellID);
            spellToCast.spellLevel = owner.GetStateValue("spell_power");
            player.StartCastingSpell(spellToCast, slotIndex);
        }
    }

    public override string[] CalculateItemDescriptionParts(Creature owner)
    {
        base.CalculateItemDescriptionParts(owner);
        int spellPowerBonus = 0;
        for (int i = 0; i < SpellsDatabase.GetSpellByID(spellID).spellTypes.Length; i++)
            spellPowerBonus += owner.GetStateValue("spell_power_bonus_" + SpellsDatabase.GetSpellByID(spellID).spellTypes[i]);
        return new string[3] { (20 + 30 * (owner.GetStateValue("spell_power") + spellPowerBonus)).ToString(), ((owner.GetStateValue("spell_power") + spellPowerBonus) / 27 + 1).ToString(), (17 + 17 * (owner.GetStateValue("spell_power") + spellPowerBonus)).ToString() };
    }
}
[System.Serializable]
public class ItemInkSpellSummonIceGolem : InventoryItem
{
    public ItemInkSpellSummonIceGolem(int ID) : base(ID)
    {
        itemNameRaw = "ink_spell_summon_ice_golem";
        itemDescription = "ink_spell_summon_ice_golem_desc";
        itemStats = "ink_spell_summon_ice_golem_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "little_inkwell");
        itemType = ItemType.Component;

        recipeShouldBeExplored = true;
    }
}
[System.Serializable]
public class ItemScrollSpellFrozenShield : InventoryItem
{
    int spellID = 7;
    public ItemScrollSpellFrozenShield(int ID) : base(ID)
    {
        itemNameRaw = "scroll_spell_frozen_shield";
        itemDescription = "scroll_spell_frozen_shield_desc";
        itemStats = "spell_frozen_shield_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "ice_spell_scroll");
        itemType = ItemType.Component;

        recipeExplorationResultID = 51;
    }

    public override void Use(Creature owner, int slotIndex)
    {
        base.Use(owner, slotIndex);
        if (owner.GetComponent<PlayerController>() != null)
        {
            PlayerController player = owner.GetComponent<PlayerController>();
            player.DeselectVisualSlots();
            Spell spellToCast = SpellsDatabase.GetSpellByID(spellID);
            spellToCast.spellLevel = owner.GetStateValue("spell_power");
            player.StartCastingSpell(spellToCast, slotIndex);
        }
    }

    public override string[] CalculateItemDescriptionParts(Creature owner)
    {
        base.CalculateItemDescriptionParts(owner);
        int spellPowerBonus = 0;
        for (int i = 0; i < SpellsDatabase.GetSpellByID(spellID).spellTypes.Length; i++)
            spellPowerBonus += owner.GetStateValue("spell_power_bonus_" + SpellsDatabase.GetSpellByID(spellID).spellTypes[i]);
        return new string[2] { ((owner.GetStateValue("spell_power") + spellPowerBonus) / 23 + 1).ToString(), (3 + 5 * (owner.GetStateValue("spell_power") + spellPowerBonus)).ToString() };
    }
}
[System.Serializable]
public class ItemInkSpellFrozenShield : InventoryItem
{
    public ItemInkSpellFrozenShield(int ID) : base(ID)
    {
        itemNameRaw = "ink_spell_frozen_shield";
        itemDescription = "ink_spell_frozen_shield_desc";
        itemStats = "ink_spell_frozen_shield_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "little_inkwell");
        itemType = ItemType.Component;

        recipeShouldBeExplored = true;
    }
}
[System.Serializable]
public class ItemScrollSpellSummonIceCrystal : InventoryItem
{
    int spellID = 8;
    public ItemScrollSpellSummonIceCrystal(int ID) : base(ID)
    {
        itemNameRaw = "scroll_spell_summon_ice_crystal";
        itemDescription = "scroll_spell_summon_ice_crystal_desc";
        itemStats = "spell_summon_ice_crystal_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "ice_spell_scroll");
        itemType = ItemType.Component;

        recipeExplorationResultID = 53;
    }

    public override void Use(Creature owner, int slotIndex)
    {
        base.Use(owner, slotIndex);
        if (owner.GetComponent<PlayerController>() != null)
        {
            PlayerController player = owner.GetComponent<PlayerController>();
            player.DeselectVisualSlots();
            Spell spellToCast = SpellsDatabase.GetSpellByID(spellID);
            spellToCast.spellLevel = owner.GetStateValue("spell_power");
            player.StartCastingSpell(spellToCast, slotIndex);
        }
    }

    public override string[] CalculateItemDescriptionParts(Creature owner)
    {
        base.CalculateItemDescriptionParts(owner);
        int spellPowerBonus = 0;
        for (int i = 0; i < SpellsDatabase.GetSpellByID(spellID).spellTypes.Length; i++)
            spellPowerBonus += owner.GetStateValue("spell_power_bonus_" + SpellsDatabase.GetSpellByID(spellID).spellTypes[i]);
        return new string[1] { ((owner.GetStateValue("spell_power") + spellPowerBonus) / 8 + 1).ToString() };
    }
}
[System.Serializable]
public class ItemInkSpellSummonIceCrystal : InventoryItem
{
    public ItemInkSpellSummonIceCrystal(int ID) : base(ID)
    {
        itemNameRaw = "ink_spell_summon_ice_crystal";
        itemDescription = "ink_spell_summon_ice_crystal_desc";
        itemStats = "ink_spell_summon_ice_crystal_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "little_inkwell");
        itemType = ItemType.Component;

        recipeShouldBeExplored = true;
    }
}
[System.Serializable]
public class ItemScrollSpellChillingWind : InventoryItem
{
    int spellID = 9;
    public ItemScrollSpellChillingWind(int ID) : base(ID)
    {
        itemNameRaw = "scroll_spell_chilling_wind";
        itemDescription = "scroll_spell_chilling_wind_desc";
        itemStats = "spell_chilling_wind_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "ice_spell_scroll");
        itemType = ItemType.Component;

        recipeExplorationResultID = 55;
    }

    public override void Use(Creature owner, int slotIndex)
    {
        base.Use(owner, slotIndex);
        if (owner.GetComponent<PlayerController>() != null)
        {
            PlayerController player = owner.GetComponent<PlayerController>();
            player.DeselectVisualSlots();
            Spell spellToCast = SpellsDatabase.GetSpellByID(spellID);
            spellToCast.spellLevel = owner.GetStateValue("spell_power");
            player.StartCastingSpell(spellToCast, slotIndex);
        }
    }

    public override string[] CalculateItemDescriptionParts(Creature owner)
    {
        base.CalculateItemDescriptionParts(owner);
        int spellPowerBonus = 0;
        for (int i = 0; i < SpellsDatabase.GetSpellByID(spellID).spellTypes.Length; i++)
            spellPowerBonus += owner.GetStateValue("spell_power_bonus_" + SpellsDatabase.GetSpellByID(spellID).spellTypes[i]);
        return new string[2] { ((owner.GetStateValue("spell_power") + spellPowerBonus) / 12 + 2).ToString(), (5 + 15 * (owner.GetStateValue("spell_power") + spellPowerBonus)).ToString() };
    }
}
[System.Serializable]
public class ItemInkSpellChillingWind : InventoryItem
{
    public ItemInkSpellChillingWind(int ID) : base(ID)
    {
        itemNameRaw = "ink_spell_chilling_wind";
        itemDescription = "ink_spell_chilling_wind_desc";
        itemStats = "ink_spell_chilling_wind_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "little_inkwell");
        itemType = ItemType.Component;

        recipeShouldBeExplored = true;
    }
}
[System.Serializable]
public class ItemScrollSpellChills : InventoryItem
{
    int spellID = 10;
    public ItemScrollSpellChills(int ID) : base(ID)
    {
        itemNameRaw = "scroll_spell_chills";
        itemDescription = "scroll_spell_chills_desc";
        itemStats = "spell_chills_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "ice_spell_scroll");
        itemType = ItemType.Component;

        recipeExplorationResultID = 57;
    }

    public override void Use(Creature owner, int slotIndex)
    {
        base.Use(owner, slotIndex);
        if (owner.GetComponent<PlayerController>() != null)
        {
            PlayerController player = owner.GetComponent<PlayerController>();
            player.DeselectVisualSlots();
            Spell spellToCast = SpellsDatabase.GetSpellByID(spellID);
            spellToCast.spellLevel = owner.GetStateValue("spell_power");
            player.StartCastingSpell(spellToCast, slotIndex);
        }
    }

    public override string[] CalculateItemDescriptionParts(Creature owner)
    {
        base.CalculateItemDescriptionParts(owner);
        int spellPowerBonus = 0;
        for (int i = 0; i < SpellsDatabase.GetSpellByID(spellID).spellTypes.Length; i++)
            spellPowerBonus += owner.GetStateValue("spell_power_bonus_" + SpellsDatabase.GetSpellByID(spellID).spellTypes[i]);
        return new string[2] { ((owner.GetStateValue("spell_power") + spellPowerBonus) / 4 + 1).ToString(), ((owner.GetStateValue("spell_power") + spellPowerBonus) / 19 + 1).ToString() };
    }
}
[System.Serializable]
public class ItemInkSpellChills : InventoryItem
{
    public ItemInkSpellChills(int ID) : base(ID)
    {
        itemNameRaw = "ink_spell_chills";
        itemDescription = "ink_spell_chills_desc";
        itemStats = "ink_spell_chills_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "little_inkwell");
        itemType = ItemType.Component;

        recipeShouldBeExplored = true;
    }
}
[System.Serializable]
public class ItemScrollSpellGlacialWisdom : InventoryItem
{
    int spellID = 11;
    public ItemScrollSpellGlacialWisdom(int ID) : base(ID)
    {
        itemNameRaw = "scroll_spell_glacial_wisdom";
        itemDescription = "scroll_spell_glacial_wisdom_desc";
        itemStats = "spell_glacial_wisdom_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "ice_spell_scroll");
        itemType = ItemType.Component;

        recipeExplorationResultID = 59;
    }

    public override void Use(Creature owner, int slotIndex)
    {
        base.Use(owner, slotIndex);
        if (owner.GetComponent<PlayerController>() != null)
        {
            PlayerController player = owner.GetComponent<PlayerController>();
            player.DeselectVisualSlots();
            Spell spellToCast = SpellsDatabase.GetSpellByID(spellID);
            spellToCast.spellLevel = owner.GetStateValue("spell_power");
            player.StartCastingSpell(spellToCast, slotIndex);
        }
    }

    public override string[] CalculateItemDescriptionParts(Creature owner)
    {
        base.CalculateItemDescriptionParts(owner);
        int spellPowerBonus = 0;
        for (int i = 0; i < SpellsDatabase.GetSpellByID(spellID).spellTypes.Length; i++)
            spellPowerBonus += owner.GetStateValue("spell_power_bonus_" + SpellsDatabase.GetSpellByID(spellID).spellTypes[i]);
        return new string[2] { ((owner.GetStateValue("spell_power") + spellPowerBonus) / 4 + 1).ToString(), ((owner.GetStateValue("spell_power") + spellPowerBonus) / 21 + 1).ToString() };
    }
}
[System.Serializable]
public class ItemInkSpellGlacialWisdom : InventoryItem
{
    public ItemInkSpellGlacialWisdom(int ID) : base(ID)
    {
        itemNameRaw = "ink_spell_glacial_wisdom";
        itemDescription = "ink_spell_glacial_wisdom_desc";
        itemStats = "ink_spell_glacial_wisdom_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "little_inkwell");
        itemType = ItemType.Component;

        recipeShouldBeExplored = true;
    }
}
[System.Serializable]
public class ItemScrollSpellSummonFieryMine : InventoryItem
{
    int spellID = 12;
    public ItemScrollSpellSummonFieryMine(int ID) : base(ID)
    {
        itemNameRaw = "scroll_spell_summon_fiery_mine";
        itemDescription = "scroll_spell_summon_fiery_mine_desc";
        itemStats = "spell_summon_fiery_mine_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "fire_spell_scroll");
        itemType = ItemType.Component;

        recipeExplorationResultID = 61;
    }

    public override void Use(Creature owner, int slotIndex)
    {
        base.Use(owner, slotIndex);
        if (owner.GetComponent<PlayerController>() != null)
        {
            PlayerController player = owner.GetComponent<PlayerController>();
            player.DeselectVisualSlots();
            Spell spellToCast = SpellsDatabase.GetSpellByID(spellID);
            spellToCast.spellLevel = owner.GetStateValue("spell_power");
            player.StartCastingSpell(spellToCast, slotIndex);
        }
    }

    public override string[] CalculateItemDescriptionParts(Creature owner)
    {
        base.CalculateItemDescriptionParts(owner);
        int spellPowerBonus = 0;
        for (int i = 0; i < SpellsDatabase.GetSpellByID(spellID).spellTypes.Length; i++)
            spellPowerBonus += owner.GetStateValue("spell_power_bonus_" + SpellsDatabase.GetSpellByID(spellID).spellTypes[i]);
        return new string[1] { (5 + 19 * (owner.GetStateValue("spell_power") + spellPowerBonus)).ToString() };
    }
}
[System.Serializable]
public class ItemInkSpellSummonFieryMine : InventoryItem
{
    public ItemInkSpellSummonFieryMine(int ID) : base(ID)
    {
        itemNameRaw = "ink_spell_summon_fiery_mine";
        itemDescription = "ink_spell_summon_fiery_mine_desc";
        itemStats = "ink_spell_summon_fiery_mine_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "little_inkwell");
        itemType = ItemType.Component;

        recipeShouldBeExplored = true;
    }
}
[System.Serializable]
public class ItemScrollSpellFlamingBlast : InventoryItem
{
    int spellID = 13;
    public ItemScrollSpellFlamingBlast(int ID) : base(ID)
    {
        itemNameRaw = "scroll_spell_flaming_blast";
        itemDescription = "scroll_spell_flaming_blast_desc";
        itemStats = "spell_flaming_blast_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "fire_spell_scroll");
        itemType = ItemType.Component;

        recipeExplorationResultID = 63;
    }

    public override void Use(Creature owner, int slotIndex)
    {
        base.Use(owner, slotIndex);
        if (owner.GetComponent<PlayerController>() != null)
        {
            PlayerController player = owner.GetComponent<PlayerController>();
            player.DeselectVisualSlots();
            Spell spellToCast = SpellsDatabase.GetSpellByID(spellID);
            spellToCast.spellLevel = owner.GetStateValue("spell_power");
            player.StartCastingSpell(spellToCast, slotIndex);
        }
    }

    public override string[] CalculateItemDescriptionParts(Creature owner)
    {
        base.CalculateItemDescriptionParts(owner);
        int spellPowerBonus = 0;
        for (int i = 0; i < SpellsDatabase.GetSpellByID(spellID).spellTypes.Length; i++)
            spellPowerBonus += owner.GetStateValue("spell_power_bonus_" + SpellsDatabase.GetSpellByID(spellID).spellTypes[i]);
        return new string[4] 
        {
            (6 + 15 * (owner.GetStateValue("spell_power") + spellPowerBonus)).ToString(),
            (10 + (owner.GetStateValue("spell_power") + spellPowerBonus) / 4).ToString(),
            RomanNumerals.GetRomanNumeral(1 + (owner.GetStateValue("spell_power") + spellPowerBonus) / 7),
            (1 + (owner.GetStateValue("spell_power") + spellPowerBonus) / 29).ToString()
        };
    }
}
[System.Serializable]
public class ItemInkSpellFlamingBlast : InventoryItem
{
    public ItemInkSpellFlamingBlast(int ID) : base(ID)
    {
        itemNameRaw = "ink_spell_flaming_blast";
        itemDescription = "ink_spell_flaming_blast_desc";
        itemStats = "ink_spell_flaming_blast_stats";
        itemIcon = Resources.Load<Sprite>(ItemsDatabase.itemsIconsDirectory + "little_inkwell");
        itemType = ItemType.Component;

        recipeShouldBeExplored = true;
    }
}