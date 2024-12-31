using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager audioManager;

    [Header("Musics")]
    [SerializeField] private AudioSource menuMusic;
    [SerializeField] private AudioSource gameplayMusic;

    [Header("SFX")]
    [SerializeField] private AudioSource levelFailedSfx;
    [SerializeField] private AudioSource levelCompletedSfx;
    [SerializeField] private AudioSource poppingCandySfx;
    [SerializeField] private AudioSource clickSfx;

    private void Awake()
    {
        if(audioManager != null && audioManager != this)
        {
            Destroy(gameObject);
        }
        else
        {
            audioManager = this;
            DontDestroyOnLoad(gameObject);
        }
    }



}
