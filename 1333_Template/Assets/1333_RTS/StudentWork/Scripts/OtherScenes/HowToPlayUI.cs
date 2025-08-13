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

    void Start()
    {
        Debug.Log($"[WinScreenUI] Start called. introAudioManager = {(introAudioManager == null ? "NULL" : introAudioManager.name)}");
        Debug.Log($"[WinScreenUI] playButton = {(playButton == null ? "NULL" : playButton.name)}");

        string prev = SceneTracker.SceneHistory.Count >= 2
    ? SceneTracker.SceneHistory[^2]
    : null;

        //add another listener for GoToPauseMenu

        playButton.onClick.AddListener(GoToMainMenu);

        if (introAudioManager != null)
        {
            introAudioManager.PlayMusic("Gamela", true);
        }
        else
        {
            Debug.LogError("[WinScreenUI] introAudioManager is NULL when trying to play SFX!");
        }
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

        // When switching scenes:
        SceneTracker.SceneHistory.Add(SceneManager.GetActiveScene().name); //get name of current scene
        SceneManager.LoadScene("MainMenu");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        introAudioManager.PlaySFX("Answer Button");
    }
}
