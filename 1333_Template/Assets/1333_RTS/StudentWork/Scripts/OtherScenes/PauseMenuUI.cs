using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenuUI : MonoBehaviour
{
    public Button playButton;
    [SerializeField] private IntroAudioManager introAudioManager;
    //public TimerUI timer;

    void Start()
    {
        // Look for the timer from the previous scene (PathfindingTest)
        //timer = FindObjectOfType<TimerUI>();
        //if (timer != null)
        //{
        //    timer.StopTimer();  // Pause timer when entering PauseMenu
        //}

        playButton.onClick.AddListener(GoToMainGame); // if applicable
        introAudioManager.PlayMusic("Gamela", true);
    }


    public void GoToMainGame()
    {
        introAudioManager.StopMusic(); // stop music
        introAudioManager.PlayMusic("Beep Short");
        SceneManager.LoadScene("PathfindingTest");

        // Resume the timer (will work only if TimerUI survives the scene load)
        //TimerUI timer = FindObjectOfType<TimerUI>();
        //if (timer != null)
        //{
        //    timer.timerIsRunning = true;
        //}
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        introAudioManager.PlaySFX("Answer Button");
    }
}
