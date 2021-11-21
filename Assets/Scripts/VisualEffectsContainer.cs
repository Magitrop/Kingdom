using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualEffectsContainer : MonoBehaviour
{
    public List<VisualEffect> visualEffects = new List<VisualEffect>();
    
    public VisualEffect GetVisualEffect(string name)
    {
        for (int i = 0; i < visualEffects.Count; i++)
            if (visualEffects[i].effectName == name)
                return visualEffects[i];
        return null;
    }
}