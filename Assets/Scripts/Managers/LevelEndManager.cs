using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndManager : MonoBehaviour
{
    private LevelData currentLevelData; 
    private SwipeManager swipeManager;
    private MatchFinder matchFinder;
    private UIManager uiManager;
    private ScoreManager scoreManager;

    private bool isEndLevelActive;
    public int stars = 0; 
    private SaveManager saveManager;

    private void Start()
    {
        swipeManager = FindObjectOfType<SwipeManager>();
        matchFinder = FindObjectOfType<MatchFinder>();
        uiManager = FindObjectOfType<UIManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        isEndLevelActive = false;
        saveManager = SaveManager.Instance;
        string currentScene = SceneManager.GetActiveScene().name;
        currentLevelData = Resources.Load<LevelData>($"ScriptableObjects/{currentScene}Data");
    }

    private void Update()
    {
        if (!isEndLevelActive && 
            (swipeManager.MoveLimit <= 0 || HasCollectedRequiredCandies()))
        {
            swipeManager.enabled = false;
            isEndLevelActive = true;
            StartCoroutine(EndLevel());
        }
    }

    private bool HasCollectedRequiredCandies()
    {
        foreach (var candyNeed in currentLevelData.CandyNeeds)
        {
            foreach (CandyNeed collected in matchFinder.MatchedNeedCandies)
            {
                if(collected.candyTag == candyNeed.candyTag && 
                    collected.neededAmount < candyNeed.neededAmount)
                {
                    return false;
                }
            }
        }
        return true; 
    }

    private IEnumerator EndLevel()
    {
        if (swipeManager.MoveLimit <= 0 && !HasCollectedRequiredCandies())
        {
            Debug.Log("Level baþarýlamadý.");
            uiManager.LevelFailedPanel.SetActive(true);
            Time.timeScale = 0f;
            yield break;
        }

        uiManager.UpdateScore
            (uiManager.CurrentScore + 
            scoreManager.AddMoveCountExtraScore(swipeManager.MoveLimit));
        yield return null;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        saveManager.SaveLevelData(currentSceneIndex, stars, true, false);
        yield return new WaitForSeconds(0.2f);
        uiManager.OpenLevelCompletedPanel();
    }
}
