using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    

    private AudioSource audioSource;
    public GlobalVariables GV;
    public AudioClip song;

    void Start()
    {
        audioSource = GetComponentInChildren<AudioSource>();
        GV = GetComponent<GlobalVariables>();

        
        audioSource.volume = GV.getAudioVolume() / 10f;
        audioSource.clip = song;
        audioSource.loop = true;
        audioSource.Play();

    }
    void Update()
    {
        audioSource.volume = GV.getAudioVolume() / 10f;
    }

}
