using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HowToPlayUI : MonoBehaviour
{

    public Button playButton;
    [SerializeField] private IntroAudioManager introAudioManager;
    //public TimerUI timer;

    void Start()
    {
        Debug.Log($"[WinScreenUI] Start called. introAudioManager = {(introAudioManager == null ? "NULL" : introAudioManager.name)}");
        Debug.Log($"[WinScreenUI] playButton = {(playButton == null ? "NULL" : playButton.name)}");

        playButton.onClick.AddListener(GoToMainMenu);

        if (introAudioManager != null)
            introAudioManager.PlayMusic("Gamela", true);
        else
            Debug.LogError("[WinScreenUI] introAudioManager is NULL at Start()!");
    }



    public void GoToMainMenu()
    {
        Debug.Log($"[WinScreenUI] GoToMainGame called. introAudioManager = {(introAudioManager == null ? "NULL" : introAudioManager.name)}");

        if (introAudioManager != null)
        {
            introAudioManager.StopMusic();
            introAudioManager.PlayMusic("Beep Short");
        }
        else
        {
            Debug.LogError("[WinScreenUI] introAudioManager is NULL when trying to stop/play music!");
        }

        SceneManager.LoadScene("MainMenu");
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        introAudioManager.PlaySFX("Answer Button");
    }
}
