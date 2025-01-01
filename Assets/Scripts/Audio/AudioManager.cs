using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager audioManager;

    [Header("Musics")]
    [SerializeField] private AudioClip menuMusicClip;
    [SerializeField] private AudioClip gameplayMusicClip;

    [Header("SFX")]
    [SerializeField] private AudioClip levelFailedSfxClip;
    [SerializeField] private AudioClip levelCompletedSfxClip;
    [SerializeField] private AudioClip poppingCandySfxClip;
    [SerializeField] private AudioClip clickSfxClip;

    private AudioSource musicSource; 
    private AudioSource sfxSource;

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
        AudioSource[] audioSources = GetComponents<AudioSource>();
        musicSource = audioSources[0]; 
        sfxSource = audioSources[1];
    }

    private void Start()
    {
        PlayMenuMusic();
        AddClickSoundToButtons();
    }

    private void OnEnable()
    {
        // Sahne yüklendikten sonra tetiklenecek olayý dinle
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Olaydan çýk, gereksiz yere dinlenmesini engelle
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Sahne yüklendikten sonra yapýlacak iþlemler
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu" || scene.name == "SelectLevel")
        {
            if (!musicSource.isPlaying || musicSource.clip != menuMusicClip)
            {
                PlayMenuMusic();
            }
        }
        else
        {
            if (!musicSource.isPlaying || musicSource.clip != gameplayMusicClip)
            {
                PlayGameplayMusic();
            }
        }
        AddClickSoundToButtons();
    }
    private void AddClickSoundToButtons()
    {
        Button[] buttons = FindObjectsOfType<Button>(true); // true, deaktif objeleri de dahil eder
        foreach (Button button in buttons)
        {
            if (!ButtonHasClickListener(button)) // Eðer butonun üzerinde PlayClickSfx dinleyicisi yoksa
            {
                button.onClick.AddListener(PlayClickSfx);         
            }
        }
    }

    // PlayClickSfx dinleyicisinin butona eklenip eklenmediðini kontrol eden fonksiyon
    private bool ButtonHasClickListener(Button button)
    {
        for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
        {
            if (button.onClick.GetPersistentMethodName(i) == "PlayClickSfx")
            {
                return true; // PlayClickSfx zaten eklenmiþ
            }
        }
        return false; // PlayClickSfx eklenmemiþ
    }




    public void PlayMenuMusic()
    {
        musicSource.clip = menuMusicClip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayGameplayMusic()
    {
        musicSource.clip = gameplayMusicClip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayLevelFailedSfx()
    {
        sfxSource.PlayOneShot(levelFailedSfxClip);
    }

    public void PlayLevelCompletedSfx()
    {
        sfxSource.PlayOneShot(levelCompletedSfxClip);
    }

    public void PlayPoppingCandySfx()
    {
        sfxSource.PlayOneShot(poppingCandySfxClip);
    }

    public void PlayClickSfx()
    {
        sfxSource.PlayOneShot(clickSfxClip);
    }

    public void ToggleMusicMute()
    {
        if(musicSource.volume == 0f)
        {
            musicSource.volume = 1f;
        }
        else
        {
            musicSource.volume = 0f;
        }
    }
    public void ToggleEffectMute()
    {
        if (sfxSource.volume == 0f)
        {
            sfxSource.volume = 1f;
        }
        else
        {
            sfxSource.volume = 0f;
        }
    }
}
