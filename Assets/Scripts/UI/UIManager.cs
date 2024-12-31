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
    public int CurrentScore { get { return currentScore;}}
    public Image Star1 { get { return star1; } }
    public Image BigStar2 { get { return bigstar2; } }
    public Image Star3 { get { return star3; } }

    public Text ScoreText { get { return scoreText; } }
    public GameObject LevelFailedPanel { get { return levelFailedPanel;} }

    private SwipeManager swipeManager;
    private VoiceData voiceData;
    private LevelData levelData;
    private MatchFinder matchFinder;
    private LevelEndManager levelEndManager;

    private int currentScore = 0;
    private int[] starScoreLimits;

    [Header("Move Count")]
    [SerializeField] private Text moveCountText;

    [Header("Score")]
    [SerializeField] private Text scoreText;

    [Header("Stars")]
    [SerializeField] private Image star1;
    [SerializeField] private Image bigstar2;
    [SerializeField] private Image star3;

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

    [Header("Level Completed Stars")]
    [SerializeField] private Image panelStar1;
    [SerializeField] private Image panelBigstar2;
    [SerializeField] private Image panelStar3;

    [Header("Level Completed Score and Star Count")]
    [SerializeField] private Text completedScoreText;
    [SerializeField] private Text starCount;

    [Header("Level Failed Panel Buttons")]
    [SerializeField] private Button exitSmallButton;
    [SerializeField] private Button restartSmallButton;

    [Header("Paused Panel")]
    [SerializeField] private GameObject pausedPanel;

    [Header("Level Failed Panel")]
    [SerializeField] private GameObject levelFailedPanel;

    [Header("Level Completed Panel")]
    [SerializeField] private GameObject levelCompletedPanel;

    [Header("Needs UI")]
    [SerializeField] private CandyNeedUI[] needUIs;

    private void Start()
    {
        levelEndManager = FindObjectOfType<LevelEndManager>();
        swipeManager = FindObjectOfType<SwipeManager>();
        matchFinder = FindObjectOfType<MatchFinder>();

        voiceData = Resources.Load<VoiceData>($"ScriptableObjects/VoiceData");
        SetStartVolumeSettings();

        levelFailedPanel.SetActive(false);
        levelCompletedPanel.SetActive(false);

        GameData gameData = Resources.Load<GameData>($"ScriptableObjects/GameData");
        GameObject[] prefabCandies = gameData.PrefabCandies;
        string currentScene = SceneManager.GetActiveScene().name;
        levelData = Resources.Load<LevelData>($"ScriptableObjects/{currentScene}Data");

        starScoreLimits = new int[3];
        for(int i = 0; i < 3; i++)
        {
            starScoreLimits[i] = levelData.StarScoreLimits[i];
        }

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
        SceneManager.LoadScene("SelectLevel");
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

    private IEnumerator AnimateScore(int startScore, int targetScore)
    {
        float duration = 1f; 
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            currentScore = (int)Mathf.Lerp(startScore, targetScore, timeElapsed / duration);  
            scoreText.text = $"{currentScore}";  

            timeElapsed += Time.deltaTime;  
            yield return null;
        }

        currentScore = targetScore;  
        scoreText.text = $"{currentScore}";
    }

    public void UpdateScore(int targetScore)
    {
        StartCoroutine(AnimateScore(currentScore, targetScore));
        UpdateStars(targetScore);
    }

    public void UpdateStars(int targetScore)
    {
        float star1Fill = Mathf.Clamp01((float)targetScore / starScoreLimits[0]);
        float star2Fill = 0f;
        float star3Fill = 0f;

        if (targetScore > starScoreLimits[0])
        {
            star2Fill = Mathf.Clamp01((float)(targetScore - starScoreLimits[0]) /
                                      (starScoreLimits[1] - starScoreLimits[0]));
        }

        if (targetScore > starScoreLimits[1])
        {
            star3Fill = Mathf.Clamp01((float)(targetScore - starScoreLimits[1]) /
                                      (starScoreLimits[2] - starScoreLimits[1]));
        }

        // Sýralý þekilde animasyonlarý çalýþtýr
        StartCoroutine(AnimateStarsSequentially(star1, star1.fillAmount, star1Fill,
                                                bigstar2, star2Fill,
                                                star3, star3Fill));

        // Yýldýz sayýsýný belirle
        if (star3Fill >= 1f)
        {
            levelEndManager.stars = 3;
        }
        else if (star2Fill >= 1f)
        {
            levelEndManager.stars = 2;
        }
        else if (star1Fill >= 1f)
        {
            levelEndManager.stars = 1;
        }
        else
        {
            levelEndManager.stars = 0;
        }
    }

    private IEnumerator AnimateStarsSequentially(Image star1, float star1Current, float star1Target,
                                                 Image star2, float star2Target,
                                                 Image star3, float star3Target)
    {
        // 1. yýldýzýn animasyonu
        yield return AnimateStarFill(star1, star1Current, star1Target);

        // 2. yýldýzýn animasyonu (eðer hedef > 0 ise)
        if (star2Target > 0f)
        {
            yield return AnimateStarFill(star2, star2.fillAmount, star2Target);
        }

        // 3. yýldýzýn animasyonu (eðer hedef > 0 ise)
        if (star3Target > 0f)
        {
            yield return AnimateStarFill(star3, star3.fillAmount, star3Target);
        }
    }


    private IEnumerator AnimateStarFill(Image star, float startFill, float targetFill)
    {
        float duration = 1f; 
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            star.fillAmount = Mathf.Lerp(startFill, targetFill, timeElapsed / duration); 
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        star.fillAmount = targetFill; 
    }

    public void FailedPanelRestartButtonFunction()
    {
        Time.timeScale = 1f;
        matchFinder.ResetMatchedNeedCandies();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void FailedPanelExitButtonFunction()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SelectLevel");
    }

    public void CompletedPanelHomeButtonFunction()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void CompletedPanelForwardButtonFunction()
    {
        Time.timeScale = 1f;
        int index = SceneManager.GetActiveScene().buildIndex + 1;

        if (SceneManager.GetSceneByBuildIndex(index).name != "SelectLevel" && 
            index < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(index);
        }
    }


    public void OpenLevelCompletedPanel()
    {
        levelCompletedPanel.SetActive(true);
        StartCoroutine(CoroutineOpenStarsAndAnimateScore());
    }

    private IEnumerator CoroutineOpenStarsAndAnimateScore()
    {
        float duration = 3f;
        float timeElapsed = 0f;
        int currentAnimatedScore = 0;

        while (timeElapsed < duration)
        {
            currentAnimatedScore = (int)Mathf.Lerp(0, currentScore, timeElapsed / duration);
            completedScoreText.text = $"{currentAnimatedScore}";

            if (levelEndManager.stars >= 1 && timeElapsed >= 0.5f && !panelStar1.enabled)
            {
                panelStar1.enabled = true;
                starCount.text = "1";
            }
            if (levelEndManager.stars >= 2 && timeElapsed >= 1.0f && !panelBigstar2.enabled)
            {
                panelBigstar2.enabled = true;
                starCount.text = "2";
            }
            if (levelEndManager.stars == 3 && timeElapsed >= 1.5f && !panelStar3.enabled)
            {
                panelStar3.enabled = true;
                starCount.text = "3";
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        currentAnimatedScore = currentScore;
        completedScoreText.text = $"{currentAnimatedScore}";

        if (levelEndManager.stars == 3 && !panelStar3.enabled)
        {
            panelStar3.enabled = true;
            starCount.text = "3";
        }
    }


}
