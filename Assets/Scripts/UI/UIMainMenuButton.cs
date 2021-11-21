using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenuButton : MonoBehaviour, IPointerDownHandler
{
    public Text buttonText;
    public bool afterDeath;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GetComponent<Button>().interactable == true)
        {
            if (afterDeath == true)
            {
                if (Directory.Exists(Application.dataPath + "/Saves/") == false)
                    Directory.CreateDirectory(Application.dataPath + "/Saves/");
                if (File.Exists(Application.dataPath + "/Saves/save.xml") == true)
                    File.Delete(Application.dataPath + "/Saves/save.xml");
                if (File.Exists(Application.dataPath + "/Bestiary/bestiary.xml") == true)
                    File.Delete(Application.dataPath + "/Bestiary/bestiary.xml");
                if (File.Exists(Application.dataPath + "/Save/statistics.xml") == true)
                    File.Delete(Application.dataPath + "/Save/statistics.xml");
            }
            //SceneManager.LoadScene("mainmenu");
            LoadingScreen.Instance.Show(SceneManager.LoadSceneAsync("mainmenu"));
        }
    }
}