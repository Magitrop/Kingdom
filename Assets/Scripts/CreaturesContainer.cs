using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreaturesContainer : MonoBehaviour
{
    public List<Creature> creatures;

    /// <summary>
    /// Returns creature from list. NOT A COPY!
    /// </summary>
    /// <param name="_name"></param>
    /// <returns></returns>
    public Creature GetCreatureByName(string _name)
    {
        for (int i = 0; i < creatures.Count; i++)
            if (creatures[i].creatureNameID == _name)
                return creatures[i];
        return null;
    }
}