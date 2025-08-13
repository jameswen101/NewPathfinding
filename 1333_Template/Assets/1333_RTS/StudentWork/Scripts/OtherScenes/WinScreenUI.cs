using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinScreenUI : MonoBehaviour
{
    public Button playButton;
    public Button quitButton;
    [SerializeField] private IntroAudioManager introAudioManager;
    //public TimerUI timer;

    void Start()
    {
        Debug.Log($"[WinScreenUI] Start called. introAudioManager = {(introAudioManager == null ? "NULL" : introAudioManager.name)}");
        Debug.Log($"[WinScreenUI] playButton = {(playButton == null ? "NULL" : playButton.name)}");
        if (playButton == null)
        {
            Debug.LogError("[WinScreenUI] playButton is NULL in Start()!");
            return; // Exit early to avoid null reference errors
        }
        Debug.Log($"[WinScreenUI] quitButton = {(quitButton == null ? "NULL" : quitButton.name)}");
        if (quitButton == null)
        {
            Debug.LogError("[WinScreenUI] quitButton is NULL in Start()!");
            return; // Exit early to avoid null reference errors
        }

        playButton.onClick.AddListener(GoToMainMenu);
        quitButton.onClick.AddListener(QuitGame);

        if (introAudioManager != null)
            introAudioManager.PlayMusic("Party Waltz", true);
        else
            Debug.LogError("[WinScreenUI] introAudioManager is NULL at Start()!");
    }



    public void GoToMainMenu()
    {
        Debug.Log($"[WinScreenUI] GoToMainGame called. introAudioManager = {(introAudioManager == null ? "NULL" : introAudioManager.name)}");

        if (introAudioManager != null)
        {
            introAudioManager.StopMusic();
            introAudioManager.PlaySFX("Beep Short");
        }
        else
        {
            Debug.LogError("[WinScreenUI] introAudioManager is NULL when trying to stop/play music!");
        }

        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        if (introAudioManager != null)
        {
            introAudioManager.StopMusic();
            introAudioManager.PlaySFX("Beep Short");
        }
        else
        {
            Debug.LogError("[WinScreenUI] introAudioManager is NULL when trying to stop/play music on quit!");
        }

        Application.Quit(); // Works in builds only
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        introAudioManager.PlaySFX("Answer Button");
    }
}
