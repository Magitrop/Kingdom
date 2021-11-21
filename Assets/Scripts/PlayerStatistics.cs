using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class PlayerStatistics : MonoBehaviour
{
    public int killedEnemiesThisTime, killedEnemiesTotal;
    public int maxLevelThisTime, maxLevelEver;
    public int damageDealtThisTime, damageDealtTotal;
    public int damageReceivedThisTime, damageReceivedTotal;
    public int healingReceivedThisTime, healingReceivedTotal;
    public int deathsCount;

    public void ClearStatistics()
    {
        PlayerPrefs.SetInt("maxLevelEver", 0);
        PlayerPrefs.SetInt("killedEnemiesTotal", 0);
        PlayerPrefs.SetInt("damageDealtTotal", 0);
        PlayerPrefs.SetInt("damageReceivedTotal", 0);
        PlayerPrefs.SetInt("healingReceivedTotal", 0);
        PlayerPrefs.SetInt("deathsCount", 0);
        PlayerPrefs.SetInt("killedEnemiesThisTime", 0);
        PlayerPrefs.SetInt("maxLevelThisTime", 0);
        PlayerPrefs.SetInt("damageDealtThisTime", 0);
        PlayerPrefs.SetInt("damageReceivedThisTime", 0);
        PlayerPrefs.SetInt("healingReceivedThisTime", 0);
        killedEnemiesThisTime = killedEnemiesTotal = 
            maxLevelThisTime = maxLevelEver = 
            damageDealtThisTime = damageDealtTotal = 
            damageReceivedThisTime = damageReceivedTotal = 
            healingReceivedThisTime = healingReceivedTotal =
            deathsCount = 0;
    }

    public void SaveStatistics()
    {
        PlayerPrefs.SetInt("killedEnemiesTotal", killedEnemiesTotal);
        PlayerPrefs.SetInt("maxLevelEver", maxLevelEver);
        PlayerPrefs.SetInt("damageDealtTotal", damageDealtTotal);
        PlayerPrefs.SetInt("damageReceivedTotal", damageReceivedTotal);
        PlayerPrefs.SetInt("healingReceivedTotal", healingReceivedTotal);
        PlayerPrefs.SetInt("deathsCount", deathsCount);

        PlayerPrefs.SetInt("killedEnemiesThisTime", killedEnemiesThisTime);
        PlayerPrefs.SetInt("maxLevelThisTime", maxLevelThisTime);
        PlayerPrefs.SetInt("damageDealtThisTime", damageDealtThisTime);
        PlayerPrefs.SetInt("damageReceivedThisTime", damageReceivedThisTime);
        PlayerPrefs.SetInt("healingReceivedThisTime", healingReceivedThisTime);

        /*
        if (Directory.Exists(Application.dataPath + "/Saves/") == false)
            Directory.CreateDirectory(Application.dataPath + "/Saves/");

        XmlNode userNode;
        XmlElement element;
        XmlDocument xmlDoc = new XmlDocument();
        XmlNode rootNode = xmlDoc.CreateElement("statistics");
        xmlDoc.AppendChild(rootNode);

        userNode = xmlDoc.CreateElement("Statistics");

        element = xmlDoc.CreateElement("statistics");
        //element = xmlDoc.CreateElement("killedEnemies");
        element.SetAttribute("killedEnemies", killedEnemiesThisTime.ToString());

        //element = xmlDoc.CreateElement("maxLevel");
        element.SetAttribute("maxLevel", maxLevelThisTime.ToString());

        //element = xmlDoc.CreateElement("damageDealt");
        element.SetAttribute("damageDealt", damageDealtThisTime.ToString());

        //element = xmlDoc.CreateElement("damageReceived");
        element.SetAttribute("damageReceived", damageReceivedThisTime.ToString());

        //element = xmlDoc.CreateElement("healingReceived");
        element.SetAttribute("healingReceived", healingReceivedThisTime.ToString());

        userNode.AppendChild(element);

        rootNode.AppendChild(userNode);
        xmlDoc.Save(Application.dataPath + "/Saves/statistics.xml");
        */
    }

    public void LoadStatistics()
    {
        killedEnemiesTotal = PlayerPrefs.GetInt("killedEnemiesTotal");
        maxLevelEver = PlayerPrefs.GetInt("maxLevelEver");
        damageDealtTotal = PlayerPrefs.GetInt("damageDealtTotal");
        damageReceivedTotal = PlayerPrefs.GetInt("damageReceivedTotal");
        healingReceivedTotal = PlayerPrefs.GetInt("healingReceivedTotal");
        deathsCount = PlayerPrefs.GetInt("deathsCount");
        killedEnemiesThisTime = maxLevelThisTime = damageDealtThisTime = damageReceivedThisTime = healingReceivedThisTime = 0;

        /*
        killedEnemiesThisTime = PlayerPrefs.GetInt("killedEnemiesThisTime");
        maxLevelThisTime = PlayerPrefs.GetInt("maxLevelThisTime");
        damageDealtThisTime = PlayerPrefs.GetInt("damageDealtThisTime");
        damageReceivedThisTime = PlayerPrefs.GetInt("damageReceivedThisTime");
        healingReceivedThisTime = PlayerPrefs.GetInt("healingReceivedThisTime");
        */

        /*
        if (Directory.Exists(Application.dataPath + "/Saves/") == false)
            Directory.CreateDirectory(Application.dataPath + "/Saves/");

        if (File.Exists(Application.dataPath + "/Saves/statistics.xml") == true)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Application.dataPath + "/Saves/statistics.xml");
            XmlElement xRoot = xDoc.DocumentElement;

            foreach (XmlNode xnode in xRoot)
            {
                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    for (int i = 0; i < childnode.Attributes.Count; i++)
                    {
                        if (childnode.Attributes[i].Name.StartsWith("killedEnemies") == true)
                            killedEnemiesThisTime = int.Parse(childnode.Attributes[i].Value);
                        if (childnode.Attributes[i].Name.StartsWith("maxLevel") == true)
                            maxLevelThisTime = int.Parse(childnode.Attributes[i].Value);
                        if (childnode.Attributes[i].Name.StartsWith("damageDealt") == true)
                            damageDealtThisTime = int.Parse(childnode.Attributes[i].Value);
                        if (childnode.Attributes[i].Name.StartsWith("damageReceived") == true)
                            damageReceivedThisTime = int.Parse(childnode.Attributes[i].Value);
                        if (childnode.Attributes[i].Name.StartsWith("healingReceived") == true)
                            healingReceivedThisTime = int.Parse(childnode.Attributes[i].Value);
                    }
                }
            }
        }
        */
    }
}