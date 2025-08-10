using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnlockedScreenUI : MonoBehaviour
{
    public Button returnButton;
    [SerializeField] private IntroAudioManager introAudioManager;
    private Scene currentScene;

    void Start()
    {
        Debug.Log($"[UnlockedScreenUI] Start called. introAudioManager = {(introAudioManager == null ? "NULL" : introAudioManager.name)}");
        Debug.Log($"[UnlockedScreenUI] playButton = {(returnButton == null ? "NULL" : returnButton.name)}");
        currentScene = SceneManager.GetActiveScene();
        Debug.Log($"[UnlockedScreenUI] Loaded scene: {currentScene.name}");

        returnButton.onClick.AddListener(ReturnToMainGame);
    }



    public void ReturnToMainGame()
    {
        Debug.Log($"[UnlockedScreenUI] ReturnToMainGame called. introAudioManager = {(introAudioManager == null ? "NULL" : introAudioManager.name)}");

        if (introAudioManager != null)
        {
            introAudioManager.PlaySFX("Beep Short");
        }
        else
        {
            Debug.LogError("[UnlockedScreenUI] introAudioManager is NULL when trying to play SFX!");
        }
        SceneManager.UnloadSceneAsync(currentScene);
        // Access the EnemyAIManager in the main scene and update the flag
        EnemyAIManager enemyAI = FindAnyObjectByType<EnemyAIManager>();
        if (enemyAI != null)
        {
            enemyAI.isUnlockedScreenOpen = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"[UnlockedScreenUI] OnPointerEnter called. introAudioManager = {(introAudioManager == null ? "NULL" : introAudioManager.name)}");
        if (introAudioManager != null)
        {
            introAudioManager.PlaySFX("Answer Button");
        }
        
    }
}
