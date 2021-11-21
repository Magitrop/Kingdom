using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIStartGameButton : MonoBehaviour, IPointerDownHandler
{
    public Text buttonText;

    string levelName;
    string playerName;
    int heroLevel;

    public void Start()
    {
        if (Directory.Exists(Application.dataPath + "/Saves/") == false)
            Directory.CreateDirectory(Application.dataPath + "/Saves/");

        if (File.Exists(Application.dataPath + "/Saves/save.xml"))
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Application.dataPath + "/Saves/save.xml");
            XmlElement xRoot = xDoc.DocumentElement;

            foreach (XmlNode xnode in xRoot)
            {
                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    if (childnode.Name == "hero")
                    {
                        string[] s = childnode.Attributes.GetNamedItem("value").Value.Split(';');
                        levelName = s[0];
                        playerName = s[1];
                        heroLevel = int.Parse(s[7]);
                    }
                }
            }

            //buttonText.text = playerName + " (lvl " + heroLevel.ToString() + ")";
            buttonText.text = Translate.TranslateText("buttons_startgame") +  ": Level " + heroLevel.ToString();
        }
        else
        {
            GetComponent<Button>().interactable = false;
            //buttonText.text = "- | -";
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GetComponent<Button>().interactable == true)
            LoadingScreen.Instance.Show(SceneManager.LoadSceneAsync(levelName));
        //SceneManager.LoadScene(levelName);
    }
}

public class LevelsManager
{
    public static readonly string[] levelsNames =
        new string[3] {
            "dungeon_tutorial_level",
            "dungeon_level_0",
            "dungeon_level_1"
        };
}