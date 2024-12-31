using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private const int BaseScoreFor3Match = 200;
    private const int BaseScoreFor4Match = 400;
    private const int BaseScoreFor5Match = 600;
    private const int BaseExplosionMultiplier = 2;  
    private const int SequentialMatchMultiplier = 100;  

    private int explosionMultiplier = 1;  
    private int sequentialMatchCount = 0;  

    public int CalculateScore(int matchCount)
    {
        int score = 0;

        if (matchCount == 3)
        {
            score = BaseScoreFor3Match;
        }
        else if (matchCount == 4)
        {
            score = BaseScoreFor4Match;
        }
        else if (matchCount == 5)
        {
            score = BaseScoreFor5Match;
        }
        score += sequentialMatchCount * SequentialMatchMultiplier;
        score *= explosionMultiplier;
        return score;
    }

    public int CalculateAdditionalScore(int matchCount)
    {
        if (matchCount > 5)
        {
            return (matchCount - 5) * 100;  
        }
        return 0;
    }

    public void IncreaseExplosionMultiplier()
    {
        if (explosionMultiplier < 4)
        {
            explosionMultiplier++; 
        }
    }
    public void IncreaseSequentialMatchCount()
    {
        sequentialMatchCount++;
    }

    public void ResetSequentialMatchCount()
    {
        sequentialMatchCount = 0;
    }

    public int AddMoveCountExtraScore(int moveCount)
    {
        if(moveCount <= 0)
        {
            Debug.LogWarning
                ("Parametre olarak gönderilen hamle sayýsý 0 veya 0 dan küçük olamaz");
            return 0;       
        }
        if (moveCount < 3)
        {
            return moveCount * 1000;
        }
        if (moveCount < 5)
        {
            return moveCount * 1400;
        }
        return moveCount * 2000;
    }
}
