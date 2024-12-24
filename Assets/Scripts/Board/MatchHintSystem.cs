using System.Collections.Generic;
using UnityEngine;

public class MatchHintSystem : MonoBehaviour
{
    private Board board;
    private SwipeManager swipeManager;
    private CandyShake candyShake;
    public float elapsedTime;
    [SerializeField] private float timeLimit;
    private GameObject previousShakedCandy;
    private void Start()
    {
        board = FindObjectOfType<Board>();
        swipeManager = FindObjectOfType<SwipeManager>();
        candyShake = FindObjectOfType<CandyShake>();
        previousShakedCandy = null;
        elapsedTime = 0f;
    }

    private void Update()
    {
        if (swipeManager.isCoroutineRunning)
        {
            elapsedTime = 0f;
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
        List<GameObject> potentialMatches = FindPotentialMatches();
        if (potentialMatches == null || potentialMatches.Count <= 0)
        {
            Debug.LogWarning("ELEMAN YOK");
            return;
        }

        int index = Random.Range(0, potentialMatches.Count);
        GameObject candy = potentialMatches[index];
        int iterationCount = 0;
        while (previousShakedCandy != null && previousShakedCandy == candy && iterationCount < 3)
        {
            index = Random.Range(0, potentialMatches.Count);
            candy = potentialMatches[index];
            iterationCount++;
        }

        previousShakedCandy = candy;
        candyShake.ShakeCandy(candy);
    }

    private List<GameObject> FindPotentialMatches()
    {
        List<GameObject> potentialMatches = new List<GameObject>();

        int width = board.Width;
        int height = board.Height;

        // Tüm tahtayı dolaş
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject candy = board.AllCandies[x, y];

                if (candy == null || potentialMatches.Contains(candy))
                {
                    continue;
                }

                if (x + 1 < width)
                {
                    if (CheckPotentialMatch(x, y, x + 1, y))
                    {
                        potentialMatches.Add(candy);
                        continue;
                    }
                    
                }

                // Soldaki ile yer değiştirip eşleşme kontrolü
                if (x - 1 >= 0)
                {
                    if (CheckPotentialMatch(x, y, x - 1, y))
                    {
                        potentialMatches.Add(candy);
                        continue;
                    }
                    
                }

                // Yukarıdaki ile yer değiştirip eşleşme kontrolü
                if (y + 1 < height)
                {
                    if (CheckPotentialMatch(x, y, x, y + 1))
                    {
                        potentialMatches.Add(candy);
                        continue;
                    }
                    
                }

                // Aşağıdaki ile yer değiştirip eşleşme kontrolü
                if (y - 1 >= 0)
                {
                    if (CheckPotentialMatch(x, y, x, y - 1))
                    {
                        potentialMatches.Add(candy);
                        continue;
                    }
                    
                }
            }
        }

        return potentialMatches;
    }

    private bool CheckPotentialMatch(int x1, int y1, int x2, int y2)
    {
        // Tahta sınırlarını kontrol et
        int width = board.Width;
        int height = board.Height;

        if (x1 < 0 || x1 >= width || y1 < 0 || y1 >= height || x2 < 0 || x2 >= width || y2 < 0 || y2 >= height)
        {
            Debug.Log($"({x1},{y1}) ve ({x2},{y2})");
            return false;
        }

        // Şekerleri geçici olarak yer değiştir
        GameObject[,] candies = board.AllCandies;
        GameObject temp = candies[x1, y1];
        candies[x1, y1] = candies[x2, y2];
        candies[x2, y2] = temp;

        // Eşleşme olup olmadığını kontrol et
        bool isMatch = IsThereMatchAt(x1, y1) || IsThereMatchAt(x2, y2);

        // Şekerleri eski yerine koy
        candies[x2, y2] = candies[x1, y1];
        candies[x1, y1] = temp;

        return isMatch;
    }

    private bool IsThereMatchAt(int x, int y)
    {
        GameObject[,] candies = board.AllCandies;
        GameObject candy = candies[x, y];

        if (candy == null) return false;

        // Yatay eşleşme kontrolü
        if (x > 0 && x < board.Width - 1 &&
            candies[x - 1, y] != null &&
            candies[x + 1, y] != null &&
            candies[x - 1, y].tag == candy.tag &&
            candies[x + 1, y].tag == candy.tag)
        {
            return true;
        }

        // Dikey eşleşme kontrolü
        if (y > 0 && y < board.Height - 1 &&
            candies[x, y - 1] != null &&
            candies[x, y + 1] != null &&
            candies[x, y - 1].tag == candy.tag &&
            candies[x, y + 1].tag == candy.tag)
        {
            return true;
        }

        return false;
    }
}