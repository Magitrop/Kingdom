using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsDatabase
{
    public static List<Skill> registeredSkills = new List<Skill>();
    public static string skillsIconsDirectory;

    public static void InitializeDatabase()
    {
        skillsIconsDirectory = "SkillsSprites/";

        registeredSkills.Clear();
        registeredSkills.Add(new SkillOccultism(0, "skill_occultism"));
        registeredSkills.Add(new SkillVitality(1, "skill_vitality"));
        registeredSkills.Add(new SkillStamina(2, "skill_stamina"));
        registeredSkills.Add(new SkillStrengthening(3, "skill_strengthening"));
        registeredSkills.Add(new SkillArmament(4, "skill_armament"));
        registeredSkills.Add(new SkillMedium(5, "skill_medium"));
        registeredSkills.Add(new SkillThickSkin(6, "skill_thick_skin"));
        registeredSkills.Add(new SkillSharpenedBlades(7, "skill_sharpened_blades"));
        registeredSkills.Add(new SkillPlateArmor(8, "skill_plate_armor"));
        registeredSkills.Add(new SkillAgility(9, "skill_agility"));
        registeredSkills.Add(new SkillMysticism(10, "skill_mysticism"));
        registeredSkills.Add(new SkillJaggedBlades(11, "skill_jagged_blades"));
        registeredSkills.Add(new SkillThorns(12, "skill_thorns"));
        registeredSkills.Add(new SkillAutosuggestion(13, "skill_autosuggestion"));
        registeredSkills.Add(new SkillSynarchy(14, "skill_synarchy"));
        registeredSkills.Add(new SkillHeavyStrike(15, "skill_heavy_strike"));
        registeredSkills.Add(new SkillLuckiness(16, "skill_luckiness"));
        registeredSkills.Add(new SkillSpiritualism(17, "skill_spiritualism"));
        registeredSkills.Add(new SkillStoic(18, "skill_stoic"));
        registeredSkills.Add(new SkillMagicalPatronage(19, "skill_magical_patronage"));
    }

    public static Skill GetSkillByID(int ID)
    {
        for (int i = 0; i < registeredSkills.Count; i++)
            if (registeredSkills[i].skillID == ID)
                return registeredSkills[i].Clone(ID, registeredSkills[i].skillName);
        return registeredSkills[0].Clone(ID, registeredSkills[0].skillName);
    }

    public static bool CanSkillBeGained(Skill skillToGain, List<Skill> gainedSkills)
    {
        for (int i = 0; i < skillToGain.requiredSkillsIDs.Length; i++)
        {
            bool interResult = false;
            for (int j = 0; j < gainedSkills.Count; j++)
            {
                if (gainedSkills[j].skillID == skillToGain.requiredSkillsIDs[i])
                {
                    interResult = true;
                    break;
                }
            }
            if (interResult == false)
                return false;
        }
        return true;
    }
}