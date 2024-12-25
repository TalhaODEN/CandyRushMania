using System.Collections.Generic;
using UnityEngine;

public class MatchHintSystem : MonoBehaviour
{
    private Board board;
    private SwipeManager swipeManager;
    private CandyShake candyShake;

    [Header("Time")]
    [SerializeField] private float elapsedTime;
    [SerializeField] private float timeLimit;

    [Header("Previous Candy")]
    [SerializeField] private GameObject previousShakedCandy;

    [Header("Potential Matches List")]
    [SerializeField] private List<GameObject> potentialMatches;
    private void Start()
    {
        board = FindObjectOfType<Board>();
        swipeManager = FindObjectOfType<SwipeManager>();
        candyShake = FindObjectOfType<CandyShake>();
        potentialMatches = new List<GameObject>();
        previousShakedCandy = null;
        elapsedTime = 0f;
    }

    private void Update()
    {
        if (swipeManager.isCoroutineRunning || swipeManager.MoveLimit <= 0)
        {
            if(potentialMatches.Count > 0 || previousShakedCandy != null || elapsedTime > 0)
            {
                elapsedTime = 0f;
                previousShakedCandy = null;
                potentialMatches.Clear();
            }

            return;
        }

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= timeLimit)
        {
            elapsedTime = 0f;
            ShowHint();        
        }
    }

    private void ShowHint()
    {
        if (potentialMatches == null || potentialMatches.Count == 0)
        {
            potentialMatches = FindPotentialMatches();
        }

        if (potentialMatches.Count <= 0)
        {
            Debug.LogWarning("Eşleşme bulunamadı.");
            return;
        }

        int index = Random.Range(0, potentialMatches.Count);
        GameObject candy = potentialMatches[index];
        if (previousShakedCandy != null && potentialMatches.Count > 1)
        {
            int iterationCount = 0;
            while (candy == previousShakedCandy && iterationCount < potentialMatches.Count)
            {
                index = (index + 1) % potentialMatches.Count;
                candy = potentialMatches[index];
                iterationCount++;
            }
        }

        previousShakedCandy = candy;
        candyShake.ShakeCandy(candy);
    }

    private List<GameObject> FindPotentialMatches()
    {
        List<GameObject> potentialMatches = new List<GameObject>();
        int width = board.Width;
        int height = board.Height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject candy = board.AllCandies[x, y];

                if (candy == null || potentialMatches.Contains(candy))
                    continue;

                if (x + 1 < width && CheckPotentialMatch(x, y, x + 1, y))
                {
                    potentialMatches.Add(candy);
                    continue;
                }

                if (x - 1 >= 0 && CheckPotentialMatch(x, y, x - 1, y))
                {
                    potentialMatches.Add(candy);
                    continue;
                }

                if (y + 1 < height && CheckPotentialMatch(x, y, x, y + 1))
                {
                    potentialMatches.Add(candy);
                    continue;
                }

                if (y - 1 >= 0 && CheckPotentialMatch(x, y, x, y - 1))
                {
                    potentialMatches.Add(candy);
                    continue;
                }

                if (CheckAdvancedMatch(x, y))
                {
                    potentialMatches.Add(candy);
                }
            }
        }

        return potentialMatches;
    }

    private bool CheckAdvancedMatch(int x, int y)
    {
        GameObject candy = board.AllCandies[x, y];
        if (candy == null) return false;

        GameObject[,] candies = board.AllCandies;

        bool isTShape =
            (x > 0 && x < board.Width - 1 &&
             y > 0 && y < board.Height - 1 &&
             candies[x - 1, y]?.tag == candy.tag &&
             candies[x + 1, y]?.tag == candy.tag &&
             candies[x, y - 1]?.tag == candy.tag &&
             candies[x, y + 1]?.tag == candy.tag);

        bool isLShape =
            (x > 1 && candies[x - 1, y]?.tag == candy.tag &&
             candies[x - 2, y]?.tag == candy.tag &&
             y > 0 && candies[x, y - 1]?.tag == candy.tag) ||
            (x < board.Width - 2 && candies[x + 1, y]?.tag == candy.tag &&
             candies[x + 2, y]?.tag == candy.tag &&
             y < board.Height - 1 && candies[x, y + 1]?.tag == candy.tag);

        return isTShape || isLShape;
    }


    private bool CheckPotentialMatch(int x1, int y1, int x2, int y2)
    {
        GameObject candy1 = board.AllCandies[x1, y1];
        GameObject candy2 = board.AllCandies[x2, y2];

        if (candy1 == null || candy2 == null) return false;

        board.AllCandies[x1, y1] = candy2;
        board.AllCandies[x2, y2] = candy1;

        bool isMatch = IsThereMatchAt(x1, y1) || IsThereMatchAt(x2, y2);

        board.AllCandies[x1, y1] = candy1;
        board.AllCandies[x2, y2] = candy2;

        return isMatch;
    }



    private bool IsThereMatchAt(int x, int y)
    {
        GameObject[,] candies = board.AllCandies;
        GameObject candy = candies[x, y];

        if (candy == null) return false;

        if (x > 0 && x < board.Width - 1 &&
            candies[x - 1, y]?.tag == candy.tag &&
            candies[x + 1, y]?.tag == candy.tag)
        {
            return true;
        }

        if (y > 0 && y < board.Height - 1 &&
            candies[x, y - 1]?.tag == candy.tag &&
            candies[x, y + 1]?.tag == candy.tag)
        {
            return true;
        }

        if (x > 1 && candies[x - 1, y]?.tag == candy.tag &&
            candies[x - 2, y]?.tag == candy.tag)
        {
            return true;
        }

        if (x < board.Width - 2 && candies[x + 1, y]?.tag == candy.tag &&
            candies[x + 2, y]?.tag == candy.tag)
        {
            return true;
        }

        if (y > 1 && candies[x, y - 1]?.tag == candy.tag &&
            candies[x, y - 2]?.tag == candy.tag)
        {
            return true;
        }

        if (y < board.Height - 2 && candies[x, y + 1]?.tag == candy.tag &&
            candies[x, y + 2]?.tag == candy.tag)
        {
            return true;
        }

        return false;
    }

}