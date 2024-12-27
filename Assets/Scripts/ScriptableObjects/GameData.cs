using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game/GameDatas/GameData")]
public class GameData : ScriptableObject
{
    public GameObject[] PrefabCandies { get { return prefabCandies; } }

    [Header("Prefab Candies")]
    [SerializeField] private GameObject[] prefabCandies;


}
