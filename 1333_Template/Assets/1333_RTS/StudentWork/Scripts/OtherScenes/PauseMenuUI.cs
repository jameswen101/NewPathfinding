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
    public Button controlsButton;
    [SerializeField] private IntroAudioManager introAudioManager;
    //public TimerUI timer;

    void Start()
    {
        Debug.Log($"[PauseMenuUI] Start called. introAudioManager = {(introAudioManager == null ? "NULL" : introAudioManager.name)}");
        Debug.Log($"[PauseMenuUI] playButton = {(playButton == null ? "NULL" : playButton.name)}");
        Debug.Log($"[PauseMenuUI] quitButton = {(quitButton == null ? "NULL" : quitButton.name)}");
        Debug.Log($"[PauseMenuUI] controlsButton = {(controlsButton == null ? "NULL" : controlsButton.name)}");

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
            introAudioManager.PlaySFX("Beep Short");
        }
        else
        {
            Debug.LogError("[PauseMenuUI] introAudioManager is NULL when trying to stop/play music!");
        }

        //SceneManager.LoadScene("PathfindingTest");
        SceneManager.UnloadSceneAsync("PauseMenu");
    }

    public void GoToControls()
    {
        Debug.Log($"[PauseMenuUI] GoToControls called. introAudioManager = {(introAudioManager == null ? "NULL" : introAudioManager.name)}");
        if (introAudioManager != null)
        {
            introAudioManager.PlaySFX("Beep Short");
        }
        else
        {
            Debug.LogError("[PauseMenuUI] introAudioManager is NULL when trying to play SFX!");
        }
        SceneTracker.SceneHistory.Add(SceneManager.GetActiveScene().name); //get name of current scene
        SceneManager.LoadScene("HowToPlay 1", LoadSceneMode.Additive);
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
