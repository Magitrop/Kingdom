using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip musicToPlay;
    public float musicTime;

    public void Start()
    {
        audioSource.clip = musicToPlay;
        audioSource.time = musicTime;
        audioSource.Play();
    }
}