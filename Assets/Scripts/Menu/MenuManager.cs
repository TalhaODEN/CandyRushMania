using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private VoiceData voiceData;
    private SaveManager saveManager;

    private Button forwardButton;
    private Button quitButton;
    private Button deleteButton;

    public delegate void ButtonAction();
    public static event ButtonAction OnForwardButtonClicked;
    public static event ButtonAction OnQuitButtonClicked;
    public static event ButtonAction OnDeleteButtonClicked;

    private void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();

        forwardButton = GameObject.Find("ForwardButton").GetComponent<Button>();
        quitButton = GameObject.Find("QuitButton").GetComponent<Button>();
        deleteButton = GameObject.Find("DeleteButton").GetComponent<Button>();

        forwardButton.onClick.AddListener(ForwardButtonClicked);
        quitButton.onClick.AddListener(QuitButtonClicked);
        deleteButton.onClick.AddListener(DeleteButtonClicked);

        OnForwardButtonClicked += HandleForwardButtonClick;
        OnQuitButtonClicked += HandleQuitButtonClick;
        OnDeleteButtonClicked += HandleDeleteButtonClick;


        voiceData = Resources.Load<VoiceData>($"ScriptableObjects/VoiceData");
    }

    private void OnDestroy()
    {
        OnForwardButtonClicked -= HandleForwardButtonClick;
        OnQuitButtonClicked -= HandleQuitButtonClick;
        OnDeleteButtonClicked -= HandleDeleteButtonClick;
    }

    private void ForwardButtonClicked()
    {
        OnForwardButtonClicked?.Invoke();
    }

    private void QuitButtonClicked()
    {
        OnQuitButtonClicked?.Invoke();
    }

    private void DeleteButtonClicked()
    {
        OnDeleteButtonClicked?.Invoke();
    }

    private void HandleForwardButtonClick()
    {
        SceneManager.LoadScene("SelectLevel"); 
    }

    private void HandleQuitButtonClick()
    {
        #if UNITY_EDITOR
            voiceData.isMusicActive = true;
            voiceData.isSoundActive = true;
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            voiceData.isMusicActive = true;
            voiceData.isSoundActive = true;
            Application.Quit();  
        #endif
    }

    private void HandleDeleteButtonClick()
    {
        saveManager.ResetAllLevelData();
    }

    
}
