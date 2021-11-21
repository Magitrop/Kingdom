using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CellObjectSound
{
    public string clipName;
    public AudioClip clip;
}

public abstract class CellObject : MonoBehaviour
{
    public MapController map;
    public int x, y;
    public int cellObjectID = -1;

    public SpriteRenderer spriteRenderer;
    public GameObject visualObject;
    public SpriteRenderer healthbar;
    public SpriteRenderer healthbarEmpty;
    public TextMesh levelText;
    public AudioSource audioSource;
    public List<CellObjectSound> sounds;

    public bool isDestroyed;

    public virtual void Initialize()
    {
        spriteRenderer = visualObject.GetComponent<SpriteRenderer>();
        if (healthbar != null)
            healthbar.sortingOrder = 999;
        if (healthbarEmpty != null)
            healthbarEmpty.sortingOrder = 998;
    }

    public void PlaySound(string clipName)
    {
        List<CellObjectSound> objectSounds = sounds.Where(s => s.clipName == clipName).ToList();
        //if (objectSounds.Count == 0)
            //print(clipName + " not found");
        for (int i = 0; i < objectSounds.Count; i++)
        {
            audioSource.PlayOneShot(objectSounds[i].clip);
        }
    }

    public virtual void Update()
    {
        if (spriteRenderer != null)
            spriteRenderer.sortingOrder = map.mapSizeY - y;
    }

    private void OnDestroy()
    {
        isDestroyed = true;
    }
}