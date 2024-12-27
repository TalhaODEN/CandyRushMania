using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class Level
{
    public int levelIndex;
    public GameObject levelButton;
}

public class LevelSelectManager : MonoBehaviour
{
    private Level selectedLevel;

    [Header("Levels")]
    [SerializeField] private Level[] levels;

    [Header("Selected Level Text")]
    [SerializeField] private Text SelectedLevelText;

    [Header("Forward Button")]
    [SerializeField] private GameObject forwardButton;

    private void Start()
    {
        foreach (Level level in levels)
        {
            Button levelButton = level.levelButton.GetComponent<Button>();
            levelButton.onClick.AddListener(() => SelectLevel(level));
        }

        forwardButton.GetComponent<Image>().raycastTarget = false;
    }

    private void SelectLevel(Level level)
    {
        selectedLevel = level;
        if(selectedLevel.levelIndex <= 1)
        {
            Debug.Log("BU BÖLÜMLER EKLENMEDİ!!!");
            SelectedLevelText.text = "";
            forwardButton.GetComponent<Image>().raycastTarget = false;
            return;
        }
        SelectedLevelText.text = $"{selectedLevel.levelButton.name}";
        forwardButton.GetComponent<Image>().raycastTarget = true;
    }

    public void ReturnMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadSelectedLevel()
    {
        if (selectedLevel == null)
        {
            Debug.Log("SelectedLevel null !!!");
            return;
        }
        if(selectedLevel.levelIndex <= 1)
        {
            Debug.Log("BU BÖLÜMLER HENÜZ YAPILMADI!!!");
            return;
        }
        SceneManager.LoadScene(selectedLevel.levelIndex);
    }

    

}
