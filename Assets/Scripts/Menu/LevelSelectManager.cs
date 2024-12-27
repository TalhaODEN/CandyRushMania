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
    private bool isShaking;

    [Header("Levels")]
    [SerializeField] private Level[] levels;
    [SerializeField] private GameObject levelsPanel;

    [Header("Selected Level Text")]
    [SerializeField] private Text SelectedLevelText;

    [Header("Forward Button")]
    [SerializeField] private GameObject forwardButton;

    [Header("Locked Level Sprite")]
    [SerializeField] private Sprite lockedLevelSprite;


    private void Start()
    {
        foreach (Level level in levels)
        {
            Button levelButton = level.levelButton.GetComponent<Button>();
            levelButton.onClick.AddListener(() => SelectLevel(level));
        }
        isShaking = false;
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
        if(selectedLevel.levelButton.GetComponent<Image>().sprite.name == lockedLevelSprite.name)
        {
            if (!isShaking)
            {
                StartCoroutine(ShakeButton());
            }
            selectedLevel = null;
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

    private IEnumerator ShakeButton()
    {
        isShaking = true;  
        GridLayoutGroup gridComponent = levelsPanel.GetComponent<GridLayoutGroup>();
        gridComponent.enabled = false;
        float shakeAmount = 10f;
        float shakeDuration = 0.5f;
        float elapsedTime = 0f;

        RectTransform rectTransform = selectedLevel.levelButton.GetComponent<RectTransform>();
        Vector2 originalPosition = rectTransform.anchoredPosition;

        while (elapsedTime < shakeDuration)
        {
            rectTransform.anchoredPosition = new Vector2(
                originalPosition.x + UnityEngine.Random.Range(-shakeAmount, shakeAmount), originalPosition.y);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = originalPosition;
        gridComponent.enabled = true;
        isShaking = false;  
    }


}
