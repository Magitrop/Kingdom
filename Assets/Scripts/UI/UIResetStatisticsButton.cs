using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIResetStatisticsButton : MonoBehaviour, IPointerDownHandler
{
    public PlayerStatistics statistics;
    public DeathMenu deathMenu;

    public void OnPointerDown(PointerEventData eventData)
    {
        statistics.ClearStatistics();
        statistics.SaveStatistics();
        deathMenu.ShowStatistics();
    }
}