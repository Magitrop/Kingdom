using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventoryActionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public UITooltip tooltip;
    public string tooltipText;
    public PlayerController player;

    protected bool isUnavailable;

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (player.isAlive == false || player.inSpellMode == true || player.inExploreMode == true
            || player.menu.activeSelf == true || player.notificationPanel.gameObject.activeSelf == true 
            || player.levelupPanelController.gameObject.activeSelf == true)
            isUnavailable = true;
        else isUnavailable = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip != null)
        {
            tooltip.gameObject.SetActive(true);
            tooltip.SetTooltipText(Translate.TranslateText(tooltipText));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null)
            tooltip.gameObject.SetActive(false);
    }
}