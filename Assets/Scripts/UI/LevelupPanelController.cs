using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelupPanelController : MonoBehaviour
{
    public PlayerController player;
    public UISkillChooseButton[] skillsButtons;
    public int unusedSkillPoints;

    List<int> unusedIDs = new List<int>();
    public void PrepareSkills()
    {
        player.preparedSkillsIDs.Clear();
        for (int i = 0; i < skillsButtons.Length; i++)
        {
            unusedIDs.Clear();
            for (int j = 0; j < SkillsDatabase.registeredSkills.Count; j++)
                unusedIDs.Add(SkillsDatabase.registeredSkills[j].skillID);
            int n = unusedIDs[Random.Range(0, unusedIDs.Count)];
            while (player.gainedSkills.Any(s => s.skillID == n) == true 
                || player.preparedSkillsIDs.Contains(n) == true 
                || SkillsDatabase.CanSkillBeGained(SkillsDatabase.GetSkillByID(n), player.gainedSkills) == false)
            {
                unusedIDs.Remove(n);
                if (unusedIDs.Count < 1)
                    return;
                n = unusedIDs[Random.Range(0, unusedIDs.Count)];
            }

            Skill curr = SkillsDatabase.registeredSkills[n];
            if (SkillsDatabase.CanSkillBeGained(curr, player.gainedSkills))
            {
                /*
                skillsButtons[i].skillID = n;
                skillsButtons[i].heroLevel = player.GetStateValue("level");
                skillsButtons[i].skillName = curr.skillName;
                skillsButtons[i].skillDescription = curr.TranslateSkillDescription(curr.skillName + "_desc", player.GetStateValue("level"));
                skillsButtons[i].skillIcon.sprite = curr.skillIcon;
                */
                player.preparedSkillsIDs.Add(n);
            }
        }
    }

    public void CheckSkills()
    {
        for (int i = 0; i < player.preparedSkillsIDs.Count; i++)
        {
            unusedIDs.Clear();
            Skill curr = SkillsDatabase.registeredSkills[player.preparedSkillsIDs[i]];
            if (SkillsDatabase.CanSkillBeGained(curr, player.gainedSkills) == false)
            {
                for (int j = 0; j < SkillsDatabase.registeredSkills.Count; j++)
                    unusedIDs.Add(SkillsDatabase.registeredSkills[j].skillID);
                int n = unusedIDs[Random.Range(0, unusedIDs.Count)];
                while (player.gainedSkills.Any(s => s.skillID == n) == true
                    || player.preparedSkillsIDs.Contains(n) == true
                    || SkillsDatabase.CanSkillBeGained(SkillsDatabase.GetSkillByID(n), player.gainedSkills) == false)
                {
                    unusedIDs.Remove(n);
                    if (unusedIDs.Count < 1)
                        return;
                    n = unusedIDs[Random.Range(0, unusedIDs.Count)];
                }
                curr = SkillsDatabase.registeredSkills[n];
            }
            player.preparedSkillsIDs[i] = curr.skillID;
        }
    }

    public void ShowSkills()
    {
        CheckSkills();
        for (int i = 0; i < skillsButtons.Length; i++)
        {
            /*
            unusedIDs.Clear();
            for (int j = 0; j < SkillsDatabase.registeredSkills.Count; j++)
                unusedIDs.Add(SkillsDatabase.registeredSkills[j].skillID);
            int n = unusedIDs[Random.Range(0, unusedIDs.Count)];
            while (player.gainedSkills.Any(s => s.skillID == n) == true
                || preparedIDs.Contains(n) == true
                || SkillsDatabase.CanSkillBeGained(SkillsDatabase.GetSkillByID(n), player.gainedSkills) == false)
            {
                unusedIDs.Remove(n);
                if (unusedIDs.Count < 1)
                    return;
                n = unusedIDs[Random.Range(0, unusedIDs.Count)];
            }
            */
            skillsButtons[i].gameObject.SetActive(true);
            if (i < player.preparedSkillsIDs.Count)
            {
                int n = player.preparedSkillsIDs[i];
                Skill curr = SkillsDatabase.registeredSkills[n];
                //if (SkillsDatabase.CanSkillBeGained(curr, player.gainedSkills))
                {
                    skillsButtons[i].skillID = n;
                    skillsButtons[i].heroLevel = player.GetStateValue("level");
                    skillsButtons[i].skillName = curr.skillName;
                    skillsButtons[i].skillDescription = curr.TranslateSkillDescription(curr.skillName + "_desc", player.GetStateValue("level"));
                    skillsButtons[i].skillIcon.sprite = curr.skillIcon;
                }
            }
            else skillsButtons[i].gameObject.SetActive(false);
        }
    }
}