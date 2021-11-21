using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonHeroLook : UIInventoryActionButton, IPointerDownHandler
{
    public FacingDirection direction;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (isUnavailable == true)
            return;
        /*
        if (player.isAlive == false || player.inSpellMode == true || player.inExploreMode == true)
            return;
            */
        player.facingDirection = direction;
        player.PlayIdleAnimation();
        player.RefreshStatsVisual();
    }
}