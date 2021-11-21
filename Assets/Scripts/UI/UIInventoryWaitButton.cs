using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInventoryWaitButton : UIInventoryActionButton, IPointerDownHandler
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        player.DisableSpellMode();
        if (isUnavailable == true)
            return;
        /*
        if (player.isAlive == false || player.inSpellMode == true || player.inExploreMode == true)
            return;
            */
        if (player.turnIsCompleted == false && 
            player.isMoving == false && 
            player.bestiary.gameObject.activeSelf == false && 
            player.menu.activeSelf == false && 
            player.visualInventory.activeSelf == false)
        {
            player.Wait();
        }

        player.RefreshStatsVisual();
    }
}
