using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartIntent : MonoBehaviour
{
    public enum StartMode { None, NewGame, LoadGame }
    public StartMode mode = StartMode.None;

    private void Awake()
    {
        if (FindObjectsOfType<GameStartIntent>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        // Ensure this GameObject persists across scene loads
        DontDestroyOnLoad(gameObject); // persists across scenes
    }
}

