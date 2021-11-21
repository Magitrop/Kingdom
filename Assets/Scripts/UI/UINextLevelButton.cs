using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UINextLevelButton : MonoBehaviour, IPointerDownHandler
{
    public MapController map;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (map == null)
            map = FindObjectOfType<MapController>();

        if (GetComponent<Button>().interactable == true)
            if (map.currentLevelIndex + 1 < LevelsManager.levelsNames.Length)
            {
                if (Directory.Exists(Application.dataPath + "/Saves/") == false)
                    Directory.CreateDirectory(Application.dataPath + "/Saves/");
                map.player.SetStateValue("cur_health", map.player.GetStateValue("max_health"));
                map.player.bestiary.SaveBestiary();
                if (File.Exists(Application.dataPath + "/Saves/fog.xml") == true)
                    File.Delete(Application.dataPath + "/Saves/fog.xml");
                map.clearedCellsFogOfWar.Clear();
                map.SaveGame();
                LoadingScreen.Instance.Show(SceneManager.LoadSceneAsync(LevelsManager.levelsNames[map.currentLevelIndex + 1]));
                //SceneManager.LoadScene(LevelsManager.levelsNames[map.currentLevelIndex + 1]);
            }
    }
}