using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill
{
    public int skillID;
    public string skillName;
    public int[] requiredSkillsIDs = new int[0];
    public Sprite skillIcon;

    public PlayerController player;
    public int heroLevelWhenGained;

    public Skill(int _id, string _name)
    {
        skillID = _id;
        skillName = _name;
    }

    protected string[] skillDescriptionParts;

    public virtual void OnSkillGain(int heroLevel) { heroLevelWhenGained = heroLevel; }
    public virtual void OnTurnStart() { }
    public virtual void OnTurnStartLate() { }
    public virtual void OnTurnEnd() { }
    public virtual int OnCreatureDamage(Creature _sender, DamageType _damageType, int _damage) { return 0; } // should return how much damage will be canceled after this function
    public virtual void OnCreatureAttack(Creature _attacked, DamageType _damageType, int _damage) { }
    public virtual void OnCreatureHeal(int _healing) { }
    public virtual void OnCreatureDeath() { }
    public virtual void OnCreatureKill(Creature creature) { }
    public virtual void OnCreatureCastSpell(Spell spell) { }
    public virtual void OnCreatureMove() { }

    // SHOULD BE OVERRIDDEN
    public virtual Skill Clone(int _id, string _name)
    {
        return new Skill(_id, _name) { skillID = _id, skillName = _name };
    }

    public override string ToString()
    {
        return skillID + ";" + heroLevelWhenGained;
    }

    public virtual string TranslateSkillDescription(string skillDescription, int _heroLevel)
    {
        return "skill not found";
    }
}

public class SkillOccultism : Skill
{
    public SkillOccultism(int _id, string _name) : base(_id, _name)
    {
        skillIcon = Resources.Load<Sprite>(SkillsDatabase.skillsIconsDirectory + skillName);

        skillDescriptionParts = new string[1];
    }

    public override void OnSkillGain(int heroLevel)
    {
        base.OnSkillGain(heroLevel);
        player.ChangeStateBonus("spell_power", heroLevel * 2);
    }

    public override Skill Clone(int _id, string _name)
    {
        // don't forget to change
        return new SkillOccultism(_id, _name) { skillID = _id, skillName = _name };
    }

    public override string TranslateSkillDescription(string skillDescription, int _heroLevel)
    {
        skillDescriptionParts[0] = (_heroLevel * 2).ToString();
        string result = Translate.TranslateText(skillDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), skillDescriptionParts[number]);
        }
        return result;
    }
}

public class SkillVitality : Skill
{
    public SkillVitality(int _id, string _name) : base(_id, _name)
    {
        skillIcon = Resources.Load<Sprite>(SkillsDatabase.skillsIconsDirectory + skillName);

        skillDescriptionParts = new string[1];
    }

    public override void OnSkillGain(int heroLevel)
    {
        base.OnSkillGain(heroLevel);
        player.ChangeStateBonus("max_health", 16 * heroLevel);
        player.GetHealing(16 * heroLevel);
    }

    public override Skill Clone(int _id, string _name)
    {
        // don't forget to change
        return new SkillVitality(_id, _name) { skillID = _id, skillName = _name };
    }

    public override string TranslateSkillDescription(string skillDescription, int _heroLevel)
    {
        skillDescriptionParts[0] = (16 * _heroLevel).ToString();
        string result = Translate.TranslateText(skillDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), skillDescriptionParts[number]);
        }
        return result;
    }
}

public class SkillStamina : Skill
{
    public SkillStamina(int _id, string _name) : base(_id, _name)
    {
        skillIcon = Resources.Load<Sprite>(SkillsDatabase.skillsIconsDirectory + skillName);

        skillDescriptionParts = new string[1];
    }

    public override void OnSkillGain(int heroLevel)
    {
        base.OnSkillGain(heroLevel);
        player.ChangeStateBonus("max_energy", 1 + heroLevel / 15);
    }

    public override Skill Clone(int _id, string _name)
    {
        // don't forget to change
        return new SkillStamina(_id, _name) { skillID = _id, skillName = _name };
    }

    public override string TranslateSkillDescription(string skillDescription, int _heroLevel)
    {
        skillDescriptionParts[0] = (1 + _heroLevel / 15).ToString();
        string result = Translate.TranslateText(skillDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), skillDescriptionParts[number]);
        }
        return result;
    }
}

public class SkillStrengthening : Skill
{
    public SkillStrengthening(int _id, string _name) : base(_id, _name)
    {
        skillIcon = Resources.Load<Sprite>(SkillsDatabase.skillsIconsDirectory + skillName);

        skillDescriptionParts = new string[1];
    }

    public override void OnSkillGain(int heroLevel)
    {
        base.OnSkillGain(heroLevel);
        player.ChangeStateBonus("protection_blunt", heroLevel / 4 + 1);
        player.ChangeStateBonus("protection_pricking", heroLevel / 4 + 1);
        player.ChangeStateBonus("protection_cutting", heroLevel / 4 + 1);
        player.ChangeStateBonus("protection_hewing", heroLevel / 4 + 1);
        player.ChangeStateBonus("protection_lashing", heroLevel / 4 + 1);
        player.ChangeStateBonus("protection_suffocative", heroLevel / 4 + 1);
        player.ChangeStateBonus("protection_scalding", heroLevel / 4 + 1);
        player.ChangeStateBonus("protection_freezing", heroLevel / 4 + 1);
        player.ChangeStateBonus("protection_electric", heroLevel / 4 + 1);
        player.ChangeStateBonus("protection_poison", heroLevel / 4 + 1);
    }

    public override Skill Clone(int _id, string _name)
    {
        // don't forget to change
        return new SkillStrengthening(_id, _name) { skillID = _id, skillName = _name };
    }

    public override string TranslateSkillDescription(string skillDescription, int _heroLevel)
    {
        skillDescriptionParts[0] = (_heroLevel / 4 + 1).ToString();
        string result = Translate.TranslateText(skillDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), skillDescriptionParts[number]);
        }
        return result;
    }
}

public class SkillArmament : Skill
{
    public SkillArmament(int _id, string _name) : base(_id, _name)
    {
        skillIcon = Resources.Load<Sprite>(SkillsDatabase.skillsIconsDirectory + skillName);

        skillDescriptionParts = new string[1];
    }

    public override void OnSkillGain(int heroLevel)
    {
        base.OnSkillGain(heroLevel);
        player.ChangeStateBonus("attack_damage", 5 * heroLevel);
    }

    public override Skill Clone(int _id, string _name)
    {
        // don't forget to change
        return new SkillArmament(_id, _name) { skillID = _id, skillName = _name };
    }

    public override string TranslateSkillDescription(string skillDescription, int _heroLevel)
    {
        skillDescriptionParts[0] = (5 * _heroLevel).ToString();
        string result = Translate.TranslateText(skillDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), skillDescriptionParts[number]);
        }
        return result;
    }
}

public class SkillMedium : Skill
{
    public SkillMedium(int _id, string _name) : base(_id, _name)
    {
        skillIcon = Resources.Load<Sprite>(SkillsDatabase.skillsIconsDirectory + skillName);

        requiredSkillsIDs = new int[1] { 0 };
        skillDescriptionParts = new string[1];
    }

    public override void OnSkillGain(int heroLevel)
    {
        base.OnSkillGain(heroLevel);
    }

    public override void OnTurnEnd()
    {
        base.OnTurnStart();
        if (player.spellsCastInThisTurn > 0)
            player.GetHealing((5 + 10 * player.GetStateValue("level")) * player.GetStateValue("cur_energy"));
    }

    public override Skill Clone(int _id, string _name)
    {
        // don't forget to change
        return new SkillMedium(_id, _name) { skillID = _id, skillName = _name };
    }

    public override string TranslateSkillDescription(string skillDescription, int _heroLevel)
    {
        skillDescriptionParts[0] = (5 + 10 * _heroLevel).ToString();
        string result = Translate.TranslateText(skillDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), skillDescriptionParts[number]);
        }
        return result;
    }
}

public class SkillThickSkin : Skill
{
    public SkillThickSkin(int _id, string _name) : base(_id, _name)
    {
        skillIcon = Resources.Load<Sprite>(SkillsDatabase.skillsIconsDirectory + skillName);

        requiredSkillsIDs = new int[1] { 1 };
        skillDescriptionParts = new string[1];
    }

    public override void OnSkillGain(int heroLevel)
    {
        base.OnSkillGain(heroLevel);
    }

    public override int OnCreatureDamage(Creature _sender, DamageType _damageType, int _damage)
    {
        if (_damageType != DamageType.pure)
            return (1 + 2 * player.GetStateValue("level"));
        else return 0;
    }

    public override Skill Clone(int _id, string _name)
    {
        // don't forget to change
        return new SkillThickSkin(_id, _name) { skillID = _id, skillName = _name };
    }

    public override string TranslateSkillDescription(string skillDescription, int _heroLevel)
    {
        skillDescriptionParts[0] = (1 + 2 * _heroLevel).ToString();
        string result = Translate.TranslateText(skillDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), skillDescriptionParts[number]);
        }
        return result;
    }
}

public class SkillSharpenedBlades : Skill
{
    public SkillSharpenedBlades(int _id, string _name) : base(_id, _name)
    {
        skillIcon = Resources.Load<Sprite>(SkillsDatabase.skillsIconsDirectory + skillName);

        requiredSkillsIDs = new int[1] { 4 };
        skillDescriptionParts = new string[2];
    }

    public override void OnSkillGain(int heroLevel)
    {
        base.OnSkillGain(heroLevel);
    }

    public override void OnCreatureAttack(Creature _attacked, DamageType _damageType, int _damage)
    {
        base.OnCreatureAttack(_attacked, _damageType, _damage);
        if (Random.Range(0f, 100f) < 22)
        {
            Effect.ApplyEffect(EffectsDatabase.GetEffectByID(1), _attacked, player, 1 + player.GetStateValue("level") / 20, 1 + player.GetStateValue("level") / 6);
        }
    }

    public override Skill Clone(int _id, string _name)
    {
        // don't forget to change
        return new SkillSharpenedBlades(_id, _name) { skillID = _id, skillName = _name };
    }

    public override string TranslateSkillDescription(string skillDescription, int _heroLevel)
    {
        skillDescriptionParts[0] = RomanNumerals.GetRomanNumeral(1 + _heroLevel / 6);
        skillDescriptionParts[1] = (1 + _heroLevel / 20).ToString();
        string result = Translate.TranslateText(skillDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), skillDescriptionParts[number]);
        }
        return result;
    }
}

public class SkillPlateArmor : Skill
{
    public SkillPlateArmor(int _id, string _name) : base(_id, _name)
    {
        skillIcon = Resources.Load<Sprite>(SkillsDatabase.skillsIconsDirectory + skillName);

        requiredSkillsIDs = new int[1] { 3 };
        skillDescriptionParts = new string[1];
    }

    public override void OnSkillGain(int heroLevel)
    {
        base.OnSkillGain(heroLevel);
    }

    public override int OnCreatureDamage(Creature _sender, DamageType _damageType, int _damage)
    {
        if (player.GetStateValue("protection_" + _damageType) > 0)
            return player.GetStateValue("protection_" + _damageType) * ((player.GetStateValue("level") / 5) + 1);
        else return 0;
    }

    public override Skill Clone(int _id, string _name)
    {
        // don't forget to change
        return new SkillPlateArmor(_id, _name) { skillID = _id, skillName = _name };
    }

    public override string TranslateSkillDescription(string skillDescription, int _heroLevel)
    {
        skillDescriptionParts[0] = (1 + _heroLevel / 5).ToString();
        string result = Translate.TranslateText(skillDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), skillDescriptionParts[number]);
        }
        return result;
    }
}

public class SkillAgility : Skill
{
    public SkillAgility(int _id, string _name) : base(_id, _name)
    {
        skillIcon = Resources.Load<Sprite>(SkillsDatabase.skillsIconsDirectory + skillName);

        requiredSkillsIDs = new int[1] { 2 };
        skillDescriptionParts = new string[0];
    }

    public override void OnSkillGain(int heroLevel)
    {
        base.OnSkillGain(heroLevel);
    }
      
    public override void OnCreatureMove()
    {
        base.OnCreatureMove();
        if (Random.Range(0f, 100f) < 25f && player.GetStateValue("cur_energy") < player.GetStateValue("max_energy"))
            player.SetStateValue("cur_energy", player.GetStateValue("cur_energy") + 1);
    }

    public override Skill Clone(int _id, string _name)
    {
        // don't forget to change
        return new SkillAgility(_id, _name) { skillID = _id, skillName = _name };
    }

    public override string TranslateSkillDescription(string skillDescription, int _heroLevel)
    {
        string result = Translate.TranslateText(skillDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), skillDescriptionParts[number]);
        }
        return result;
    }
}

public class SkillMysticism : Skill
{
    public SkillMysticism(int _id, string _name) : base(_id, _name)
    {
        skillIcon = Resources.Load<Sprite>(SkillsDatabase.skillsIconsDirectory + skillName);

        requiredSkillsIDs = new int[1] { 5 };
        skillDescriptionParts = new string[1];
    }

    public override void OnSkillGain(int heroLevel)
    {
        base.OnSkillGain(heroLevel);
    }

    public override void OnCreatureCastSpell(Spell spell)
    {
        base.OnCreatureCastSpell(spell);
        player.GetHealing(20 + 20 * player.GetStateValue("level"));
    }

    public override Skill Clone(int _id, string _name)
    {
        // don't forget to change
        return new SkillMysticism(_id, _name) { skillID = _id, skillName = _name };
    }

    public override string TranslateSkillDescription(string skillDescription, int _heroLevel)
    {
        skillDescriptionParts[0] = (5 + 20 * _heroLevel).ToString();
        string result = Translate.TranslateText(skillDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), skillDescriptionParts[number]);
        }
        return result;
    }
}

public class SkillJaggedBlades : Skill
{
    public SkillJaggedBlades(int _id, string _name) : base(_id, _name)
    {
        skillIcon = Resources.Load<Sprite>(SkillsDatabase.skillsIconsDirectory + skillName);

        requiredSkillsIDs = new int[1] { 7 };
        skillDescriptionParts = new string[0];
    }

    public override void OnSkillGain(int heroLevel)
    {
        base.OnSkillGain(heroLevel);
    }

    public override void OnCreatureAttack(Creature _attacked, DamageType _damageType, int _damage)
    {
        base.OnCreatureAttack(_attacked, _damageType, _damage);
        int damage = Mathf.RoundToInt((_attacked.GetStateValue("max_health") - _attacked.GetStateValue("cur_health")) * 0.08f);
        _attacked.GetDamage(damage, DamageType.pure, player);
    }

    public override Skill Clone(int _id, string _name)
    {
        // don't forget to change
        return new SkillJaggedBlades(_id, _name) { skillID = _id, skillName = _name };
    }

    public override string TranslateSkillDescription(string skillDescription, int _heroLevel)
    {
        string result = Translate.TranslateText(skillDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), skillDescriptionParts[number]);
        }
        return result;
    }
}

public class SkillThorns : Skill
{
    public SkillThorns(int _id, string _name) : base(_id, _name)
    {
        skillIcon = Resources.Load<Sprite>(SkillsDatabase.skillsIconsDirectory + skillName);

        requiredSkillsIDs = new int[1] { 8 };
        skillDescriptionParts = new string[0];
    }

    public override void OnSkillGain(int heroLevel)
    {
        base.OnSkillGain(heroLevel);
    }

    public override int OnCreatureDamage(Creature _sender, DamageType _damageType, int _damage)
    {
        if (_sender.cellObjectID != player.cellObjectID)
            _sender.GetDamage(player.GetStateValue("protection_" + _damageType) * 6, DamageType.pricking, player);
        return 0;
    }

    public override Skill Clone(int _id, string _name)
    {
        // don't forget to change
        return new SkillThorns(_id, _name) { skillID = _id, skillName = _name };
    }

    public override string TranslateSkillDescription(string skillDescription, int _heroLevel)
    {
        string result = Translate.TranslateText(skillDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), skillDescriptionParts[number]);
        }
        return result;
    }
}

public class SkillAutosuggestion : Skill
{
    public SkillAutosuggestion(int _id, string _name) : base(_id, _name)
    {
        skillIcon = Resources.Load<Sprite>(SkillsDatabase.skillsIconsDirectory + skillName);

        requiredSkillsIDs = new int[1] { 9 };
        skillDescriptionParts = new string[1];
    }

    public override void OnSkillGain(int heroLevel)
    {
        base.OnSkillGain(heroLevel);
    }

    public override void OnTurnStart()
    {
        base.OnTurnStart();
        if (player.GetStateValue("cur_health") >= player.GetStateValue("max_health"))
            Effect.ApplyEffect(EffectsDatabase.GetEffectByID(11), player, player, 1, player.GetStateValue("level"));
    }

    /*
    public override void OnCreatureMove()
    {
        base.OnCreatureMove();
        if (Random.Range(0f, 100f) < 33f)
            player.GetHealing(4 + 4 * player.GetStateValue("level"));
    }
    */

    public override Skill Clone(int _id, string _name)
    {
        // don't forget to change
        return new SkillAutosuggestion(_id, _name) { skillID = _id, skillName = _name };
    }

    public override string TranslateSkillDescription(string skillDescription, int _heroLevel)
    {
        skillDescriptionParts[0] = RomanNumerals.GetRomanNumeral(_heroLevel);
        string result = Translate.TranslateText(skillDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), skillDescriptionParts[number]);
        }
        return result;
    }
}

public class SkillSynarchy : Skill
{
    public SkillSynarchy(int _id, string _name) : base(_id, _name)
    {
        skillIcon = Resources.Load<Sprite>(SkillsDatabase.skillsIconsDirectory + skillName);

        requiredSkillsIDs = new int[2] { 10, 6 };
        skillDescriptionParts = new string[1];
    }

    public override void OnSkillGain(int heroLevel)
    {
        base.OnSkillGain(heroLevel);
    }

    public override void OnTurnStartLate()
    {
        base.OnTurnStart();
        if (player.GetStateValue("cur_health") >= player.GetStateValue("max_health"))
            player.SetStateValue("cur_energy", player.GetStateValue("cur_energy") + (1 + player.GetStateValue("level") / 20));
    }

    public override Skill Clone(int _id, string _name)
    {
        // don't forget to change
        return new SkillSynarchy(_id, _name) { skillID = _id, skillName = _name };
    }

    public override string TranslateSkillDescription(string skillDescription, int _heroLevel)
    {
        skillDescriptionParts[0] = (1 + _heroLevel / 20).ToString();
        string result = Translate.TranslateText(skillDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), skillDescriptionParts[number]);
        }
        return result;
    }
}

public class SkillHeavyStrike : Skill
{
    public SkillHeavyStrike(int _id, string _name) : base(_id, _name)
    {
        skillIcon = Resources.Load<Sprite>(SkillsDatabase.skillsIconsDirectory + skillName);

        requiredSkillsIDs = new int[2] { 6, 11 };
        skillDescriptionParts = new string[0];
    }

    public override void OnSkillGain(int heroLevel)
    {
        base.OnSkillGain(heroLevel);
    }

    public override void OnCreatureAttack(Creature _attacked, DamageType _damageType, int _damage)
    {
        base.OnCreatureAttack(_attacked, _damageType, _damage);
        int damage = Mathf.RoundToInt(player.GetStateValue("cur_health") * 0.1f);
        _attacked.GetDamage(damage, player.autoattackDamageType, player);
    }

    public override Skill Clone(int _id, string _name)
    {
        // don't forget to change
        return new SkillHeavyStrike(_id, _name) { skillID = _id, skillName = _name };
    }

    public override string TranslateSkillDescription(string skillDescription, int _heroLevel)
    {
        string result = Translate.TranslateText(skillDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), skillDescriptionParts[number]);
        }
        return result;
    }
}

public class SkillLuckiness : Skill
{
    public SkillLuckiness(int _id, string _name) : base(_id, _name)
    {
        skillIcon = Resources.Load<Sprite>(SkillsDatabase.skillsIconsDirectory + skillName);

        requiredSkillsIDs = new int[1] { 4 };
        skillDescriptionParts = new string[0];
    }

    public override void OnSkillGain(int heroLevel)
    {
        base.OnSkillGain(heroLevel);
        player.ChangeStateBonus("crit_chance", 10);
    }

    public override Skill Clone(int _id, string _name)
    {
        // don't forget to change
        return new SkillLuckiness(_id, _name) { skillID = _id, skillName = _name };
    }

    public override string TranslateSkillDescription(string skillDescription, int _heroLevel)
    {
        string result = Translate.TranslateText(skillDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), skillDescriptionParts[number]);
        }
        return result;
    }
}

public class SkillStoic : Skill
{
    public SkillStoic(int _id, string _name) : base(_id, _name)
    {
        skillIcon = Resources.Load<Sprite>(SkillsDatabase.skillsIconsDirectory + skillName);

        requiredSkillsIDs = new int[1] { 3 };
        skillDescriptionParts = new string[1];
    }

    public override void OnSkillGain(int heroLevel)
    {
        base.OnSkillGain(heroLevel);
        player.ChangeStateBonus("protection_blunt", 1 + Mathf.RoundToInt(heroLevel / 3));
        player.ChangeStateBonus("protection_pricking", 1 + Mathf.RoundToInt(heroLevel / 3));
        player.ChangeStateBonus("protection_cutting", 1 + Mathf.RoundToInt(heroLevel / 3));
        player.ChangeStateBonus("protection_hewing", 1 + Mathf.RoundToInt(heroLevel / 3));
        player.ChangeStateBonus("protection_lashing", 1 + Mathf.RoundToInt(heroLevel / 3));
    }

    public override Skill Clone(int _id, string _name)
    {
        // don't forget to change
        return new SkillStoic(_id, _name) { skillID = _id, skillName = _name };
    }

    public override string TranslateSkillDescription(string skillDescription, int _heroLevel)
    {
        skillDescriptionParts[0] = (1 + Mathf.RoundToInt(_heroLevel / 3)).ToString();
        string result = Translate.TranslateText(skillDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), skillDescriptionParts[number]);
        }
        return result;
    }
}

public class SkillMagicalPatronage : Skill
{
    public SkillMagicalPatronage(int _id, string _name) : base(_id, _name)
    {
        skillIcon = Resources.Load<Sprite>(SkillsDatabase.skillsIconsDirectory + skillName);

        requiredSkillsIDs = new int[1] { 3 };
        skillDescriptionParts = new string[1];
    }

    public override void OnSkillGain(int heroLevel)
    {
        base.OnSkillGain(heroLevel);
        player.ChangeStateBonus("protection_suffocative", 1 + Mathf.RoundToInt(heroLevel / 3));
        player.ChangeStateBonus("protection_scalding", 1 + Mathf.RoundToInt(heroLevel / 3));
        player.ChangeStateBonus("protection_freezing", 1 + Mathf.RoundToInt(heroLevel / 3));
        player.ChangeStateBonus("protection_electric", 1 + Mathf.RoundToInt(heroLevel / 3));
        player.ChangeStateBonus("protection_poison", 1 + Mathf.RoundToInt(heroLevel / 3));
    }

    public override Skill Clone(int _id, string _name)
    {
        // don't forget to change
        return new SkillMagicalPatronage(_id, _name) { skillID = _id, skillName = _name };
    }

    public override string TranslateSkillDescription(string skillDescription, int _heroLevel)
    {
        skillDescriptionParts[0] = (1 + Mathf.RoundToInt(_heroLevel / 3)).ToString();
        string result = Translate.TranslateText(skillDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), skillDescriptionParts[number]);
        }
        return result;
    }
}

public class SkillSpiritualism : Skill
{
    public SkillSpiritualism(int _id, string _name) : base(_id, _name)
    {
        skillIcon = Resources.Load<Sprite>(SkillsDatabase.skillsIconsDirectory + skillName);

        requiredSkillsIDs = new int[1] { 10 };
        skillDescriptionParts = new string[3];
    }

    public override void OnSkillGain(int heroLevel)
    {
        base.OnSkillGain(heroLevel);
    }

    public override void OnTurnStart()
    {
        base.OnTurnStart();
        for (int i = 0; i < player.creaturesKilledInThisTurnPositions.Count; i++)
        {
            if (Random.Range(0f, 100f) < 66f)
            {
                MapCell cell = player.map.cells[player.creaturesKilledInThisTurnPositions[i].x, player.creaturesKilledInThisTurnPositions[i].y];
                Creature spirit = player.map.SpawnCreature(player.map.creaturesContainer.GetCreatureByName("friendly_awakened_spirit"), cell.x, cell.y);
                if (player.GetComponent<PlayerController>() != null)
                    spirit.isSummonedByPlayer = true;
                spirit.ChangeStateBonus("max_health", 5 + 15 * player.GetStateValue("level"));
                spirit.GetStateByName("max_health").startStateValue = 5 + 15 * player.GetStateValue("level") + 1;
                spirit.SetStateValue("cur_health", 5 + 15 * player.GetStateValue("level") + 1);
                spirit.ChangeStateBonus("max_energy", player.GetStateValue("level") / 10 + 1);
                spirit.GetStateByName("max_energy").startStateValue = player.GetStateValue("level") / 10 + 1;
                spirit.SetStateValue("cur_energy", player.GetStateValue("level") / 10 + 1);
                spirit.ChangeStateBonus("attack_damage", 6 * player.GetStateValue("level"));
                spirit.GetStateByName("attack_damage").startStateValue = 6 * player.GetStateValue("level");
            }
        }
    }

    public override Skill Clone(int _id, string _name)
    {
        // don't forget to change
        return new SkillSpiritualism(_id, _name) { skillID = _id, skillName = _name };
    }

    public override string TranslateSkillDescription(string skillDescription, int _heroLevel)
    {
        skillDescriptionParts[0] = (1 + _heroLevel / 10).ToString();
        skillDescriptionParts[1] = (5 + 15 * _heroLevel).ToString();
        skillDescriptionParts[2] = (6 * _heroLevel).ToString();
        string result = Translate.TranslateText(skillDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), skillDescriptionParts[number]);
        }
        return result;
    }
}