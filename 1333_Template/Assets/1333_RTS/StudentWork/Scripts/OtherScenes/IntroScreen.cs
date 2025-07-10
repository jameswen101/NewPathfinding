using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroScreen : MonoBehaviour
{
    public Button playButton;
    public AudioSource GhostMusic;

    void Start()
    {
        playButton.onClick.AddListener(StartGame);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("PathfindingTest");
        GhostMusic.Play();
    }
}
