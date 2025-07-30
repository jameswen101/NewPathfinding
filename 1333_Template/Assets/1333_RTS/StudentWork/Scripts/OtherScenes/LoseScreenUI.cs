using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoseScreenUI : MonoBehaviour
{
    public Button playButton;
    public Button quitButton;
    [SerializeField] private IntroAudioManager introAudioManager;
    //public TimerUI timer;

    void Start()
    {
        Debug.Log($"[LoseScreenUI] Start called. introAudioManager = {(introAudioManager == null ? "NULL" : introAudioManager.name)}");
        Debug.Log($"[LoseScreenUI] playButton = {(playButton == null ? "NULL" : playButton.name)}");
        if (playButton == null)
        {
            Debug.LogError("[LoseScreenUI] playButton is NULL in Start()!");
            return; // Exit early to avoid null reference errors
        }
        Debug.Log($"[LoseScreenUI] quitButton = {(quitButton == null ? "NULL" : quitButton.name)}");
        if (quitButton == null)
        {
            Debug.LogError("[LoseScreenUI] quitButton is NULL in Start()!");
            return; // Exit early to avoid null reference errors
        }

        playButton.onClick.AddListener(GoToMainMenu);
        quitButton.onClick.AddListener(QuitGame);

        if (introAudioManager != null)
            introAudioManager.PlayMusic("Golden Cage", true);
        else
            Debug.LogError("[LoseScreenUI] introAudioManager is NULL at Start()!");
    }



    public void GoToMainMenu()
    {
        Debug.Log($"[LoseScreenUI] GoToMainGame called. introAudioManager = {(introAudioManager == null ? "NULL" : introAudioManager.name)}");

        if (introAudioManager != null)
        {
            introAudioManager.StopMusic();
            introAudioManager.PlaySFX("Beep Short");
        }
        else
        {
            Debug.LogError("[LoseScreenUI] introAudioManager is NULL when trying to stop/play music!");
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
            Debug.LogError("[LoseScreenUI] introAudioManager is NULL when trying to stop/play music on quit!");
        }

        Application.Quit(); // Works in builds only
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        introAudioManager.PlaySFX("Answer Button");
    }
}
