using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInventoryScrollButton : UIInventoryActionButton, IPointerDownHandler
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        player.RefreshStatsVisual();
        if (isUnavailable == true)
            return;
        player.scroll.Reverse();
    }
}