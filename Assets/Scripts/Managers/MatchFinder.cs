using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MatchFinder : MonoBehaviour
{
    private Board board;
    private SwipeManager swipeManager;
    private void Start()
    {
        board = FindObjectOfType<Board>();
        swipeManager = FindObjectOfType<SwipeManager>();
    }
    public List<GameObject> FindMatchesAtStart()
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

        return matchedCandies;
    }
    public void FindMatchesForSwipe()
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
            return;
        }

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

        if(matchedCandies.Count <= 0)
        {
            swipeManager.isMatchedAnything = false;
            yield break;
        }
        swipeManager.isMatchedAnything = true;
        StartCoroutine(ExplosionAnimationController(matchedCandies));
    }
    
    private IEnumerator ExplosionAnimationController(List<GameObject> matchedCandies)
    {
        List<Coroutine> runningAnimations = new List<Coroutine>();
        Coroutine animation = null;
        foreach(GameObject matchedCandy in matchedCandies)
        {
            animation = StartCoroutine(PlayCandyExplosionAnimation(matchedCandy));
            runningAnimations.Add(animation);
        }

        //yield return WaitForExplosionAnimations(runningAnimations);

        CleanDestroyedCandies(matchedCandies);

        yield return null;
    }
    private IEnumerator WaitForExplosionAnimations(List<Coroutine> runningAnimations)
    {
        while (runningAnimations.Count > 0)
        {
            for (int i = runningAnimations.Count - 1; i >= 0; i--)
            {
                if (runningAnimations[i] == null)
                {
                    runningAnimations.RemoveAt(i);
                }
            }
            yield return null;
        }

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
}
