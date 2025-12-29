using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using World;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip presentSong;
    [SerializeField] private AudioClip pastSong;
    [SerializeField] private AudioClip futureSong;

    float currentTime = 0;

    private void Start()
    {
        audioSource.Play();
    }

    public void ChangeAudio(EWorldState newWorldState)
    {
        currentTime = audioSource.time;

        switch (newWorldState)
        {
            case EWorldState.PAST:
            {
                    audioSource.clip = pastSong;
                    break;
            }
            case EWorldState.PRESENT:
                {
                    audioSource.clip = presentSong;
                    break;
                }
            case EWorldState.FUTURE:
                {
                    audioSource.clip = futureSong;
                    break;
                }
        }

        audioSource.time = currentTime;
        audioSource.Play();
    }
}
