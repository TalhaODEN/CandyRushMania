using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwipeManager : MonoBehaviour
{
    private Board board;
    private MatchFinder matchFinder;
    private BoardManager boardManager;
    private LevelData levelData;
    private UIManager uiManager;
    public int MoveLimit { get { return moveLimit;} }

    [Header("Mouse Infos")]
    [SerializeField] private Vector2 startMousePosition;
    [SerializeField] private Vector2 finalMousePosition;
    [SerializeField] private Vector2 direction;

    [Header("Selected and Target Candy")]
    [SerializeField] private GameObject selectedCandy = null;
    [SerializeField] private GameObject targetCandy = null;

    [Header("Selected Candy Infos")]
    [SerializeField] private int selectedRow = -1;
    [SerializeField] private int selectedColumn = -1;

    [Header("Target Candy Infos")]
    [SerializeField] private int targetRow = -1;
    [SerializeField] private int targetColumn = -1;

    [Header("Bool Condition Variable")]
    [SerializeField] public bool isCoroutineRunning;
    [SerializeField] public bool isMatchedAnything;

    [Header("Move Limit")]
    [SerializeField] private int moveLimit;

    private void Start()
    {
        board = FindObjectOfType<Board>();
        matchFinder = FindObjectOfType<MatchFinder>();
        boardManager = FindObjectOfType<BoardManager>();
        uiManager = FindObjectOfType<UIManager>();
        string currentScene = SceneManager.GetActiveScene().name;
        levelData = Resources.Load<LevelData>($"ScriptableObjects/{currentScene}Data");
        moveLimit = levelData.LevelMoveLimit;
        uiManager.SetMoveCount(moveLimit);
        isCoroutineRunning = false;
        isMatchedAnything = false;
    }

    private void Update()
    {
        if(Time.timeScale <= 0f)
        {
            return;
        }
        if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonUp(0) && 
            IsCandySelected() && !isCoroutineRunning)
        {
            ResetVariables();
            return;
        }
        if (Input.GetMouseButtonDown(0) && !isCoroutineRunning)
        {
            if (moveLimit <= 0)
            {
                Debug.LogWarning("HAMLE SINIRINA ULAŞTIN");
                return;
            }
            startMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(startMousePosition, Vector2.zero);
            if (hit.collider != null)
            {
                selectedCandy = hit.collider.gameObject;
                selectedRow = selectedCandy.GetComponent<Candie>().row;
                selectedColumn = selectedCandy.GetComponent<Candie>().column;
            }
        }

        if (Input.GetMouseButtonUp(0) && !isCoroutineRunning) 
        {
            targetRow = targetColumn = -1;
            targetCandy = null;
            if (IsCandySelected())
            {
                return;
            }
            finalMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction = finalMousePosition - startMousePosition;
            StartCoroutine(ChooseTargetCandy()); 
        }
    }

    private bool IsCandySelected()
    {
        return selectedRow < 0 || selectedColumn < 0 || selectedCandy == null ||
               targetRow >= 0 || targetColumn >= 0 || targetCandy != null;
    }

    private IEnumerator ChooseTargetCandy()
    {
        if (direction.magnitude < 0.45f)
        {
            yield break;
        }

        isCoroutineRunning = true;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0 && selectedRow + 1 < board.Width)
            {
                targetRow = selectedRow + 1;
                targetColumn = selectedColumn;
                if (board.AllCandies[targetRow, targetColumn] != null)
                {
                    targetCandy = board.AllCandies[targetRow, targetColumn];
                    yield return StartCoroutine(MoveCandies(selectedCandy, targetCandy));
                }
            }
            else if (direction.x < 0 && selectedRow > 0)
            {
                targetRow = selectedRow - 1;
                targetColumn = selectedColumn;
                if (board.AllCandies[targetRow, targetColumn] != null)
                {
                    targetCandy = board.AllCandies[targetRow, targetColumn];
                    yield return StartCoroutine(MoveCandies(selectedCandy, targetCandy));
                }
            }
        }
        else if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
        {
            if (direction.y > 0 && selectedColumn + 1 < board.Height)
            {
                targetRow = selectedRow;
                targetColumn = selectedColumn + 1;
                if (board.AllCandies[targetRow, targetColumn] != null)
                {
                    targetCandy = board.AllCandies[targetRow, targetColumn];
                    yield return StartCoroutine(MoveCandies(selectedCandy, targetCandy));
                }
            }
            else if (direction.y < 0 && selectedColumn > 0)
            {
                targetRow = selectedRow;
                targetColumn = selectedColumn - 1;
                if (board.AllCandies[targetRow, targetColumn] != null)
                {
                    targetCandy = board.AllCandies[targetRow, targetColumn];
                    yield return StartCoroutine(MoveCandies(selectedCandy, targetCandy));
                }
            }
        }

        isCoroutineRunning = false;
        
    }

    private IEnumerator MoveCandies(GameObject selectedCandy, GameObject targetCandy)
    {
        float duration = 0.2f;
        Vector2 startPosition = board.AllTiles[selectedRow, selectedColumn].transform.position;
        Vector2 targetPosition = board.AllTiles[targetRow, targetColumn].transform.position;
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            selectedCandy.transform.position = Vector2.Lerp(startPosition, targetPosition, timeElapsed / duration);
            targetCandy.transform.position = Vector2.Lerp(targetPosition, startPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        selectedCandy.transform.position = targetPosition;
        targetCandy.transform.position = startPosition;

        IsRevertingAttributes(false);

        yield return StartCoroutine(boardManager.StartShiftAndSpawnProcess());
        if (targetCandy == null || selectedCandy == null)
        {
            moveLimit--;
            ResetVariables();
            uiManager.SetMoveCount(moveLimit);
            yield break;
        }

        timeElapsed = 0;

        while (timeElapsed < duration)
        {
            selectedCandy.transform.position = Vector2.Lerp(targetPosition, startPosition, timeElapsed / duration);
            targetCandy.transform.position = Vector2.Lerp(startPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        selectedCandy.transform.position = startPosition;
        targetCandy.transform.position = targetPosition;

        IsRevertingAttributes(true);

    }

    private bool IsRevertingAttributes(bool check)
    {
        GameObject selectedParentTile = board.AllTiles[selectedRow, selectedColumn];
        GameObject targetParentTile = board.AllTiles[targetRow, targetColumn];
        switch (check)
        {
            case false:

                board.AllCandies[targetRow, targetColumn] = selectedCandy;
                board.AllCandies[selectedRow, selectedColumn] = targetCandy;

                targetCandy.GetComponent<Candie>().row = selectedRow;
                targetCandy.GetComponent<Candie>().column = selectedColumn;

                selectedCandy.GetComponent<Candie>().row = targetRow;
                selectedCandy.GetComponent<Candie>().column = targetColumn;

                selectedCandy.name = $"{targetParentTile.name}_candy";
                targetCandy.name = $"{selectedParentTile.name}_candy";
                break;

            case true:

                board.AllCandies[targetRow, targetColumn] = targetCandy;
                board.AllCandies[selectedRow, selectedColumn] = selectedCandy;

                selectedCandy.GetComponent<Candie>().row = selectedRow;
                selectedCandy.GetComponent<Candie>().column = selectedColumn;

                targetCandy.GetComponent<Candie>().row = targetRow;
                targetCandy.GetComponent<Candie>().column = targetColumn;

                selectedCandy.name = $"{selectedParentTile.name}_candy";
                targetCandy.name = $"{targetParentTile.name}_candy";
                break;
        }
        return true;
    }

    private void ResetVariables()
    {
        selectedRow = selectedColumn = targetRow = targetColumn = -1;
        selectedCandy = targetCandy = null;
    }
}


