using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInventoryStepButton : UIInventoryActionButton, IPointerDownHandler
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
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
            if (player.GetStateValue("cur_energy") > 0)
                player.Move();
            else player.map.ShowNotification("notifications_not_enough_energy");
        }

        player.RefreshStatsVisual();
    }
}

