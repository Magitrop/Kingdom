using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EffectType
{
    Buff,
    Debuff
}

[System.Serializable]
public class Effect
{
    public int effectID;
    public string effectName;
    /// <summary>
    /// A creature who handle this effect and is affected by its influence.
    /// </summary>
    public Creature effectHandler;
    /// <summary>
    /// A creature who applied this effect on effectHander.
    /// </summary>
    public Creature effectSender;
    /// <summary>
    /// Duration of this effect measured in turns.
    /// </summary>
    public int effectDuration;
    public int effectLevel;
    public EffectType effectType;
    public bool hasEnded;

    public Sprite effectIcon;
    public VisualEffect connectedVisualEffect;
    // for player only
    public EffectIcon connectedIcon;

    protected string[] effectDescriptionParts;

    public Effect(int _id, string _name)
    {
        effectID = _id;
        effectName = _name;
    }

    public virtual void OnEffectBegin() { }
    public virtual void OnEffectEnd() { }

    public virtual void OnTurnStart() { }
    public virtual void OnTurnEnd() { }
    public virtual int OnCreatureDamage(Creature _sender, DamageType _damageType, int _damage) { return 0; } // should return how much damage will be canceled after this function
    public virtual void OnCreatureHeal() { }
    public virtual void OnCreatureDeath() { }
    public virtual void OnCreatureKill(Creature creature) { }
    public virtual void OnCreatureCastSpell(Spell spell) { }
    public virtual void OnCreatureMove() { }

    public static void ApplyEffect(Effect effectToApply, Creature effectHandler, Creature effectSender, int duration, int level)
    {
        //MonoBehaviour.print(effectHandler);
        if (effectHandler == null || effectHandler.canBeAffectedByEffects == false)
            return;
        for (int i = 0; i < effectHandler.currentEffects.Count; i++)
        {
            if (effectHandler.currentEffects[i].effectID == effectToApply.effectID)
            {
                effectHandler.currentEffects[i].OnEffectEnd();
                if (effectHandler.currentEffects[i].effectLevel < level)
                    effectHandler.currentEffects[i].effectLevel = level;
                if (effectHandler.currentEffects[i].effectDuration < duration)
                    effectHandler.currentEffects[i].effectDuration = duration;
                effectHandler.currentEffects[i].effectSender = effectSender;
                effectHandler.currentEffects[i].effectHandler = effectHandler;
                effectHandler.currentEffects[i].OnEffectBegin();
                if (effectHandler.GetComponent<PlayerController>() != null)
                {
                    if (effectHandler.currentEffects[i].connectedIcon != null)
                    {
                        effectHandler.currentEffects[i].connectedIcon.effectLevel = effectHandler.currentEffects[i].effectLevel;
                        effectHandler.currentEffects[i].connectedIcon.effectName = effectHandler.currentEffects[i].effectName;
                        effectHandler.currentEffects[i].connectedIcon.effectDescription = effectHandler.currentEffects[i].TranslateEffectDescription(effectHandler.currentEffects[i].effectName + "_desc");
                    }
                }
                return;
            }
        }

        //effectHandler.currentEffects[effectHandler.currentEffects.Count - 1].effectDuration = duration;
        effectToApply.effectDuration = duration;
        effectToApply.effectLevel = level;
        effectToApply.effectSender = effectSender;
        effectToApply.effectHandler = effectHandler;
        effectToApply.OnEffectBegin();
        effectHandler.currentEffects.Add(effectToApply);
        if (effectHandler.GetComponent<PlayerController>() != null)
        {
            effectHandler.GetComponent<PlayerController>().OnEffectBegin(effectToApply);
        }
    }

    // SHOULD BE OVERRIDDEN
    public virtual Effect Clone(int _id, string _name)
    {
        return new Effect(_id, _name) { effectID = _id, effectName = _name };
    }

    public override string ToString()
    {
        return effectID + ";" + effectHandler.cellObjectID + ";" + effectSender.cellObjectID + ";" + effectDuration + ";" + effectLevel;
    }

    public virtual string TranslateEffectDescription(string effectDescription)
    {
        return "effect not found";
    }
}

[System.Serializable]
public class EffectPoison : Effect
{
    public EffectPoison(int _id, string _name) : base(_id, _name)
    {
        effectType = EffectType.Debuff;
        effectIcon = Resources.Load<Sprite>(EffectsDatabase.effectsIconsDirectory + effectName);

        effectDescriptionParts = new string[1];
    }

    public override void OnEffectBegin()
    {
        base.OnEffectBegin();
        effectDescriptionParts[0] = (7 * effectLevel).ToString();
        connectedVisualEffect = effectHandler.PlayVisualEffect("effect_poison_idle");
    }

    public override void OnEffectEnd()
    {
        base.OnEffectEnd();
        if (connectedVisualEffect != null)
            connectedVisualEffect.EndEffect();
    }

    public override void OnTurnStart()
    {
        base.OnTurnStart();
        effectHandler.GetDamage(7 * effectLevel, DamageType.poison, effectSender);
        effectHandler.PlayVisualEffect("effect_poison_damage");
    }

    public override Effect Clone(int _id, string _name)
    {
        // don't forget to change
        return new EffectPoison(_id, _name) { effectID = _id, effectName = _name };
    }

    public override string TranslateEffectDescription(string effectDescription)
    {
        effectDescriptionParts[0] = (7 * effectLevel).ToString();
        string result = Translate.TranslateText(effectDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), effectDescriptionParts[number]);
        }
        return result;
    }
}

[System.Serializable]
public class EffectBleeding : Effect
{
    public EffectBleeding(int _id, string _name) : base(_id, _name)
    {
        effectType = EffectType.Debuff;
        effectIcon = Resources.Load<Sprite>(EffectsDatabase.effectsIconsDirectory + effectName);

        effectDescriptionParts = new string[1];
    }

    public override void OnEffectBegin()
    {
        base.OnEffectBegin();
        effectDescriptionParts[0] = (Mathf.RoundToInt((float)(effectHandler.GetStateValue("max_health") * 0.01 * effectLevel))).ToString();
        connectedVisualEffect = effectHandler.PlayVisualEffect("effect_bleeding_idle");
    }

    public override void OnEffectEnd()
    {
        base.OnEffectEnd();
        if (connectedVisualEffect != null)
            connectedVisualEffect.EndEffect();
    }

    public override void OnTurnStart()
    {
        base.OnTurnStart();
        effectHandler.GetDamage(Mathf.RoundToInt((float)(effectHandler.GetStateValue("max_health") * 0.01 * effectLevel)), DamageType.pure, effectSender);
        effectHandler.PlayVisualEffect("effect_bleeding_damage");
    }

    public override Effect Clone(int _id, string _name)
    {
        // don't forget to change
        return new EffectBleeding(_id, _name) { effectID = _id, effectName = _name };
    }

    public override string TranslateEffectDescription(string effectDescription)
    {
        effectDescriptionParts[0] = (Mathf.RoundToInt((float)(effectHandler.GetStateValue("max_health") * 0.01 * effectLevel))).ToString();
        string result = Translate.TranslateText(effectDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), effectDescriptionParts[number]);
        }
        return result;
    }
}

[System.Serializable]
public class EffectFear : Effect
{
    public EffectFear(int _id, string _name) : base(_id, _name)
    {
        effectType = EffectType.Debuff;
        effectIcon = Resources.Load<Sprite>(EffectsDatabase.effectsIconsDirectory + effectName);

        effectDescriptionParts = new string[1];
    }

    public override void OnEffectBegin()
    {
        base.OnEffectBegin();
        effectDescriptionParts[0] = (10 * effectLevel).ToString();
        effectHandler.ChangeStateBonus("attack_damage", -10 * effectLevel);
        effectHandler.PlayVisualEffect("effect_fear_start");
        connectedVisualEffect = effectHandler.PlayVisualEffect("effect_fear_idle");
    }

    public override void OnEffectEnd()
    {
        base.OnEffectBegin();
        effectHandler.ChangeStateBonus("attack_damage", 10 * effectLevel);
        if (connectedVisualEffect != null)
            connectedVisualEffect.EndEffect();
    }

    public override Effect Clone(int _id, string _name)
    {
        // don't forget to change
        return new EffectFear(_id, _name) { effectID = _id, effectName = _name };
    }

    public override string TranslateEffectDescription(string effectDescription)
    {
        effectDescriptionParts[0] = (10 * effectLevel).ToString();
        string result = Translate.TranslateText(effectDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), effectDescriptionParts[number]);
        }
        return result;
    }
}

[System.Serializable]
public class EffectCombustion : Effect
{
    public EffectCombustion(int _id, string _name) : base(_id, _name)
    {
        effectType = EffectType.Debuff;
        effectIcon = Resources.Load<Sprite>(EffectsDatabase.effectsIconsDirectory + effectName);

        effectDescriptionParts = new string[1];
    }

    public override void OnEffectBegin()
    {
        base.OnEffectBegin();
        effectDescriptionParts[0] = (5 * effectLevel).ToString() + "-" + (10 * effectLevel).ToString();
        connectedVisualEffect = effectHandler.PlayVisualEffect("effect_combustion_idle");
    }

    public override void OnTurnStart()
    {
        base.OnTurnStart();
        effectHandler.GetDamage(Random.Range(5, 11) * effectLevel, DamageType.scalding, effectSender);
        effectHandler.PlayVisualEffect("effect_combustion_damage");
    }

    public override void OnEffectEnd()
    {
        base.OnEffectEnd();
        if (connectedVisualEffect != null)
            connectedVisualEffect.EndEffect();
    }

    public override Effect Clone(int _id, string _name)
    {
        // don't forget to change
        return new EffectCombustion(_id, _name) { effectID = _id, effectName = _name };
    }

    public override string TranslateEffectDescription(string effectDescription)
    {
        effectDescriptionParts[0] = (5 * effectLevel).ToString() + "-" + (10 * effectLevel).ToString();
        string result = Translate.TranslateText(effectDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), effectDescriptionParts[number]);
        }
        return result;
    }
}

[System.Serializable]
public class EffectMotionCurse : Effect
{
    public EffectMotionCurse(int _id, string _name) : base(_id, _name)
    {
        effectType = EffectType.Debuff;
        effectIcon = Resources.Load<Sprite>(EffectsDatabase.effectsIconsDirectory + effectName);

        effectDescriptionParts = new string[1];
    }

    public override void OnEffectBegin()
    {
        base.OnEffectBegin();
        effectDescriptionParts[0] = (5 * effectLevel).ToString();
        connectedVisualEffect = effectHandler.PlayVisualEffect("effect_motion_curse_idle");
    }

    public override void OnCreatureMove()
    {
        base.OnCreatureMove();
        effectHandler.GetDamage(5 * effectLevel, DamageType.scalding, effectSender);
        effectHandler.PlayVisualEffect("effect_motion_curse_damage");
    }

    public override void OnEffectEnd()
    {
        base.OnEffectEnd();
        if (connectedVisualEffect != null)
            connectedVisualEffect.EndEffect();
    }

    public override Effect Clone(int _id, string _name)
    {
        // don't forget to change
        return new EffectMotionCurse(_id, _name) { effectID = _id, effectName = _name };
    }

    public override string TranslateEffectDescription(string effectDescription)
    {
        string result = Translate.TranslateText(effectDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), effectDescriptionParts[number]);
        }
        return result;
    }
}

[System.Serializable]
public class EffectSlowdown : Effect
{
    public EffectSlowdown(int _id, string _name) : base(_id, _name)
    {
        effectType = EffectType.Debuff;
        effectIcon = Resources.Load<Sprite>(EffectsDatabase.effectsIconsDirectory + effectName);

        effectDescriptionParts = new string[1];
    }

    public override void OnEffectBegin()
    {
        base.OnEffectBegin();
        effectDescriptionParts[0] = effectLevel.ToString();
        effectHandler.ChangeStateBonus("max_energy", -effectLevel);
        connectedVisualEffect = effectHandler.PlayVisualEffect("effect_slowndown_idle");
    }

    public override void OnEffectEnd()
    {
        base.OnEffectBegin();
        effectHandler.ChangeStateBonus("max_energy", effectLevel);
        if (connectedVisualEffect != null)
            connectedVisualEffect.EndEffect();
    }

    public override Effect Clone(int _id, string _name)
    {
        // don't forget to change
        return new EffectSlowdown(_id, _name) { effectID = _id, effectName = _name };
    }

    public override string TranslateEffectDescription(string effectDescription)
    {
        effectDescriptionParts[0] = (10 * effectLevel).ToString();
        string result = Translate.TranslateText(effectDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), effectDescriptionParts[number]);
        }
        return result;
    }
}

[System.Serializable]
public class EffectFrost : Effect
{
    public EffectFrost(int _id, string _name) : base(_id, _name)
    {
        effectType = EffectType.Debuff;
        effectIcon = Resources.Load<Sprite>(EffectsDatabase.effectsIconsDirectory + effectName);

        effectDescriptionParts = new string[1];
    }

    public override void OnEffectBegin()
    {
        base.OnEffectBegin();
        effectDescriptionParts[0] = effectLevel.ToString();
        effectHandler.ChangeStateBonus("max_energy", -9999);
        connectedVisualEffect = effectHandler.PlayVisualEffect("spell_icing");
        effectHandler.animator.enabled = false;
    }

    public override void OnEffectEnd()
    {
        base.OnEffectBegin();
        effectHandler.ChangeStateBonus("max_energy", 9999);
        if (connectedVisualEffect != null)
            connectedVisualEffect.animator.SetBool("ended", true);
        effectHandler.animator.enabled = true;
    }

    public override Effect Clone(int _id, string _name)
    {
        // don't forget to change
        return new EffectFrost(_id, _name) { effectID = _id, effectName = _name };
    }

    public override string TranslateEffectDescription(string effectDescription)
    {
        effectDescriptionParts[0] = (10 * effectLevel).ToString();
        string result = Translate.TranslateText(effectDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), effectDescriptionParts[number]);
        }
        return result;
    }
}

[System.Serializable]
public class EffectFrostbite : Effect
{
    int energyReduction;

    public EffectFrostbite(int _id, string _name) : base(_id, _name)
    {
        effectType = EffectType.Debuff;
        effectIcon = Resources.Load<Sprite>(EffectsDatabase.effectsIconsDirectory + effectName);

        effectDescriptionParts = new string[3];
    }

    public override void OnEffectBegin()
    {
        base.OnEffectBegin();
        effectDescriptionParts[0] = (effectLevel / 17 + 1).ToString();
        effectDescriptionParts[1] = (10 + effectLevel * 8).ToString();
        effectDescriptionParts[2] = (effectLevel / 21 + 1).ToString();
        energyReduction =
            effectHandler.GetStateValue("max_energy") > (effectLevel / 17 + 1) ?
            (effectLevel / 17 + 1) :
            effectHandler.GetStateValue("max_energy") - 1;
        effectHandler.ChangeStateBonus("max_energy", -energyReduction);
        connectedVisualEffect = effectHandler.PlayVisualEffect("effect_frostbite_idle");
    }

    public override void OnCreatureMove()
    {
        base.OnCreatureMove();
        effectHandler.GetDamage(10 + effectLevel * 8, DamageType.freezing, effectSender);
        effectHandler.PlayVisualEffect("effect_frostbite_damage");
    }

    public override void OnEffectEnd()
    {
        base.OnEffectEnd();
        effectHandler.ChangeStateBonus("max_energy", energyReduction);
        if (connectedVisualEffect != null)
            connectedVisualEffect.EndEffect();
    }

    public override Effect Clone(int _id, string _name)
    {
        // don't forget to change
        return new EffectFrostbite(_id, _name) { effectID = _id, effectName = _name };
    }

    public override string TranslateEffectDescription(string effectDescription)
    {
        string result = Translate.TranslateText(effectDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), effectDescriptionParts[number]);
        }
        return result;
    }
}

[System.Serializable]
public class EffectFrozenShield : Effect
{
    int damageReduction;
    public EffectFrozenShield(int _id, string _name) : base(_id, _name)
    {
        effectType = EffectType.Buff;
        effectIcon = Resources.Load<Sprite>(EffectsDatabase.effectsIconsDirectory + effectName);

        effectDescriptionParts = new string[2];
    }

    public override void OnEffectBegin()
    {
        base.OnEffectBegin();
        damageReduction = 3 + 5 * effectLevel;
        effectDescriptionParts[0] = (effectLevel / 23 + 1).ToString();
        effectDescriptionParts[1] = (3 + 5 * effectLevel).ToString();
        connectedVisualEffect = effectHandler.PlayVisualEffect("effect_frozen_shield_appear");
    }

    public override int OnCreatureDamage(Creature _sender, DamageType _damageType, int _damage)
    {
        base.OnCreatureDamage(_sender, _damageType, _damage);
        if (_damageType == DamageType.freezing)
            return _damage;
        return damageReduction;
    }

    public override void OnEffectEnd()
    {
        base.OnEffectEnd();
        if (connectedVisualEffect != null)
            connectedVisualEffect.EndEffect();
    }

    public override Effect Clone(int _id, string _name)
    {
        // don't forget to change
        return new EffectFrozenShield(_id, _name) { effectID = _id, effectName = _name };
    }

    public override string TranslateEffectDescription(string effectDescription)
    {
        string result = Translate.TranslateText(effectDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), effectDescriptionParts[number]);
        }
        return result;
    }
}

[System.Serializable]
public class EffectChills : Effect
{
    int protectionReduction;
    public EffectChills(int _id, string _name) : base(_id, _name)
    {
        effectType = EffectType.Debuff;
        effectIcon = Resources.Load<Sprite>(EffectsDatabase.effectsIconsDirectory + effectName);

        effectDescriptionParts = new string[1];
    }

    public override void OnEffectBegin()
    {
        base.OnEffectBegin();
        protectionReduction = effectLevel / 4 + 1;
        effectDescriptionParts[0] = (effectLevel / 4 + 1).ToString();
        effectHandler.ChangeStateBonus("protection_freezing", -protectionReduction);
        connectedVisualEffect = effectHandler.PlayVisualEffect("effect_chills");
    }

    public override void OnEffectEnd()
    {
        base.OnEffectEnd();
        if (connectedVisualEffect != null)
            connectedVisualEffect.EndEffect();
        effectHandler.ChangeStateBonus("protection_freezing", protectionReduction);
    }

    public override Effect Clone(int _id, string _name)
    {
        // don't forget to change
        return new EffectChills(_id, _name) { effectID = _id, effectName = _name };
    }

    public override string TranslateEffectDescription(string effectDescription)
    {
        string result = Translate.TranslateText(effectDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), effectDescriptionParts[number]);
        }
        return result;
    }
}

[System.Serializable]
public class EffectGlacialWisdom : Effect
{
    int spellPowerForIce;
    public EffectGlacialWisdom(int _id, string _name) : base(_id, _name)
    {
        effectType = EffectType.Buff;
        effectIcon = Resources.Load<Sprite>(EffectsDatabase.effectsIconsDirectory + effectName);

        effectDescriptionParts = new string[1];
    }

    public override void OnEffectBegin()
    {
        base.OnEffectBegin();
        spellPowerForIce = effectLevel / 4 + 1;
        effectDescriptionParts[0] = (effectLevel / 4 + 1).ToString();
        connectedVisualEffect = effectHandler.PlayVisualEffect("effect_glacial_wisdom");
        effectHandler.ChangeStateBonus("spell_power_bonus_ice", spellPowerForIce);
    }

    public override void OnCreatureCastSpell(Spell spell)
    {
        base.OnCreatureCastSpell(spell);
        if (spell.spellTypes.Contains(SpellType.ice))
            spell.spellLevel += spellPowerForIce;
    }

    public override void OnEffectEnd()
    {
        base.OnEffectEnd();
        if (connectedVisualEffect != null)
            connectedVisualEffect.EndEffect();
        effectHandler.ChangeStateBonus("spell_power_bonus_ice", -spellPowerForIce);
    }

    public override Effect Clone(int _id, string _name)
    {
        // don't forget to change
        return new EffectGlacialWisdom(_id, _name) { effectID = _id, effectName = _name };
    }

    public override string TranslateEffectDescription(string effectDescription)
    {
        string result = Translate.TranslateText(effectDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), effectDescriptionParts[number]);
        }
        return result;
    }
}

[System.Serializable]
public class EffectAutosuggestion : Effect
{
    public EffectAutosuggestion(int _id, string _name) : base(_id, _name)
    {
        effectType = EffectType.Buff;
        effectIcon = Resources.Load<Sprite>(EffectsDatabase.effectsIconsDirectory + effectName);
        if (effectIcon == null)
            effectIcon = Resources.Load<Sprite>(EffectsDatabase.effectsIconsDirectory + "none");

        effectDescriptionParts = new string[1];
    }

    public override void OnEffectBegin()
    {
        base.OnEffectBegin();
        effectDescriptionParts[0] = (4 + 4 * effectLevel).ToString();
        //connectedVisualEffect = effectHandler.PlayVisualEffect("effect_autosuggestion");
    }

    public override void OnCreatureMove()
    {
        base.OnCreatureMove();
        if (Random.Range(0f, 100f) < 33f)
            effectHandler.GetHealing(4 + 4 * effectLevel);
    }

    public override void OnEffectEnd()
    {
        base.OnEffectEnd();
        if (connectedVisualEffect != null)
            connectedVisualEffect.EndEffect();
    }

    public override Effect Clone(int _id, string _name)
    {
        // don't forget to change
        return new EffectAutosuggestion(_id, _name) { effectID = _id, effectName = _name };
    }

    public override string TranslateEffectDescription(string effectDescription)
    {
        string result = Translate.TranslateText(effectDescription).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), effectDescriptionParts[number]);
        }
        return result;
    }
}