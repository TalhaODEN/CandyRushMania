using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CandyNeed
{
    public string candyTag;
    public int neededAmount;
}

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Levels/LevelData")]
public class LevelData : ScriptableObject
{
    public int LevelMoveLimit { get { return levelmoveLimit; } }
    public CandyNeed[] CandyNeeds { get { return candyNeeds; } }


    [Header("Move Limit")]
    [SerializeField] private int levelmoveLimit;

    [Header("Needs")]
    [SerializeField] private CandyNeed[] candyNeeds;
    
}