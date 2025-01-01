using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game/GameDatas/GameData")]
public class GameData : ScriptableObject
{
    public GameObject[] PrefabCandies { get { return prefabCandies; } }
    public GameObject CandyBombPrefab { get { return candyBombPrefab; } }

    [Header("Prefab Candies")]
    [SerializeField] private GameObject[] prefabCandies;

    [Header("Candy Bomb Prefab")]
    [SerializeField] private GameObject candyBombPrefab;

}
