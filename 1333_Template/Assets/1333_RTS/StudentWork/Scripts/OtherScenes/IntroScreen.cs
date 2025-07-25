using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IntroScreen : MonoBehaviour
{
    public Button playButton;
    public Button howToPlayButton;
    public Button quitButton;
    [SerializeField] private IntroAudioManager introAudioManager;

    void Start()
    {
        Debug.Log($"IntroScreen.Start: introAudioManager = {(introAudioManager == null ? "NULL" : introAudioManager.name)}, playButton={(playButton == null ? "NULL" : "OK")}");
        playButton.onClick.AddListener(StartGame);
        howToPlayButton.onClick.AddListener(HowToPlay);
        quitButton.onClick.AddListener(QuitGame);
        if (introAudioManager == null)
        {
            Debug.LogError("[IntroScreen] introAudioManager is NULL before calling StopMusic!");
        }
        introAudioManager.PlayMusic("Gamela", true);
    }

    public void StartGame()
    {
        introAudioManager.StopMusic(); // stop music
        introAudioManager.PlaySFX("Beep Short");
        SceneManager.LoadScene("PathfindingTest");
    }

    public void HowToPlay()
    {
        //no need to stop playing music in How to Play screen
        introAudioManager.PlaySFX("Beep Short");
        SceneTracker.SceneHistory.Add(SceneManager.GetActiveScene().name); //get name of current scene
        SceneManager.LoadScene("HowToPlay");
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
            Debug.LogError("[IntroScreen] introAudioManager is NULL when trying to stop/play music on quit!");
        }

        Application.Quit(); // Works in builds only
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        introAudioManager.PlaySFX("Answer Button");
    }
}
