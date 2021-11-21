using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIBestiaryPageButton : MonoBehaviour, IPointerDownHandler
{
    public bool isNextPageButton;
    public Bestiary bestiary;
    public bool isInteractable;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isInteractable == true)
        {
            if (isNextPageButton == true)
            {
                int prevCreatureIndex = bestiary.currentCreatureIndex;
                while (bestiary.knownCreatures[bestiary.currentCreatureIndex].creatureNameID == bestiary.knownCreatures[prevCreatureIndex].creatureNameID)
                    if (bestiary.currentCreatureIndex < bestiary.knownCreatures.Count - 1)
                        bestiary.currentCreatureIndex++;
                    else
                    {
                        bestiary.currentCreatureIndex = prevCreatureIndex;
                        break;
                    }
                bestiary.nextPageButton.SetActive(bestiary.currentCreatureIndex < bestiary.knownCreatures.Count - 1 && bestiary.currentCreatureIndex != prevCreatureIndex);
                bestiary.prevPageButton.SetActive(bestiary.currentCreatureIndex > 0);
                bestiary.nextLevelButton.SetActive(bestiary.currentCreatureIndex < bestiary.knownCreatures.Count - 1 && bestiary.knownCreatures[bestiary.currentCreatureIndex + 1].creatureNameID == bestiary.knownCreatures[bestiary.currentCreatureIndex].creatureNameID);
                bestiary.prevLevelButton.SetActive(false);
                bestiary.LoadCurrentCreature();
                bestiary.levelText.text = bestiary.knownCreatures[bestiary.currentCreatureIndex].creatureLevel.ToString() + " lvl";
            }
            else
            {
                int prevCreatureIndex = bestiary.currentCreatureIndex;
                while (bestiary.knownCreatures[bestiary.currentCreatureIndex].creatureNameID == bestiary.knownCreatures[prevCreatureIndex].creatureNameID && bestiary.currentCreatureIndex > 0)
                    bestiary.currentCreatureIndex--;
                prevCreatureIndex = bestiary.currentCreatureIndex;
                while (bestiary.knownCreatures[bestiary.currentCreatureIndex].creatureNameID == bestiary.knownCreatures[prevCreatureIndex].creatureNameID && bestiary.currentCreatureIndex > 0)
                    bestiary.currentCreatureIndex--;
                if (bestiary.currentCreatureIndex != 0)
                    bestiary.currentCreatureIndex++;
                bestiary.nextPageButton.SetActive(bestiary.currentCreatureIndex < bestiary.knownCreatures.Count - 1);
                bestiary.prevPageButton.SetActive(bestiary.currentCreatureIndex > 0);
                bestiary.nextLevelButton.SetActive(bestiary.currentCreatureIndex < bestiary.knownCreatures.Count - 1 && bestiary.knownCreatures[bestiary.currentCreatureIndex + 1].creatureNameID == bestiary.knownCreatures[bestiary.currentCreatureIndex].creatureNameID);
                bestiary.prevLevelButton.SetActive(false);
                bestiary.LoadCurrentCreature();
                bestiary.levelText.text = bestiary.knownCreatures[bestiary.currentCreatureIndex].creatureLevel.ToString() + " lvl";
            }
        }
    }
}