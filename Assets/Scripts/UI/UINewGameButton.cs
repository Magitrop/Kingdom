using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UINewGameButton : MonoBehaviour, IPointerDownHandler
{
    public string levelName;
    public Text buttonText;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GetComponent<Button>().interactable == true)
        {
            if (Directory.Exists(Application.dataPath + "/Saves/") == false)
                Directory.CreateDirectory(Application.dataPath + "/Saves/");
            if (File.Exists(Application.dataPath + "/Saves/save.xml") == true)
                File.Delete(Application.dataPath + "/Saves/save.xml");
            if (File.Exists(Application.dataPath + "/Bestiary/bestiary.xml") == true)
                File.Delete(Application.dataPath + "/Bestiary/bestiary.xml");
            if (File.Exists(Application.dataPath + "/Saves/statistics.xml") == true)
                File.Delete(Application.dataPath + "/Saves/statistics.xml");
            if (File.Exists(Application.dataPath + "/Saves/fog.xml") == true)
                File.Delete(Application.dataPath + "/Saves/fog.xml");
            if (File.Exists(Application.dataPath + "/Saves/recipes.xml") == true)
                File.Delete(Application.dataPath + "/Saves/recipes.xml");
            if (File.Exists(Application.dataPath + "/Saves/skills.xml") == true)
                File.Delete(Application.dataPath + "/Saves/skills.xml");
            if (File.Exists(Application.dataPath + "/Saves/prepared_skills.xml") == true)
                File.Delete(Application.dataPath + "/Saves/prepared_skills.xml");

            //при загрузке игры раскрываетя туман войны гораздо дальше, чем игрок на самом деле видит
            //SceneManager.LoadScene(levelName);
            LoadingScreen.Instance.Show(SceneManager.LoadSceneAsync(levelName));
        }
    }
}