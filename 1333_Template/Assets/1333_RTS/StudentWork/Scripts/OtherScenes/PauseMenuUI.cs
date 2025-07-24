using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenuUI : MonoBehaviour
{
    public Button playButton;
    public Button quitButton;
    [SerializeField] private IntroAudioManager introAudioManager;
    //public TimerUI timer;

    void Start()
    {
        Debug.Log($"[PauseMenuUI] Start called. introAudioManager = {(introAudioManager == null ? "NULL" : introAudioManager.name)}");
        Debug.Log($"[PauseMenuUI] playButton = {(playButton == null ? "NULL" : playButton.name)}");
        Debug.Log($"[PauseMenuUI] quitButton = {(quitButton == null ? "NULL" : quitButton.name)}");

        playButton.onClick.AddListener(GoToMainGame);
        quitButton.onClick.AddListener(QuitGame);

        if (introAudioManager != null)
            introAudioManager.PlayMusic("Gamela", true);
        else
            Debug.LogError("[PauseMenuUI] introAudioManager is NULL at Start()!");
    }



    public void GoToMainGame()
    {
        Debug.Log($"[PauseMenuUI] GoToMainGame called. introAudioManager = {(introAudioManager == null ? "NULL" : introAudioManager.name)}");

        if (introAudioManager != null)
        {
            introAudioManager.StopMusic();
            introAudioManager.PlayMusic("Beep Short");
        }
        else
        {
            Debug.LogError("[PauseMenuUI] introAudioManager is NULL when trying to stop/play music!");
        }

        //SceneManager.LoadScene("PathfindingTest");
        SceneManager.UnloadSceneAsync("PauseMenu");
    }

    public void QuitGame()
    {
        if (introAudioManager != null)
        {
            introAudioManager.StopMusic();
            introAudioManager.PlayMusic("Beep Short");
        }
        else
        {
            Debug.LogError("[PauseMenuUI] introAudioManager is NULL when trying to stop/play music!");
        }

        SceneManager.LoadScene("MainMenu");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.LogError("[PauseMenuUI] introAudioManager is NULL when trying to play SFX!");
        introAudioManager.PlaySFX("Answer Button");
    }
}
