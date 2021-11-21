using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapController))]
public class InEditorCreaturesHandler : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        if (GUILayout.Button("Interlink Creatures' positions"))
        {
            List<Creature> creatures = FindObjectsOfType<Creature>().ToList();
            for (int i = 0; i < creatures.Count; i++)
            {
                creatures[i].x = Mathf.RoundToInt(creatures[i].transform.position.x);
                creatures[i].y = Mathf.RoundToInt(creatures[i].transform.position.y);
            }
        }
        if (GUILayout.Button("Embed Creatures"))
        {
            MapController mapController = FindObjectOfType<MapController>();
            List<Creature> creatures = FindObjectsOfType<Creature>().ToList();
            creatures.Remove(creatures.Find(c => c.GetComponent<PlayerController>()));
            mapController.creaturesToMove = creatures;
        }
        serializedObject.ApplyModifiedProperties();
    }
}