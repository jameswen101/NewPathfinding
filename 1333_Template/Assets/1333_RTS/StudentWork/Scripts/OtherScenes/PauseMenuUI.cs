using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    public Button playButton;
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
    }


    public void GoToMainGame()
    {
        SceneManager.LoadScene("PathfindingTest");

        // Resume the timer (will work only if TimerUI survives the scene load)
        //TimerUI timer = FindObjectOfType<TimerUI>();
        //if (timer != null)
        //{
        //    timer.timerIsRunning = true;
        //}
    }
}
