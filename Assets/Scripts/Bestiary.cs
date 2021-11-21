using MiniJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class Bestiary : MonoBehaviour
{
    public List<CreatureInformation> knownCreatures = new List<CreatureInformation>();
    public int currentCreatureIndex;

    public Text nameText;
    public Text descriptionText;
    public Text
        healthText,
        energyText,
        attackDamageText,
        levelText
        /*
        bluntProtectionText,
        prickingProtectionText,
        cuttingProtectionText,
        hewingProtectionText,
        lashingProtectionText,
        suffocativeProtectionText,
        scaldingProtectionText,
        freezingProtectionText,
        electricProtectionText,
        poisonProtectionText*/;
    public Text[] protectionTexts;

    public Animator creatureImage;

    public Text traitsLabel;
    public Text traitExemplar;
    public GameObject traitsContainer;
    public GameObject nextPageButton, prevPageButton, nextLevelButton, prevLevelButton;

    private List<Text> instantiatedTraits = new List<Text>();
    private string bestiaryFilePath;

    public void Initialize()
    {
        bestiaryFilePath = Application.dataPath + "/Bestiary/bestiary.xml";
        if (Directory.Exists(Application.dataPath + "/Bestiary/") == false)
            Directory.CreateDirectory(Application.dataPath + "/Bestiary/");

        LoadBestiary();

        traitsLabel.text = Translate.TranslateText(traitsLabel.text);
        if (gameObject.activeSelf == true)
            creatureImage.Play("bestiary_creature_none");

        prevLevelButton.gameObject.SetActive(false);
    }

    void Update()
    {
        if (knownCreatures.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                int prevCreatureIndex = currentCreatureIndex;
                while (knownCreatures[currentCreatureIndex].creatureNameID == knownCreatures[prevCreatureIndex].creatureNameID)
                    if (currentCreatureIndex < knownCreatures.Count - 1)
                        currentCreatureIndex++;
                    else
                    {
                        currentCreatureIndex = prevCreatureIndex;
                        break;
                    }
                //nextPageButton.SetActive(true);
                //else nextPageButton.SetActive(false);
                nextPageButton.SetActive(currentCreatureIndex < knownCreatures.Count - 1 && currentCreatureIndex != prevCreatureIndex);
                prevPageButton.SetActive(currentCreatureIndex > 0);
                nextLevelButton.gameObject.SetActive(currentCreatureIndex < knownCreatures.Count - 1 && knownCreatures[currentCreatureIndex + 1].creatureNameID == knownCreatures[currentCreatureIndex].creatureNameID);
                prevLevelButton.gameObject.SetActive(false);
                LoadCurrentCreature();
                levelText.text = knownCreatures[currentCreatureIndex].creatureLevel.ToString() + " lvl";
                //currentCreatureIndex = currentCreatureIndex < knownCreatures.Count - 1 ? currentCreatureIndex + 1 : currentCreatureIndex;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                int prevCreatureIndex = currentCreatureIndex;
                while (knownCreatures[currentCreatureIndex].creatureNameID == knownCreatures[prevCreatureIndex].creatureNameID && currentCreatureIndex > 0)
                    currentCreatureIndex--;
                prevCreatureIndex = currentCreatureIndex;
                while (knownCreatures[currentCreatureIndex].creatureNameID == knownCreatures[prevCreatureIndex].creatureNameID && currentCreatureIndex > 0)
                    currentCreatureIndex--;
                if (currentCreatureIndex != 0)
                    currentCreatureIndex++;
                //nextPageButton.SetActive(true);
                //else nextPageButton.SetActive(false);
                nextPageButton.SetActive(currentCreatureIndex < knownCreatures.Count - 1);
                prevPageButton.SetActive(currentCreatureIndex > 0);
                nextLevelButton.SetActive(currentCreatureIndex < knownCreatures.Count - 1 && knownCreatures[currentCreatureIndex + 1].creatureNameID == knownCreatures[currentCreatureIndex].creatureNameID);
                prevLevelButton.SetActive(false);
                LoadCurrentCreature();
                levelText.text = knownCreatures[currentCreatureIndex].creatureLevel.ToString() + " lvl";
                //currentCreatureIndex = currentCreatureIndex > 0 ? currentCreatureIndex - 1 : currentCreatureIndex;
            }
        }
    }

    public void OpenBestiary()
    {
        int prevCreatureIndex = currentCreatureIndex;
        bool thereAreAnotherCreatures = true;
        while (knownCreatures[currentCreatureIndex].creatureNameID == knownCreatures[prevCreatureIndex].creatureNameID)
            if (currentCreatureIndex < knownCreatures.Count - 1)
                currentCreatureIndex++;
            else
            {
                thereAreAnotherCreatures = false;
                break;
            }
        currentCreatureIndex = 0;
        nextPageButton.SetActive(thereAreAnotherCreatures);
        prevPageButton.SetActive(false);
        nextLevelButton.gameObject.SetActive(currentCreatureIndex < knownCreatures.Count - 1 && knownCreatures[currentCreatureIndex + 1].creatureNameID == knownCreatures[currentCreatureIndex].creatureNameID);
        prevLevelButton.gameObject.SetActive(false);
        LoadCurrentCreature();
        levelText.text = knownCreatures[currentCreatureIndex].creatureLevel.ToString() + " lvl";
    }

    public void LoadCreatureLevel(int nextLevel)
    {
        if (nextLevel == 1)
        {
            if (currentCreatureIndex < knownCreatures.Count - 1 && knownCreatures[currentCreatureIndex + 1].creatureNameID == knownCreatures[currentCreatureIndex].creatureNameID)
            {
                currentCreatureIndex++;
                nextLevelButton.SetActive(currentCreatureIndex < knownCreatures.Count - 1 && knownCreatures[currentCreatureIndex + 1].creatureNameID == knownCreatures[currentCreatureIndex].creatureNameID);
                prevLevelButton.SetActive(true);
                LoadCurrentCreature();
                levelText.text = knownCreatures[currentCreatureIndex].creatureLevel.ToString() + " lvl";
            }
        }
        else
        {
            if (currentCreatureIndex > 0 && knownCreatures[currentCreatureIndex - 1].creatureNameID == knownCreatures[currentCreatureIndex].creatureNameID)
            {
                currentCreatureIndex--;
                prevLevelButton.SetActive(currentCreatureIndex > 0 && knownCreatures[currentCreatureIndex - 1].creatureNameID == knownCreatures[currentCreatureIndex].creatureNameID);
                nextLevelButton.SetActive(true);
                LoadCurrentCreature();
                levelText.text = knownCreatures[currentCreatureIndex].creatureLevel.ToString() + " lvl";
            }
        }
    }

    public void LoadCurrentCreature()
    {
        // show/hide buttons
        //nextPageButton.SetActive(currentCreatureIndex < knownCreatures.Count - 1);
        //prevPageButton.SetActive(currentCreatureIndex > 0);

        // loading translation keys
        CreatureInformation inf = knownCreatures[currentCreatureIndex];
        nameText.text = "creature_name_" + inf.creatureNameID;
        descriptionText.text = "creature_description_" + inf.creatureNameID;
        healthText.text = inf.creatureHealth.ToString();
        energyText.text = inf.creatureEnergy.ToString();
        attackDamageText.text = inf.creatureDamage.ToString();
        for (int i = 0; i < protectionTexts.Length; i++)
            protectionTexts[i].text = inf.creatureProtections[i].ToString();

        for (int i = 0; i < instantiatedTraits.Count; i++)
            Destroy(instantiatedTraits[i].gameObject);
        instantiatedTraits.Clear();

        for (int i = 0; i < inf.traitsCount; i++)
        {
            Text trait = Instantiate(traitExemplar);
            trait.transform.SetParent(traitsContainer.transform);
            trait.text = "creature_traits_" + inf.creatureNameID + "_" + i.ToString();
            instantiatedTraits.Add(trait);
        }

        // translating
        nameText.text = Translate.TranslateText(nameText.text);
        descriptionText.text = Translate.TranslateText(descriptionText.text);
        for (int i = 0; i < inf.traitsCount; i++)
            instantiatedTraits[i].text = Translate.TranslateText(instantiatedTraits[i].text);

        creatureImage.Play("bestiary_creature_" + inf.creatureNameID);
    }

    public void SaveBestiary()
    {
        if (Directory.Exists(Application.dataPath + "/Bestiary/") == false)
            Directory.CreateDirectory(Application.dataPath + "/Bestiary/");

        XmlNode userNode;
        XmlElement element;
        XmlDocument xmlDoc = new XmlDocument();
        XmlNode rootNode = xmlDoc.CreateElement("bestiary");
        xmlDoc.AppendChild(rootNode);

        userNode = xmlDoc.CreateElement("Bestiary");
        for (int i = 0; i < knownCreatures.Count; i++)
        {
            element = xmlDoc.CreateElement("creature");
            element.SetAttribute("value", knownCreatures[i].ToString());
            userNode.AppendChild(element);
        }

        rootNode.AppendChild(userNode);
        xmlDoc.Save(bestiaryFilePath);
    }

    public void LoadBestiary()
    {
        if (Directory.Exists(Application.dataPath + "/Bestiary/") == false)
            Directory.CreateDirectory(Application.dataPath + "/Bestiary/");

        if (File.Exists(bestiaryFilePath) == true)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(bestiaryFilePath);
            XmlElement xRoot = xDoc.DocumentElement;

            foreach (XmlNode xnode in xRoot)
            {
                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    if (childnode.Name == "creature")
                    {
                        string creatureData = childnode.Attributes.GetNamedItem("value").Value;
                        CreatureInformation inf = CreatureInformation.FromString(creatureData);
                        knownCreatures.Add(inf);
                    }
                }
            }
        }
    }
}