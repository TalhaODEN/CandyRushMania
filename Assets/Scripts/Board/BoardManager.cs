using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private Board board;
    private SwipeManager swipeManager;
    private MatchFinder matchFinder;
    private bool hasMoved;

    private void Start()
    {
        board = FindObjectOfType<Board>();
        matchFinder = FindObjectOfType<MatchFinder>();
        swipeManager = FindObjectOfType<SwipeManager>();
    }

    public IEnumerator StartShiftAndSpawnProcess()
    {
        bool hasMatch = false;

        yield return StartCoroutine(matchFinder.FindMatchesForManager());
        hasMatch = swipeManager.isMatchedAnything;

        if (!hasMatch)
        {
            yield break;
        }

        while (hasMatch)
        {
            yield return StartCoroutine(SlideCandies());

            yield return StartCoroutine(matchFinder.FindMatchesForManager());
            hasMatch = swipeManager.isMatchedAnything;

            while (hasMatch)
            {
                yield return StartCoroutine(SlideCandies());

                yield return StartCoroutine(matchFinder.FindMatchesForManager());
                hasMatch = swipeManager.isMatchedAnything;
            }

            yield return StartCoroutine(SpawnCandies());

            yield return StartCoroutine(matchFinder.FindMatchesForManager());
            hasMatch = swipeManager.isMatchedAnything;

            if (!hasMatch)
            {
                yield break;
            }
        }
    }

    private IEnumerator SlideCandies()
    {
        Debug.Log("SlideCandies başladı");
        hasMoved = false;

        List<IEnumerator> slideCoroutines = new List<IEnumerator>();

        for (int y = 0; y < board.Height; y++)
        {
            slideCoroutines.Add(SlideRow(y));
        }

        for (int x = 0; x < board.Width; x++)
        {
            slideCoroutines.Add(SlideColumn(x));
        }

        foreach (var coroutine in slideCoroutines)
        {
            yield return StartCoroutine(coroutine);
        }

        if (hasMoved)
        {
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator SlideRow(int y)
    {
        for (int x = 0; x < board.Width; x++)
        {
            if (board.AllCandies[x, y] == null)
            {
                for (int aboveY = y + 1; aboveY < board.Height; aboveY++)
                {
                    if (board.AllCandies[x, aboveY] != null)
                    {
                        GameObject candyToMove = board.AllCandies[x, aboveY];
                        candyToMove.GetComponent<Candie>().row = x;
                        candyToMove.GetComponent<Candie>().column = y;
                        candyToMove.name = $"{board.AllTiles[x, y].name}_candy";

                        Vector3 startPos = candyToMove.transform.position;
                        Vector3 endPos = board.AllTiles[x, y].transform.position;
                        float duration = 0.03f;
                        float timeElapsed = 0;

                        while (timeElapsed < duration)
                        {
                            candyToMove.transform.position = Vector3.Lerp(startPos, endPos, timeElapsed / duration);
                            timeElapsed += Time.deltaTime;
                            yield return null;
                        }

                        candyToMove.transform.position = endPos;
                        board.AllCandies[x, y] = candyToMove;
                        board.AllCandies[x, aboveY] = null;

                        hasMoved = true;
                        break;
                    }
                }
            }
        }
    }

    private IEnumerator SlideColumn(int x)
    {
        for (int y = 0; y < board.Height; y++)
        {
            if (board.AllCandies[x, y] == null)
            {
                for (int aboveY = y + 1; aboveY < board.Height; aboveY++)
                {
                    if (board.AllCandies[x, aboveY] != null)
                    {
                        GameObject candyToMove = board.AllCandies[x, aboveY];
                        candyToMove.GetComponent<Candie>().row = x;
                        candyToMove.GetComponent<Candie>().column = y;
                        candyToMove.name = $"{board.AllTiles[x, y].name}_candy";

                        Vector3 startPos = candyToMove.transform.position;
                        Vector3 endPos = board.AllTiles[x, y].transform.position;
                        float duration = 0.03f;
                        float timeElapsed = 0;

                        while (timeElapsed < duration)
                        {
                            candyToMove.transform.position = Vector3.Lerp(startPos, endPos, timeElapsed / duration);
                            timeElapsed += Time.deltaTime;
                            yield return null;
                        }

                        candyToMove.transform.position = endPos;
                        board.AllCandies[x, y] = candyToMove;
                        board.AllCandies[x, aboveY] = null;

                        hasMoved = true;
                        break;
                    }
                }
            }
        }
    }

    private IEnumerator SpawnCandies()
    {
        Debug.Log("SpawnCandies başladı");

        // İlk olarak spawn olacak şekerleri yerleştiriyoruz
        List<GameObject> candiesToMove = new List<GameObject>();

        for (int x = 0; x < board.Width; x++)
        {
            candiesToMove.AddRange(SpawnColumn(x));
        }

        // Tüm spawn işlemleri tamamlandıktan sonra animasyonları başlatıyoruz
        List<IEnumerator> moveCoroutines = new List<IEnumerator>();
        foreach (var candy in candiesToMove)
        {
            Vector3 startPos = candy.transform.position;
            Vector3 endPos = board.AllTiles[candy.GetComponent<Candie>().row, candy.GetComponent<Candie>().column].transform.position;
            float duration = 0.1f;
            moveCoroutines.Add(AnimateCandyMovement(candy, startPos, endPos, duration));
        }

        // Animasyonları aynı anda başlatıyoruz
        foreach (var coroutine in moveCoroutines)
        {
            yield return StartCoroutine(coroutine);
        }

        yield return new WaitForSeconds(0.01f);
    }

    private List<GameObject> SpawnColumn(int x)
    {
        List<GameObject> candiesToMove = new List<GameObject>();

        for (int y = 0; y < board.Height; y++)
        {
            if (board.AllCandies[x, y] == null)
            {
                // Spawn pozisyonunu belirle
                Vector2 spawnPosition = board.AllTiles[x, board.Height - 1].transform.position + new Vector3(0, 1f, 0);

                // Yeni şeker oluştur
                int candyToUse = Random.Range(0, board.PrefabCandies.Length);
                GameObject candy = Instantiate(board.PrefabCandies[candyToUse], spawnPosition, Quaternion.identity);
                candy.GetComponent<Candie>().row = x;
                candy.GetComponent<Candie>().column = y;
                candy.name = $"{board.AllTiles[x, y].name}_candy";

                // Spawn edilen şekerleri listeye ekliyoruz
                candiesToMove.Add(candy);
                board.AllCandies[x, y] = candy;
            }
        }

        return candiesToMove;
    }


    private IEnumerator AnimateCandyMovement(GameObject candy, Vector3 startPos, Vector3 endPos, float duration)
    {
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            candy.transform.position = Vector3.Lerp(startPos, endPos, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        candy.transform.position = endPos;
    }
}
