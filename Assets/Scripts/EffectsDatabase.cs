using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsDatabase
{
    public static List<Effect> registeredEffects = new List<Effect>();
    public static string effectsIconsDirectory;

    public static void InitializeDatabase()
    {
        effectsIconsDirectory = "EffectsSprites/";

        registeredEffects.Clear();
        registeredEffects.Add(new EffectPoison(0, "effect_poison"));
        registeredEffects.Add(new EffectBleeding(1, "effect_bleeding"));
        registeredEffects.Add(new EffectFear(2, "effect_fear"));
        registeredEffects.Add(new EffectCombustion(3, "effect_combustion"));
        registeredEffects.Add(new EffectMotionCurse(4, "effect_motion_curse"));
        registeredEffects.Add(new EffectSlowdown(5, "effect_slowdown"));
        registeredEffects.Add(new EffectFrost(6, "effect_frost"));
        registeredEffects.Add(new EffectFrostbite(7, "effect_frostbite"));
        registeredEffects.Add(new EffectFrozenShield(8, "effect_frozen_shield"));
        registeredEffects.Add(new EffectChills(9, "effect_chills"));
        registeredEffects.Add(new EffectGlacialWisdom(10, "effect_glacial_wisdom"));
        registeredEffects.Add(new EffectAutosuggestion(11, "effect_autosuggestion"));
    }

    public static Effect GetEffectByID(int ID)
    {
        for (int i = 0; i < registeredEffects.Count; i++)
            if (registeredEffects[i].effectID == ID)
                return registeredEffects[i].Clone(ID, registeredEffects[i].effectName);
        return registeredEffects[0].Clone(ID, registeredEffects[0].effectName);
    }
}