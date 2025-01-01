using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Board : MonoBehaviour
{
    private MatchFinder matchFinder;
    private LevelData levelData;

    [Header("Width and Height")]
    [SerializeField] private int width;
    [SerializeField] private int height;

    [Header("Offsets and Spacing")]
    [SerializeField] private float xOffSet;
    [SerializeField] private float yOffSet;
    [SerializeField] private float xSpacing;
    [SerializeField] private float ySpacing;

    [Header("Prefab Object")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject boardObject;

    private GameObject[,] allTiles;
    private GameObject[,] allCandies;

    [Header("Candies")]
    [SerializeField] private GameObject[] prefabCandies;
    [SerializeField] private GameObject candyBombPrefab;
    public GameObject CandyBombPrefab { get { return candyBombPrefab; } }
    public GameObject[] PrefabCandies {get{ return prefabCandies;}}
    public float XSpacing { get { return xSpacing; } }
    public float YSpacing { get { return ySpacing; } }

    public int Width { get { return width; } }
    public int Height { get { return height; } }

    public GameObject[,] AllCandies { get { return allCandies; } set { allCandies = value; } }
    public GameObject[,] AllTiles { get { return allTiles; }}
    private void Start()
    {
        string currentScene = PlayerPrefs.GetString("CurrentScene");

        if (!string.IsNullOrEmpty(currentScene))
        {
            levelData = Resources.Load<LevelData>($"ScriptableObjects/{currentScene}Data");
            GameData gameData = Resources.Load<GameData>($"ScriptableObjects/GameData");
            prefabCandies = gameData.PrefabCandies;
            candyBombPrefab = gameData.CandyBombPrefab;
            matchFinder = FindObjectOfType<MatchFinder>();

            InitializeBoard();
            CreateBoard();
        }
    }

    private void InitializeBoard()
    {
        allTiles = new GameObject[width, height];
        allCandies = new GameObject[width, height];
    }

    private void CreateBoard()
    {
        Vector2 startPoint = new Vector2(boardObject.transform.position.x + xOffSet,
                                         boardObject.transform.position.y + yOffSet);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 tilePosition = CalculateTilePosition(startPoint, x, y);
                GameObject backgroundTile = CreateObject(tilePrefab, tilePosition, $"({x},{y})");
                allTiles[x, y] = backgroundTile;

                bool hasMatch = true;
                GameObject candy = null;

                while (hasMatch)
                {
                    PlaceCandy(tilePosition, backgroundTile, x, y);
                    candy = allCandies[x, y];

                    List<GameObject> matches = matchFinder.FindMatchesAtStart();

                    if (matches.Count == 0)
                    {
                        hasMatch = false;
                    }
                    else
                    {
                        Destroy(candy);
                    }
                }
            }
        }
        
       
        matchFinder = null;
    }

    public Vector2 CalculateTilePosition(Vector2 startPoint, int x, int y)
    {
        return startPoint + new Vector2(x * xSpacing, y * ySpacing);
    }

    public GameObject CreateObject(GameObject prefab, Vector2 position, string name)
    {
        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        obj.name = name;
        return obj;
    }

    private void PlaceCandy(Vector2 position, GameObject parentTile, int x, int y)
    {
        int candyToUse = GetRandomCandyIndex();
        GameObject candy = CreateObject(prefabCandies[candyToUse], position, $"{parentTile.name}_candy");
        candy.GetComponent<Candie>().row = x;
        candy.GetComponent<Candie>().column = y;
        allCandies[x, y] = candy;
    }

    public int GetRandomCandyIndex()
    {
        int totalNeeds = levelData.CandyNeeds.Length;

        List<float> spawnChances = new List<float>();
        float totalWeight = 0f;

        foreach (CandyNeed candyNeed in levelData.CandyNeeds)
        {
            float weight = candyNeed.neededAmount > 0 ? 1.025f : 1; 
            totalWeight += weight; 
            spawnChances.Add(weight);  
        }

        for (int i = totalNeeds; i < prefabCandies.Length; i++)
        {
            spawnChances.Add(1f);  
            totalWeight += 1f;  
        }

        float randomValue = Random.Range(0f, totalWeight);  

        float cumulativeWeight = 0f;

        for (int i = 0; i < prefabCandies.Length; i++)
        {
            cumulativeWeight += spawnChances[i];

            if (randomValue < cumulativeWeight)
            {
                return i;  
            }
        }
        return Random.Range(0, prefabCandies.Length);
    }
}
