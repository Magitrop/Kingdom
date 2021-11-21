using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInventoryBestiaryButton : UIInventoryActionButton, IPointerDownHandler
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        player.DisableSpellMode();
        if (isUnavailable == true)
            return;
        if (player.bestiary.knownCreatures.Count > 0)
        {
            if (player.bestiary.gameObject.activeSelf == false)
            {
                player.bestiary.gameObject.SetActive(true);
                player.bestiary.currentCreatureIndex = 0;
                player.bestiary.OpenBestiary();
            }
            else player.bestiary.gameObject.SetActive(false);
        }

        player.RefreshStatsVisual();
    }
}