using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UISaveGameButton : MonoBehaviour, IPointerDownHandler
{
    public MapController map;

    void Start()
    {
        map = FindObjectOfType<MapController>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        map.player.bestiary.SaveBestiary();
        map.SaveGame();
    }
}
