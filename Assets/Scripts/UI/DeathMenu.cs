using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathMenu : MonoBehaviour
{
    public PlayerStatistics statistics;
    public Text killedEnemiesThisTimeText, killedEnemiesTotalText;
    public Text maxLevelThisTimeText, maxLevelEverText;
    public Text damageDealtThisTimeText, damageDealtTotalText;
    public Text damageReceivedThisTimeText, damageReceivedTotalText;
    public Text healingReceivedThisTimeText, healingReceivedTotalText;
    public Text deathsCountText;

    public void ShowStatistics()
    {
        killedEnemiesThisTimeText.text = Translate.TranslateText("labels_killedEnemiesThisTime") + statistics.killedEnemiesThisTime.ToString();
        killedEnemiesTotalText.text = Translate.TranslateText("labels_killedEnemiesTotal") + statistics.killedEnemiesTotal.ToString();
        maxLevelThisTimeText.text = Translate.TranslateText("labels_maxLevelThisTime") + statistics.maxLevelThisTime.ToString();
        maxLevelEverText.text = Translate.TranslateText("labels_maxLevelEver") + statistics.maxLevelEver.ToString();
        damageDealtThisTimeText.text = Translate.TranslateText("labels_damageDealtThisTime") + statistics.damageDealtThisTime.ToString();
        damageDealtTotalText.text = Translate.TranslateText("labels_damageDealtTotal") + statistics.damageDealtTotal.ToString();
        damageReceivedThisTimeText.text = Translate.TranslateText("labels_damageReceivedThisTime") + statistics.damageReceivedThisTime.ToString();
        damageReceivedTotalText.text = Translate.TranslateText("labels_damageReceivedTotal") + statistics.damageReceivedTotal.ToString();
        healingReceivedThisTimeText.text = Translate.TranslateText("labels_healingReceivedThisTime") + statistics.healingReceivedThisTime.ToString();
        healingReceivedTotalText.text = Translate.TranslateText("labels_healingReceivedTotal") + statistics.healingReceivedTotal.ToString();
        deathsCountText.text = Translate.TranslateText("labels_deathsCount") + statistics.deathsCount.ToString();
    }
}