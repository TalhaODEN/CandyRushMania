using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class Level
{
    public int levelIndex;
    public GameObject levelButton;
    public Sprite levelButtonSprite;  
    public GameObject starsBackground;
    public GameObject[] stars;
}

public class LevelSelectManager : MonoBehaviour
{
    private SaveManager saveManager;

    public Level selectedLevel;
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
        saveManager = FindObjectOfType<SaveManager>();
        for (int i = 0; i < levels.Length; i++) // Seviye 1 her zaman açık olduğu için 1'den başlıyoruz
        {
            Level level = levels[i];

            if(level.levelIndex <= 0)
            {
                continue;
            }

            if (!saveManager.HasLevelData(level.levelIndex) && level.levelIndex != -1)
            {
                saveManager.SaveLevelData(level.levelIndex, 0, false, false);
            }
            int previousLevelIndex = level.levelIndex - 1;
            if (saveManager.HasAnimationPlayed(level.levelIndex) &&
                saveManager.IsLevelCompleted(previousLevelIndex))
            {
                level.levelButton.GetComponent<Image>().sprite = level.levelButtonSprite;
                level.starsBackground.SetActive(true);
            }
            if (level.starsBackground.activeSelf)
            {
                ShowStars(level); 
            }
        }

        foreach (Level level in levels)
        {
            Button levelButton = level.levelButton.GetComponent<Button>();
            levelButton.onClick.AddListener(() => SelectLevel(level));
        }
        isShaking = false;
        forwardButton.GetComponent<Image>().raycastTarget = false;
        CheckForNewlyUnlockedLevels();
    }
    private void ShowStars(Level level)
    {
        int stars = saveManager.GetLevelStars(level.levelIndex);
        switch (stars)
        {
            case 1:
                level.stars[0].SetActive(true);
                break;
            case 2:
                level.stars[0].SetActive(true);
                level.stars[1].SetActive(true);
                break;
            case 3:
                level.stars[0].SetActive(true);
                level.stars[1].SetActive(true);
                level.stars[2].SetActive(true);
                break;
            default:
                break;
        }
    }
    private void CheckForNewlyUnlockedLevels()
    {
        // Burada PlayerPrefs üzerinden hangi seviyenin açıldığını kontrol edebiliriz.
        for (int i = 1; i < levels.Length; i++) // Seviye 1 her zaman açık olduğu için 1'den başlıyoruz
        {
            Level level = levels[i];

            // Eğer level kilitliyse ve bir önceki seviye tamamlandıysa, animasyonu başlat
            if (level.levelButton.GetComponent<Image>().sprite.name == lockedLevelSprite.name)
            {
                if(level.levelIndex <= 1)
                {
                    continue;
                }
                int previousLevelIndex = level.levelIndex - 1;

                if (!saveManager.HasLevelData(previousLevelIndex))
                {
                    continue; // Eğer kaydınız yoksa, bu seviyeye geçmeyin
                }

                // Eğer önceki seviye tamamlandıysa ve animasyon oynatılmadıysa
                if (saveManager.IsLevelCompleted(previousLevelIndex) && 
                    !saveManager.HasAnimationPlayed(level.levelIndex))
                {
                    StartCoroutine(PlayUnlockAnimation(level));
                    saveManager.SaveLevelData(level.levelIndex, 0, false, true);
                }
            }
        }
    }

    private void SelectLevel(Level level)
    {
        selectedLevel = level;
        if(selectedLevel.levelIndex == -1)
        {
            SelectedLevelText.text = "Eklenmedi";
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
            SelectedLevelText.text = "Kilitli";
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
        if (selectedLevel.levelIndex < 1)
        {
            Debug.Log("BU BÖLÜMLER HENÜZ YAPILMADI!!!");
            return;
        }

        PlayerPrefs.SetString("CurrentScene", "Level" + selectedLevel.levelIndex.ToString());
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

    private IEnumerator PlayUnlockAnimation(Level level)
    {
        // İlk olarak butonu 90 derece döndür
        float rotationDuration = 1.2f;
        float timeElapsed = 0f;
        float startRotation = level.levelButton.transform.localRotation.eulerAngles.x;
        float endRotation = 90f; // Hedef dönüş açısı, bu kez 90 derece

        while (timeElapsed < rotationDuration)
        {
            float rotation = Mathf.Lerp(startRotation, endRotation, timeElapsed / rotationDuration);
            level.levelButton.transform.localRotation = Quaternion.Euler(rotation, level.levelButton.transform.localRotation.eulerAngles.y, level.levelButton.transform.localRotation.eulerAngles.z);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Dönüşüm tamamlandıktan sonra sprite değişimi
        level.levelButton.GetComponent<Image>().sprite = level.levelButtonSprite;

        // Buton arka planını aktif et
        level.starsBackground.SetActive(true);

        // Yerel dönüşümle sıfırlama (gerekirse)
        level.levelButton.transform.localRotation = Quaternion.identity;
    }



}
