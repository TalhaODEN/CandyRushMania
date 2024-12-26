using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private SwipeManager swipeManager;

    [Header("Move Count")]
    [SerializeField] private Text moveCountText;

    [Header("Music and Volume Button Sprites")]
    [SerializeField] private Sprite activeMusicButtonSprite;
    [SerializeField] private Sprite activeSoundButtonSprite;
    [SerializeField] private Sprite passiveMusicButtonSprite;
    [SerializeField] private Sprite passiveSoundButtonSprite;

    [Header("Menu Button")]
    [SerializeField] private Button menuButton;

    [Header("Action Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;

    [Header("Voice Buttons")]
    [SerializeField] private Button musicButton;
    [SerializeField] private Button soundButton;

    [Header("Level Completed Panel Buttons")]
    [SerializeField] private Button homeButton;
    [SerializeField] private Button levelSelectButton;
    [SerializeField] private Button forwardButton;

    [Header("Paused Panel")]
    [SerializeField] private GameObject pausedPanel;

    private void Start()
    {
        swipeManager = FindObjectOfType<SwipeManager>();
    }

    public void SetMoveCount()
    {
        moveCountText.text = $"{swipeManager.MoveLimit}"; 
    }

    public void MenuButtonFunction()
    {
        pausedPanel.SetActive(true);
        menuButton.interactable = false;
        Time.timeScale = 0f;
    }
    public void ResumeButtonFunction()
    { 
        menuButton.interactable = true;
        Time.timeScale = 1f;
        pausedPanel.SetActive(false);
        
    }
    
    public void RestartButtonFunction()
    {
        //Bölümü yeniden başlat
    }

    public void ExitButtonFunction()
    {
        //Ana menüye döndür
    }

    public void MusicButtonFunction()
    {
        if(musicButton.GetComponent<Image>().sprite.name == activeMusicButtonSprite.name)
        {
            musicButton.GetComponent<Image>().sprite = passiveMusicButtonSprite;
        }
        else
        {
            musicButton.GetComponent<Image>().sprite = activeMusicButtonSprite;
        }
    }

    public void SoundButtonFunction()
    {
        if (soundButton.GetComponent<Image>().sprite.name == activeSoundButtonSprite.name)
        {
            soundButton.GetComponent<Image>().sprite = passiveSoundButtonSprite;
        }
        else
        {
            soundButton.GetComponent<Image>().sprite = activeSoundButtonSprite;
        }
    }
    



}
