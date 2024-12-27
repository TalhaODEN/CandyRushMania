using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private VoiceData voiceData;

    private Button forwardButton;
    private Button quitButton;

    public delegate void ButtonAction();
    public static event ButtonAction OnForwardButtonClicked;
    public static event ButtonAction OnQuitButtonClicked;

    private void Start()
    {
        forwardButton = GameObject.Find("ForwardButton").GetComponent<Button>();
        quitButton = GameObject.Find("QuitButton").GetComponent<Button>();

        forwardButton.onClick.AddListener(ForwardButtonClicked);
        quitButton.onClick.AddListener(QuitButtonClicked);

        OnForwardButtonClicked += HandleForwardButtonClick;
        OnQuitButtonClicked += HandleQuitButtonClick;

        voiceData = Resources.Load<VoiceData>($"ScriptableObjects/VoiceData");
    }

    private void OnDestroy()
    {
        OnForwardButtonClicked -= HandleForwardButtonClick;
        OnQuitButtonClicked -= HandleQuitButtonClick;
    }

    private void ForwardButtonClicked()
    {
        OnForwardButtonClicked?.Invoke();
    }

    private void QuitButtonClicked()
    {
        OnQuitButtonClicked?.Invoke();
    }

    private void HandleForwardButtonClick()
    {
        SceneManager.LoadScene(1); 
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

    public void DisableButtonInteractivity()
    {
        forwardButton.interactable = false;
        quitButton.interactable = false;
    }
}
