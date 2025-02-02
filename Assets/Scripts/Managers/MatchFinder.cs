using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CandyBomb
{
    public int Row;
    public int Column;
}

public class MatchFinder : MonoBehaviour
{
    public CandyNeed[] MatchedNeedCandies { get { return matchedNeedCandies; } }
    public CandyBomb candyBomb { get; set; }

    private Board board;
    private SwipeManager swipeManager;
    private UIManager uiManager;
    private ScoreManager scoreManager;
    private AudioManager audioManager;
    private CandyNeed[] matchedNeedCandies;
    

    private void Start()
    {
        board = FindObjectOfType<Board>();
        swipeManager = FindObjectOfType<SwipeManager>();
        uiManager = FindObjectOfType<UIManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        audioManager = FindObjectOfType<AudioManager>();

        candyBomb = null;
        string currentScene = SceneManager.GetActiveScene().name;
        LevelData levelData = Resources.Load<LevelData>($"ScriptableObjects/{currentScene}Data");
        CandyNeed[] needs = new CandyNeed[levelData.CandyNeeds.Length];
        for (int i = 0; i < needs.Length; i++)
        {
            needs[i] = new CandyNeed()
            {
                candyTag = levelData.CandyNeeds[i].candyTag,
                neededAmount = levelData.CandyNeeds[i].neededAmount
            };
        }
        matchedNeedCandies = new CandyNeed[levelData.CandyNeeds.Length];
        for (int i = 0; i < matchedNeedCandies.Length; i++)
        {
            matchedNeedCandies[i] = new CandyNeed()
            {
                candyTag = levelData.CandyNeeds[i].candyTag,
                neededAmount = 0
            };
        }
    }
    public List<GameObject> FindMatchesAtStart()
    {
        List<GameObject> matchedCandies = new List<GameObject>();

        List<GameObject> horizontalMatches = FindHorizontalMatchesAtStart();
        foreach (var candy in horizontalMatches)
        {
            if (!matchedCandies.Contains(candy))
            {
                matchedCandies.Add(candy);
            }
        }

        List<GameObject> verticalMatches = FindVerticalMatchesAtStart();
        foreach (var candy in verticalMatches)
        {
            if (!matchedCandies.Contains(candy))
            {
                matchedCandies.Add(candy);
            }
        }
        return matchedCandies;
    }


    public IEnumerator FindMatchesForManager()
    {
        List<GameObject> matchedCandies = new List<GameObject>();

        List<GameObject> horizontalMatches = FindHorizontalMatches();
        foreach (var candy in horizontalMatches)
        {
            if (!matchedCandies.Contains(candy))
            {
                matchedCandies.Add(candy);
            }
        }

        List<GameObject> verticalMatches = FindVerticalMatches();
        foreach (var candy in verticalMatches)
        {
            if (!matchedCandies.Contains(candy))
            {
                matchedCandies.Add(candy);
            }
        }

        if (matchedCandies.Count <= 0)
        {
            swipeManager.isMatchedAnything = false;
            yield break;
        }
        swipeManager.isMatchedAnything = true;

        int totalScore = 0;
        foreach (var candy in matchedCandies)
        {
            int matchCount = 1;  
            totalScore += scoreManager.CalculateScore(matchCount);  
            totalScore += scoreManager.CalculateAdditionalScore(matchCount);  
        }

        scoreManager.IncreaseSequentialMatchCount();

        foreach (CandyNeed candyNeed in matchedNeedCandies)
        {
            foreach (GameObject checkCandy in matchedCandies)
            {
                if (checkCandy.tag == candyNeed.candyTag)
                {
                    candyNeed.neededAmount++;
                }
            }
        }
        StartCoroutine(ExplosionAnimationController(matchedCandies));
        uiManager.SetNeedsCount();
        scoreManager.IncreaseExplosionMultiplier();


    }



    public IEnumerator ExplosionAnimationController(List<GameObject> matchedCandies)
    {
        foreach(GameObject matchedCandy in matchedCandies)
        {
            StartCoroutine(PlayCandyExplosionAnimation(matchedCandy));
        }

        audioManager.PlayPoppingCandySfx();

        CleanDestroyedCandies(matchedCandies);

        yield return null;
    }
    private void CleanDestroyedCandies(List<GameObject> matchedCandies)
    {
        var allCandies = board.AllCandies;
        int row = 0;
        int column = 0;

        foreach(GameObject matchedCandy in matchedCandies)
        {
            row = matchedCandy.GetComponent<Candie>().row;
            column = matchedCandy.GetComponent<Candie>().column;
            allCandies[row, column] = null;           
        }
    }
    private List<GameObject> FindHorizontalMatches()
    {
        List<GameObject> horizontalMatches = new List<GameObject>();

        for (int y = 0; y < board.Height; y++)
        {
            for (int x = 0; x < board.Width - 2; x++)
            {
                GameObject firstCandy = board.AllCandies[x, y];
                if (firstCandy != null)
                {
                    int matchCount = 1;
                    while (x + matchCount < board.Width && board.AllCandies[x + matchCount, y] != null &&
                           board.AllCandies[x + matchCount, y].tag == firstCandy.tag)
                    {
                        matchCount++;
                    }

                    if (matchCount >= 3)
                    {
                        
                        for (int i = 0; i < matchCount; i++)
                        {
                            horizontalMatches.Add(board.AllCandies[x + i, y]);
                        }
                        int score = scoreManager.CalculateScore(matchCount);
                        uiManager.UpdateScore(score); 

                        if (matchCount >= 5)
                        {
                            scoreManager.IncreaseExplosionMultiplier(); 
                        }
                        
                    }
                }
            }
        }

        return horizontalMatches;
    }

    private List<GameObject> FindVerticalMatches()
    {
        List<GameObject> verticalMatches = new List<GameObject>();

        for (int y = 0; y < board.Height - 2; y++)
        {
            for (int x = 0; x < board.Width; x++)
            {
                GameObject firstCandy = board.AllCandies[x, y];
                if (firstCandy != null)
                {
                    int matchCount = 1;
                    while (y + matchCount < board.Height && board.AllCandies[x, y + matchCount] != null &&
                           board.AllCandies[x, y + matchCount].tag == firstCandy.tag)
                    {
                        matchCount++;
                    }

                    if (matchCount >= 3)
                    {
                        
                        for (int i = 0; i < matchCount; i++)
                        {
                            verticalMatches.Add(board.AllCandies[x, y + i]);
                        }
                        int score = scoreManager.CalculateScore(matchCount);
                        uiManager.UpdateScore(score); 

                        if (matchCount >= 5)
                        {
                            scoreManager.IncreaseExplosionMultiplier(); 
                        }
                        
                    }
                }
            }
        }

        return verticalMatches;
    }

    private List<GameObject> FindHorizontalMatchesAtStart()
    {
        List<GameObject> horizontalMatches = new List<GameObject>();

        for (int y = 0; y < board.Height; y++)
        {
            for (int x = 0; x < board.Width - 2; x++)
            {
                GameObject firstCandy = board.AllCandies[x, y];
                if (firstCandy != null)
                {
                    int matchCount = 1;
                    while (x + matchCount < board.Width && board.AllCandies[x + matchCount, y] != null &&
                           board.AllCandies[x + matchCount, y].tag == firstCandy.tag)
                    {
                        matchCount++;
                    }

                    if (matchCount >= 3)
                    {
                        for (int i = 0; i < matchCount; i++)
                        {
                            horizontalMatches.Add(board.AllCandies[x + i, y]);
                        }              
                    }
                }
            }
        }
        return horizontalMatches;
    }
    private List<GameObject> FindVerticalMatchesAtStart()
    {
        List<GameObject> verticalMatches = new List<GameObject>();

        for (int y = 0; y < board.Height - 2; y++)
        {
            for (int x = 0; x < board.Width; x++)
            {
                GameObject firstCandy = board.AllCandies[x, y];
                if (firstCandy != null)
                {
                    int matchCount = 1;
                    while (y + matchCount < board.Height && board.AllCandies[x, y + matchCount] != null &&
                           board.AllCandies[x, y + matchCount].tag == firstCandy.tag)
                    {
                        matchCount++;
                    }

                    if (matchCount >= 3)
                    {
                        for (int i = 0; i < matchCount; i++)
                        {
                            verticalMatches.Add(board.AllCandies[x, y + i]);
                        }
                        

                    }
                }
            }
        }

        return verticalMatches;
    }

    private IEnumerator PlayCandyExplosionAnimation(GameObject candy)
    {
        Vector2 originalScale = candy.transform.localScale;
        Vector2 targetScale = Vector2.zero; 

        float duration = 0.15f; 
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            candy.transform.localScale = Vector2.Lerp(originalScale, targetScale, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        candy.transform.localScale = targetScale;
        Destroy(candy);
    }
    public void ResetMatchedNeedCandies()
    {
        foreach (CandyNeed item in matchedNeedCandies)
        {
            item.neededAmount = 0;
        }
    }
}
