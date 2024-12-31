using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const string LEVEL_PREFIX = "Level_";

    // Singleton deseni kullanarak sadece bir SaveManager'�n olmas�n� sa�lar�z
    public static SaveManager Instance { get; private set; }

    private void Awake()
    {
        // E�er zaten bir SaveManager var ise, bu nesneyi yok et
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            // E�er bu ilk nesne ise, onu Singleton olarak ayarla
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Bu nesne sahneler aras�nda ta��nacak
        }
    }

    // Seviye verilerini kaydet
    public void SaveLevelData(int levelIndex, int stars, bool isCompleted, bool animationPlayed)
    {
        PlayerPrefs.SetInt(LEVEL_PREFIX + levelIndex + "_Completed", isCompleted ? 1 : 0);
        PlayerPrefs.SetInt(LEVEL_PREFIX + levelIndex + "_Stars", stars);
        PlayerPrefs.SetInt(LEVEL_PREFIX + levelIndex + "_AnimationPlayed", animationPlayed ? 1 : 0);
        PlayerPrefs.Save();
    }
    public bool HasLevelData(int levelIndex)
    {
        return PlayerPrefs.HasKey(LEVEL_PREFIX + levelIndex + "_Completed");
    }
    // Seviye tamamlanma durumu
    public bool IsLevelCompleted(int levelIndex)
    {
        return PlayerPrefs.GetInt(LEVEL_PREFIX + levelIndex + "_Completed", 0) == 1;
    }

    // Seviye y�ld�z say�s�
    public int GetLevelStars(int levelIndex)
    {
        return PlayerPrefs.GetInt(LEVEL_PREFIX + levelIndex + "_Stars", 0);
    }

    // Animasyonun oynat�l�p oynat�lmad��� durumu
    public bool HasAnimationPlayed(int levelIndex)
    {
        return PlayerPrefs.GetInt(LEVEL_PREFIX + levelIndex + "_AnimationPlayed", 0) == 1;
    }

    // T�m seviyeleri s�f�rlama (test i�in)
    public void ResetAllLevelData()
    {
        for (int i = 1; i <= 12; i++) // �rnek olarak 10 seviye
        {
            PlayerPrefs.DeleteKey(LEVEL_PREFIX + i + "_Completed");
            PlayerPrefs.DeleteKey(LEVEL_PREFIX + i + "_Stars");
            PlayerPrefs.DeleteKey(LEVEL_PREFIX + i + "_AnimationPlayed");
        }
        PlayerPrefs.Save();
    }
}
