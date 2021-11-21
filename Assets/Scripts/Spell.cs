using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpellRequirement
{
    ChooseAnyCell,
    ChoosePassableCell,
    ChooseEmptyCell,
    ChooseTargetOrSelf,
    ChooseTarget
}

public enum SpellType
{
    fire,
    ice,
    earth,
    air,
    light,
    dark,
    energy
}

[System.Serializable]
public class Spell
{
    public int spellID;
    public string spellName;
    public Sprite spellIcon;
    public string spellDescription;
    public int spellLevel;
    public SpellType[] spellTypes;

    public SpellRequirement[] spellRequirements;
    public bool isRanged;
    public int spellRange;
    public bool canBeAppliedOnDeadCreature = false;

    public VisualEffect connectedVisualEffect;

    public Spell(int ID, string name)
    {
        spellID = ID;
        spellName = name;
    }

    public virtual void EmbedRequirements(List<object> requirements) { }
    public virtual IEnumerator SpellAction(Creature sender, int _spellLevel)
    {
        yield return null;
    }
    public virtual Spell Clone(int _id, string _name)
    {
        return new Spell(_id, _name);
    }
}