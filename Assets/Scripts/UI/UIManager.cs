using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class CandyNeedUI
{
    public Image needImage;  
    public Text needCount;   
}
public class UIManager : MonoBehaviour
{
    private SwipeManager swipeManager;
    private VoiceData voiceData;
    private LevelData levelData;
    private MatchFinder matchFinder;

    [Header("Move Count")]
    [SerializeField] private Text moveCountText;

    [Header("Need Counts")]
    [SerializeField] private Text[] needsTextCounts;

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

    [Header("Needs UI")]
    [SerializeField] private CandyNeedUI[] needUIs;

    private void Start()
    {
        swipeManager = FindObjectOfType<SwipeManager>();
        matchFinder = FindObjectOfType<MatchFinder>();

        voiceData = Resources.Load<VoiceData>($"ScriptableObjects/VoiceData");
        SetStartVolumeSettings();

        GameData gameData = Resources.Load<GameData>($"ScriptableObjects/GameData");
        GameObject[] prefabCandies = gameData.PrefabCandies;
        string currentScene = SceneManager.GetActiveScene().name;
        levelData = Resources.Load<LevelData>($"ScriptableObjects/{currentScene}Data");
        CandyNeed[] needs = levelData.CandyNeeds;
        if (needs[0].neededAmount == 0)
        {
            Debug.Log("needs[0] amount 0!!!");
            return;
        }
        for (int i = 0; i < needs.Length; i++)
        {
            SetNeedsImageAndCount(needs[i], prefabCandies, needUIs[i]);
        }
        int needsLength = needs.Length;
        foreach(CandyNeedUI candyNeedUI in needUIs)
        {
            if(candyNeedUI.needImage.sprite == null)
            {
                candyNeedUI.needImage.gameObject.SetActive(false);
            }
        }
    }
    private void SetNeedsImageAndCount
        (CandyNeed candyNeed, GameObject[] prefabCandies, CandyNeedUI needUI)
    {
        foreach (GameObject prefab in prefabCandies)
        {
            if (candyNeed.candyTag == prefab.tag)
            {
                needUI.needImage.sprite = prefab.GetComponent<SpriteRenderer>().sprite;
                needUI.needCount.text = $"{candyNeed.neededAmount}";
            }
        }
    }

    private void SetStartVolumeSettings()
    {
        activeMusicButtonSprite = voiceData.ActiveMusicButtonSprite;
        passiveMusicButtonSprite = voiceData.PassiveMusicButtonSprite;
        activeSoundButtonSprite = voiceData.ActiveSoundButtonSprite;
        passiveSoundButtonSprite = voiceData.PassiveSoundButtonSprite;

        if (voiceData.isMusicActive)
        {
            musicButton.GetComponent<Image>().sprite = activeMusicButtonSprite;
        }
        else
        {
            musicButton.GetComponent<Image>().sprite = passiveMusicButtonSprite;
        }

        if (voiceData.isSoundActive)
        {
            soundButton.GetComponent<Image>().sprite = activeSoundButtonSprite;
        }
        else
        {
            soundButton.GetComponent<Image>().sprite = passiveSoundButtonSprite;
        }
    }

    public void SetMoveCount(int moveLimit)
    {
        moveCountText.text = $"{moveLimit}"; 
    }

    public void SetNeedsCount()
    {
        int index = 0;
        foreach(Text needCount in needsTextCounts)
        {
            if(!needCount.isActiveAndEnabled)
            {
                break;
            }
            needCount.text = $"{matchFinder.MatchedNeedCandies[index].neededAmount}";
            index++;
        }
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
        Time.timeScale = 1f;
        matchFinder.ResetMatchedNeedCandies();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitButtonFunction()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void MusicButtonFunction()
    {
        if(voiceData.isMusicActive)
        {
            musicButton.GetComponent<Image>().sprite = passiveMusicButtonSprite;
            voiceData.isMusicActive = false;
        }
        else
        {
            musicButton.GetComponent<Image>().sprite = activeMusicButtonSprite;
            voiceData.isMusicActive = true;
        }
    }

    public void SoundButtonFunction()
    {
        if (voiceData.isSoundActive)
        {
            soundButton.GetComponent<Image>().sprite = passiveSoundButtonSprite;
            voiceData.isSoundActive = false;
        }
        else
        {
            soundButton.GetComponent<Image>().sprite = activeSoundButtonSprite;
            voiceData.isSoundActive = true;
        }
    }
    



}
