using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IntroScreen : MonoBehaviour
{
    public Button playButton;
    [SerializeField] private IntroAudioManager introAudioManager;

    void Start()
    {
        playButton.onClick.AddListener(StartGame);
        introAudioManager.PlayMusic("Gamela", true);
    }

    public void StartGame()
    {
        introAudioManager.StopMusic(); // stop music
        introAudioManager.PlayMusic("Beep Short");
        SceneManager.LoadScene("PathfindingTest");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        introAudioManager.PlaySFX("Answer Button");
    }
}
