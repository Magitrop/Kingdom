using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellsDatabase
{
    public static List<Spell> registeredSpells = new List<Spell>();
    public static string spellsIconsDirectory;

    public static void InitializeDatabase()
    {
        spellsIconsDirectory = "SpellsSprites/";

        registeredSpells.Clear();
        registeredSpells.Add(new SpellNone(-1, "spell_none"));
        // ice spells
        registeredSpells.Add(new SpellIceBoulder(1, "spell_ice_boulder"));
        registeredSpells.Add(new SpellIceRing(3, "spell_ice_ring"));
        registeredSpells.Add(new SpellIcing(4, "spell_icing"));
        registeredSpells.Add(new SpellFrostbite(5, "spell_frostbite"));
        registeredSpells.Add(new SpellSummonIceGolem(6, "spell_summon_ice_golem"));
        registeredSpells.Add(new SpellFrozenShield(7, "spell_frozen_shield"));
        registeredSpells.Add(new SpellSummonIceCrystal(8, "spell_summon_ice_crystal"));
        registeredSpells.Add(new SpellChillingWind(9, "spell_chilling_wind"));
        registeredSpells.Add(new SpellChills(10, "spell_chills"));
        registeredSpells.Add(new SpellGlacialWisdom(11, "spell_glacial_wisdom"));

        // fire spells
        registeredSpells.Add(new SpellSummonFieryMine(12, "spell_summon_fiery_mine"));
        registeredSpells.Add(new SpellFlamingBlast(13, "spell_flaming_blast"));

        // light spells
        registeredSpells.Add(new SpellBrightening(0, "spell_brightening"));

        // energy spells
        registeredSpells.Add(new SpellEnergyStrike(2, "spell_energy_strike"));
    }

    public static Spell GetSpellByID(int ID)
    {
        for (int i = 0; i < registeredSpells.Count; i++)
            if (registeredSpells[i].spellID == ID)
                return registeredSpells[i].Clone(ID, registeredSpells[i].spellName);
        return registeredSpells[0].Clone(ID, registeredSpells[0].spellName);
    }
}

[System.Serializable]
public class SpellNone : Spell
{
    public SpellNone(int ID, string name) : base(ID, name)
    {
        spellDescription = "null";
    }

    public override Spell Clone(int _id, string _name)
    {
        return new SpellNone(_id, _name);
    }
}
[System.Serializable]
public class SpellBrightening : Spell
{
    public Vector2Int actionPosition;

    public SpellBrightening(int ID, string name) : base(ID, name)
    {
        spellDescription = name + "_desc";
        spellIcon = Resources.Load<Sprite>(SpellsDatabase.spellsIconsDirectory + spellName);
        spellTypes = new SpellType[1] { SpellType.light };

        spellRequirements = new SpellRequirement[0];
        isRanged = false;
        spellRange = 6;
    }

    public override Spell Clone(int _id, string _name)
    {
        return new SpellBrightening(_id, _name);
    }

    public override void EmbedRequirements(List<object> requirements) { }

    public override IEnumerator SpellAction(Creature sender, int _spellLevel)
    {
        MapCell senderPos = sender.map.cells[sender.x, sender.y];
        sender.PlayVisualEffect(spellName);
        for (int i = 1; i < spellRange; i++)
        {
            sender.map.RecalculateFogOfWar(senderPos, i);
            yield return new WaitForSeconds(0.125f);
        }
    }
}
[System.Serializable]
public class SpellIceBoulder : Spell
{
    public Creature target;

    public SpellIceBoulder(int ID, string name) : base(ID, name)
    {
        spellDescription = name + "_desc";
        spellIcon = Resources.Load<Sprite>(SpellsDatabase.spellsIconsDirectory + spellName);
        spellTypes = new SpellType[1] { SpellType.ice };

        spellRequirements = new SpellRequirement[1] { SpellRequirement.ChooseTarget };
        isRanged = true;
        spellRange = 6;
    }

    public override Spell Clone(int _id, string _name)
    {
        return new SpellIceBoulder(_id, _name);
    }

    public override void EmbedRequirements(List<object> requirements)
    {
        target = (Creature)requirements[0];
    }

    public override IEnumerator SpellAction(Creature sender, int _spellLevel)
    {
        spellLevel = _spellLevel;
        int damage = 9 + 35 * spellLevel;
        target.PlayVisualEffect(spellName);
        yield return new WaitForSeconds(0.2f);
        target.GetDamage(damage, DamageType.freezing, sender);
    }
}
[System.Serializable]
public class SpellEnergyStrike : Spell
{
    public Creature target;

    public SpellEnergyStrike(int ID, string name) : base(ID, name)
    {
        spellDescription = name + "_desc";
        spellIcon = Resources.Load<Sprite>(SpellsDatabase.spellsIconsDirectory + spellName);
        spellTypes = new SpellType[1] { SpellType.energy };

        spellRequirements = new SpellRequirement[1] { SpellRequirement.ChooseTarget };
        isRanged = true;
        spellRange = 4;
    }

    public override Spell Clone(int _id, string _name)
    {
        return new SpellEnergyStrike(_id, _name);
    }

    public override void EmbedRequirements(List<object> requirements)
    {
        target = (Creature)requirements[0];
    }

    public override IEnumerator SpellAction(Creature sender, int _spellLevel)
    {
        spellLevel = _spellLevel;
        int damage = 5 + Mathf.RoundToInt(4.5f * spellLevel);
        target.PlayVisualEffect(spellName);
        yield return new WaitForSeconds(0.2f);
        target.GetDamage(damage, DamageType.electric, sender);
        yield return new WaitForSeconds(1f);
        if (target != null)
        {
            if (target.map.cells[target.x, target.y + 1].cellObject != null && target.map.cells[target.x, target.y + 1].cellObject.GetComponent<Creature>() != null)
                target.map.cells[target.x, target.y + 1].cellObject.GetComponent<Creature>().GetDamage(damage, DamageType.electric, sender);
            if (target.map.cells[target.x, target.y - 1].cellObject != null && target.map.cells[target.x, target.y - 1].cellObject.GetComponent<Creature>() != null)
                target.map.cells[target.x, target.y - 1].cellObject.GetComponent<Creature>().GetDamage(damage, DamageType.electric, sender);
            if (target.map.cells[target.x + 1, target.y].cellObject != null && target.map.cells[target.x + 1, target.y].cellObject.GetComponent<Creature>() != null)
                target.map.cells[target.x + 1, target.y].cellObject.GetComponent<Creature>().GetDamage(damage, DamageType.electric, sender);
            if (target.map.cells[target.x - 1, target.y].cellObject != null && target.map.cells[target.x - 1, target.y].cellObject.GetComponent<Creature>() != null)
                target.map.cells[target.x - 1, target.y].cellObject.GetComponent<Creature>().GetDamage(damage, DamageType.electric, sender);
        }
    }
}
[System.Serializable]
public class SpellIceRing : Spell
{
    public Vector2Int cell;

    public SpellIceRing(int ID, string name) : base(ID, name)
    {
        spellDescription = name + "_desc";
        spellIcon = Resources.Load<Sprite>(SpellsDatabase.spellsIconsDirectory + spellName);
        spellTypes = new SpellType[1] { SpellType.ice };

        spellRequirements = new SpellRequirement[1] { SpellRequirement.ChoosePassableCell };
        isRanged = true;
        spellRange = 5;
    }

    public override Spell Clone(int _id, string _name)
    {
        return new SpellIceRing(_id, _name);
    }

    public override void EmbedRequirements(List<object> requirements)
    {
        cell = (Vector2Int)requirements[0];
    }

    public override IEnumerator SpellAction(Creature sender, int _spellLevel)
    {
        spellLevel = _spellLevel;
        int damage = 5 + 19 * spellLevel;
        sender.PlayVisualEffect(spellName, new Vector3(cell.x + 0.5f, cell.y + 0.65f));
        yield return new WaitForSeconds(0.4f);
        for (int x = cell.x - 1; x <= cell.x + 1; x++)
        {
            for (int y = cell.y - 1; y <= cell.y + 1; y++)
            {
                if (x < 0 || x >= sender.map.mapSizeX || y < 0 || y >= sender.map.mapSizeY || (x == cell.x && y == cell.y))
                    continue;
                if (sender.map.cells[x, y].cellObject != null && sender.map.cells[x, y].cellObject.GetComponent<Creature>() != null)
                    sender.map.cells[x, y].cellObject.GetComponent<Creature>().GetDamage(damage, DamageType.freezing, sender);
            }
        }
    }
}
[System.Serializable]
public class SpellIcing : Spell
{
    public Creature target;

    public SpellIcing(int ID, string name) : base(ID, name)
    {
        spellDescription = name + "_desc";
        spellIcon = Resources.Load<Sprite>(SpellsDatabase.spellsIconsDirectory + spellName);
        spellTypes = new SpellType[1] { SpellType.ice };

        spellRequirements = new SpellRequirement[1] { SpellRequirement.ChooseTarget };
        isRanged = true;
        spellRange = 5;
    }

    public override Spell Clone(int _id, string _name)
    {
        return new SpellIcing(_id, _name);
    }

    public override void EmbedRequirements(List<object> requirements)
    {
        target = (Creature)requirements[0];
    }

    public override IEnumerator SpellAction(Creature sender, int _spellLevel)
    {
        spellLevel = _spellLevel;
        int damage = 7 + 13 * spellLevel;
        //Effect effect = EffectsDatabase.GetEffectByID(6);
        //effect.connectedVisualEffect = target.PlayVisualEffect(spellName);
        Effect.ApplyEffect(EffectsDatabase.GetEffectByID(6), target, sender, spellLevel / 23 + 1, spellLevel / 23 + 1);
        yield return new WaitForSeconds(0.33f);
        target.GetDamage(damage, DamageType.freezing, sender);
    }
}
[System.Serializable]
public class SpellFrostbite : Spell
{
    public Creature target;

    public SpellFrostbite(int ID, string name) : base(ID, name)
    {
        spellDescription = name + "_desc";
        spellIcon = Resources.Load<Sprite>(SpellsDatabase.spellsIconsDirectory + spellName);
        spellTypes = new SpellType[1] { SpellType.ice };

        spellRequirements = new SpellRequirement[1] { SpellRequirement.ChooseTarget };
        isRanged = true;
        spellRange = 5;
    }

    public override Spell Clone(int _id, string _name)
    {
        return new SpellFrostbite(_id, _name);
    }

    public override void EmbedRequirements(List<object> requirements)
    {
        target = (Creature)requirements[0];
    }

    public override IEnumerator SpellAction(Creature sender, int _spellLevel)
    {
        spellLevel = _spellLevel;
        //Effect effect = EffectsDatabase.GetEffectByID(6);
        //effect.connectedVisualEffect = target.PlayVisualEffect(spellName);
        Effect.ApplyEffect(EffectsDatabase.GetEffectByID(7), target, sender, spellLevel, spellLevel);
        yield return new WaitForSeconds(0.33f);
    }
}
[System.Serializable]
public class SpellSummonIceGolem : Spell
{
    public Vector2Int cell;

    public SpellSummonIceGolem(int ID, string name) : base(ID, name)
    {
        spellDescription = name + "_desc";
        spellIcon = Resources.Load<Sprite>(SpellsDatabase.spellsIconsDirectory + spellName);
        spellTypes = new SpellType[1] { SpellType.ice };

        spellRequirements = new SpellRequirement[1] { SpellRequirement.ChooseEmptyCell };
        isRanged = true;
        spellRange = 4;
    }

    public override Spell Clone(int _id, string _name)
    {
        return new SpellSummonIceGolem(_id, _name);
    }

    public override void EmbedRequirements(List<object> requirements)
    {
        cell = (Vector2Int)requirements[0];
    }

    public override IEnumerator SpellAction(Creature sender, int _spellLevel)
    {
        spellLevel = _spellLevel;
        sender.PlayVisualEffect(spellName, new Vector3(cell.x + 0.5f, cell.y + 0.5f));
        yield return new WaitForSeconds(0.1f);
        Creature golem = sender.map.SpawnCreature(sender.map.creaturesContainer.GetCreatureByName("friendly_ice_golem"), cell.x, cell.y);
        if (sender.GetComponent<PlayerController>() != null)
            golem.isSummonedByPlayer = true;
        golem.ChangeStateBonus("max_health", 20 + 30 * _spellLevel);
        golem.GetStateByName("max_health").startStateValue = 20 + 30 * _spellLevel + 1;
        golem.SetStateValue("cur_health", 20 + 30 * _spellLevel + 1);
        golem.ChangeStateBonus("max_energy", _spellLevel / 27 + 1);
        golem.GetStateByName("max_energy").startStateValue = _spellLevel / 27 + 1;
        golem.SetStateValue("cur_energy", _spellLevel / 27 + 1);
        golem.ChangeStateBonus("attack_damage", 17 + 17 * _spellLevel);
        golem.GetStateByName("attack_damage").startStateValue = 17 + 17 * _spellLevel;
    }
}
[System.Serializable]
public class SpellFrozenShield : Spell
{
    public Creature target;

    public SpellFrozenShield(int ID, string name) : base(ID, name)
    {
        spellDescription = name + "_desc";
        spellIcon = Resources.Load<Sprite>(SpellsDatabase.spellsIconsDirectory + spellName);
        spellTypes = new SpellType[1] { SpellType.ice };

        spellRequirements = new SpellRequirement[1] { SpellRequirement.ChooseTargetOrSelf };
        isRanged = true;
        spellRange = 4;
    }

    public override Spell Clone(int _id, string _name)
    {
        return new SpellFrozenShield(_id, _name);
    }

    public override void EmbedRequirements(List<object> requirements)
    {
        target = (Creature)requirements[0];
    }

    public override IEnumerator SpellAction(Creature sender, int _spellLevel)
    {
        spellLevel = _spellLevel;
        yield return new WaitForSeconds(0.2f);
        Effect.ApplyEffect(EffectsDatabase.GetEffectByID(8), target, sender, spellLevel / 23 + 1, spellLevel);
    }
}
[System.Serializable]
public class SpellSummonIceCrystal : Spell
{
    public Vector2Int cell;

    public SpellSummonIceCrystal(int ID, string name) : base(ID, name)
    {
        spellDescription = name + "_desc";
        spellIcon = Resources.Load<Sprite>(SpellsDatabase.spellsIconsDirectory + spellName);
        spellTypes = new SpellType[1] { SpellType.ice };

        spellRequirements = new SpellRequirement[1] { SpellRequirement.ChooseEmptyCell };
        isRanged = true;
        spellRange = 6;
    }

    public override Spell Clone(int _id, string _name)
    {
        return new SpellSummonIceCrystal(_id, _name);
    }

    public override void EmbedRequirements(List<object> requirements)
    {
        cell = (Vector2Int)requirements[0];
    }

    public override IEnumerator SpellAction(Creature sender, int _spellLevel)
    {
        spellLevel = _spellLevel;
        yield return new WaitForSeconds(0.1f);
        Creature crystal = sender.map.SpawnCreature(sender.map.creaturesContainer.GetCreatureByName("ice_crystal"), cell.x, cell.y);
        crystal.ChangeStateBonus("max_health", _spellLevel / 8);
        crystal.GetStateByName("max_health").startStateValue = _spellLevel / 8 + 1;
        crystal.SetStateValue("cur_health", _spellLevel / 8 + 1);
        if (sender.GetComponent<PlayerController>() != null)
            crystal.isSummonedByPlayer = true;
    }
}
[System.Serializable]
public class SpellChillingWind : Spell
{
    public Vector2Int actionPosition;

    public SpellChillingWind(int ID, string name) : base(ID, name)
    {
        spellDescription = name + "_desc";
        spellIcon = Resources.Load<Sprite>(SpellsDatabase.spellsIconsDirectory + spellName);
        spellTypes = new SpellType[1] { SpellType.ice };

        spellRequirements = new SpellRequirement[0];
        isRanged = false;
    }

    public override Spell Clone(int _id, string _name)
    {
        return new SpellChillingWind(_id, _name);
    }

    public override void EmbedRequirements(List<object> requirements) { }

    public override IEnumerator SpellAction(Creature sender, int _spellLevel)
    {
        MapCell senderPos = sender.map.cells[sender.x, sender.y];
        spellLevel = _spellLevel;
        int damage = 5 + 15 * spellLevel;
        switch (sender.facingDirection)
        {
            case FacingDirection.Up:
                {
                    for (int i = 1; i <= spellLevel / 12 + 2; i++)
                    {
                        if (sender.y + i >= sender.map.mapSizeY || sender.map.cells[sender.x, sender.y + i].isPassable == false || sender.map.CheckCell(sender.x, sender.y + i, CellCheckFlags.ToVisibility) == false)
                            break;
                        sender.PlayVisualEffect(spellName + "_up", new Vector3(sender.x + 0.5f, sender.y + i + 0.5f));
                        if (sender.map.cells[sender.x, sender.y + i].cellObject != null && sender.map.cells[sender.x, sender.y + i].cellObject.GetComponent<Creature>() != null)
                            sender.map.cells[sender.x, sender.y + i].cellObject.GetComponent<Creature>().GetDamage(damage, DamageType.freezing, sender);
                        yield return new WaitForSeconds(0.25f);
                    }
                    break;
                }
            case FacingDirection.Right:
                {
                    for (int i = 1; i <= _spellLevel / 12 + 2; i++)
                    {
                        if (sender.x + i >= sender.map.mapSizeX || sender.map.cells[sender.x + i, sender.y].isPassable == false || sender.map.CheckCell(sender.x + i, sender.y, CellCheckFlags.ToVisibility) == false)
                            break;
                        sender.PlayVisualEffect(spellName + "_right", new Vector3(sender.x + i + 0.5f, sender.y + 0.75f));
                        if (sender.map.cells[sender.x + i, sender.y].cellObject != null && sender.map.cells[sender.x + i, sender.y].cellObject.GetComponent<Creature>() != null)
                            sender.map.cells[sender.x + i, sender.y].cellObject.GetComponent<Creature>().GetDamage(damage, DamageType.freezing, sender);
                        yield return new WaitForSeconds(0.25f);
                    }
                    break;
                }
            case FacingDirection.Down:
                {
                    for (int i = 1; i <= _spellLevel / 12 + 2; i++)
                    {
                        if (sender.y + i < 0 || sender.map.cells[sender.x, sender.y - i].isPassable == false || sender.map.CheckCell(sender.x, sender.y - i, CellCheckFlags.ToVisibility) == false)
                            break;
                        sender.PlayVisualEffect(spellName + "_down", new Vector3(sender.x + 0.5f, sender.y - i + 0.75f));
                        if (sender.map.cells[sender.x, sender.y - i].cellObject != null && sender.map.cells[sender.x, sender.y - i].cellObject.GetComponent<Creature>() != null)
                            sender.map.cells[sender.x, sender.y - i].cellObject.GetComponent<Creature>().GetDamage(damage, DamageType.freezing, sender);
                        yield return new WaitForSeconds(0.25f);
                    }
                    break;
                }
            case FacingDirection.Left:
                {
                    for (int i = 1; i <= _spellLevel / 12 + 2; i++)
                    {
                        if (sender.x - i < 0 || sender.map.cells[sender.x - i, sender.y].isPassable == false || sender.map.CheckCell(sender.x - i, sender.y, CellCheckFlags.ToVisibility) == false)
                            break;
                        sender.PlayVisualEffect(spellName + "_left", new Vector3(sender.x - i + 0.5f, sender.y + 0.75f));
                        if (sender.map.cells[sender.x - i, sender.y].cellObject != null && sender.map.cells[sender.x - i, sender.y].cellObject.GetComponent<Creature>() != null)
                            sender.map.cells[sender.x - i, sender.y].cellObject.GetComponent<Creature>().GetDamage(damage, DamageType.freezing, sender);
                        yield return new WaitForSeconds(0.25f);
                    }
                    break;
                }
        }
    }
}
[System.Serializable]
public class SpellChills : Spell
{
    public Creature target;

    public SpellChills(int ID, string name) : base(ID, name)
    {
        spellDescription = name + "_desc";
        spellIcon = Resources.Load<Sprite>(SpellsDatabase.spellsIconsDirectory + spellName);
        spellTypes = new SpellType[1] { SpellType.ice };

        spellRequirements = new SpellRequirement[1] { SpellRequirement.ChooseTarget };
        isRanged = true;
        spellRange = 5;
    }

    public override Spell Clone(int _id, string _name)
    {
        return new SpellChills(_id, _name);
    }

    public override void EmbedRequirements(List<object> requirements)
    {
        target = (Creature)requirements[0];
    }

    public override IEnumerator SpellAction(Creature sender, int _spellLevel)
    {
        spellLevel = _spellLevel;
        yield return new WaitForSeconds(0.2f);
        Effect.ApplyEffect(EffectsDatabase.GetEffectByID(9), target, sender, spellLevel / 19 + 1, spellLevel);
    }
}
[System.Serializable]
public class SpellGlacialWisdom : Spell
{
    public SpellGlacialWisdom(int ID, string name) : base(ID, name)
    {
        spellDescription = name + "_desc";
        spellIcon = Resources.Load<Sprite>(SpellsDatabase.spellsIconsDirectory + spellName);
        spellTypes = new SpellType[1] { SpellType.ice };

        spellRequirements = new SpellRequirement[0] { };
    }

    public override Spell Clone(int _id, string _name)
    {
        return new SpellGlacialWisdom(_id, _name);
    }

    public override void EmbedRequirements(List<object> requirements) { }

    public override IEnumerator SpellAction(Creature sender, int _spellLevel)
    {
        spellLevel = _spellLevel;
        yield return new WaitForSeconds(0.5f);
        Effect.ApplyEffect(EffectsDatabase.GetEffectByID(10), sender, sender, spellLevel / 21 + 1, spellLevel);
    }
}
[System.Serializable]
public class SpellSummonFieryMine : Spell
{
    public Vector2Int cell;

    public SpellSummonFieryMine(int ID, string name) : base(ID, name)
    {
        spellDescription = name + "_desc";
        spellIcon = Resources.Load<Sprite>(SpellsDatabase.spellsIconsDirectory + spellName);
        spellTypes = new SpellType[1] { SpellType.fire };

        spellRequirements = new SpellRequirement[1] { SpellRequirement.ChooseEmptyCell };
        isRanged = true;
        spellRange = 5;
    }

    public override Spell Clone(int _id, string _name)
    {
        return new SpellSummonFieryMine(_id, _name);
    }

    public override void EmbedRequirements(List<object> requirements)
    {
        cell = (Vector2Int)requirements[0];
    }

    public override IEnumerator SpellAction(Creature sender, int _spellLevel)
    {
        spellLevel = _spellLevel;
        yield return new WaitForSeconds(0.1f);
        Creature crystal = sender.map.SpawnCreature(sender.map.creaturesContainer.GetCreatureByName("fiery_mine"), cell.x, cell.y);
        crystal.ChangeStateBonus("attack_damage", 5 + 19 * _spellLevel);
        crystal.GetStateByName("attack_damage").startStateValue = 5 + 19 * _spellLevel + 1;
        if (sender.GetComponent<PlayerController>() != null)
            crystal.isSummonedByPlayer = true;
    }
}
[System.Serializable]
public class SpellFlamingBlast : Spell
{
    public Vector2Int cell;

    public SpellFlamingBlast(int ID, string name) : base(ID, name)
    {
        spellDescription = name + "_desc";
        spellIcon = Resources.Load<Sprite>(SpellsDatabase.spellsIconsDirectory + spellName);
        spellTypes = new SpellType[1] { SpellType.fire };

        spellRequirements = new SpellRequirement[1] { SpellRequirement.ChooseAnyCell };
        isRanged = true;
        spellRange = 5;
    }

    public override Spell Clone(int _id, string _name)
    {
        return new SpellFlamingBlast(_id, _name);
    }

    public override void EmbedRequirements(List<object> requirements)
    {
        cell = (Vector2Int)requirements[0];
    }

    public override IEnumerator SpellAction(Creature sender, int _spellLevel)
    {
        spellLevel = _spellLevel;
        int damage = 6 + 15 * spellLevel;
        sender.PlayVisualEffect(spellName, new Vector3(cell.x + 0.5f, cell.y + 0.65f));
        yield return new WaitForSeconds(0.66f);
        for (int x = cell.x - 1; x <= cell.x + 1; x++)
        {
            for (int y = cell.y - 1; y <= cell.y + 1; y++)
            {
                if (x < 0 || x >= sender.map.mapSizeX || y < 0 || y >= sender.map.mapSizeY)
                    continue;
                if (sender.map.cells[x, y].cellObject != null && sender.map.cells[x, y].cellObject.GetComponent<Creature>() != null)
                {
                    if (x == cell.x && y == cell.y)
                        sender.map.cells[x, y].cellObject.GetComponent<Creature>().GetDamage(Mathf.RoundToInt(damage * 1.5f), DamageType.scalding, sender);
                    else
                        sender.map.cells[x, y].cellObject.GetComponent<Creature>().GetDamage(damage, DamageType.scalding, sender);
                    if (Random.Range(0f, 100f) < 10 + (spellLevel / 4))
                        Effect.ApplyEffect(EffectsDatabase.GetEffectByID(3), sender.map.cells[x, y].cellObject.GetComponent<Creature>(), sender, 1 + spellLevel / 29, 1 + spellLevel / 7);
                }
            }
        }
    }
}