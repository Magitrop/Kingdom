using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializationManager : MonoBehaviour
{
    public MapController mapController;

    public void Awake()
    {
        ItemsDatabase.InitializeDatabase();
        RecipesDatabase.InitializeDatabase();
        EffectsDatabase.InitializeDatabase();
        SpellsDatabase.InitializeDatabase();
        SkillsDatabase.InitializeDatabase();
        mapController.player = FindObjectOfType<PlayerController>();
        mapController.player.InitializePlayer();
        for (int i = 0; i < mapController.creaturesToMove.Count; i++)
        {
            mapController.creaturesToMove[i].Initialize();
            mapController.creaturesToMove[i].connectedInventory.Initialize();
        }
    }
}