using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public static class GlobalEventSystemBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void EnsureGlobalEventSystem()
    {
#if UNITY_2022_2_OR_NEWER
        var existing = Object.FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include);
#else
        var existing = Object.FindObjectOfType<EventSystem>(true);
#endif
        if (existing != null) { existing.gameObject.SetActive(true); return; }

        var go = new GameObject("Global EventSystem");
        go.AddComponent<EventSystem>();

#if ENABLE_INPUT_SYSTEM
        // If you have an InputActionAsset, assign it here (optional if you use default bindings).
        var ui = go.AddComponent<InputSystemUIInputModule>();
        // ui.actionsAsset = ...;   // assign your asset if needed
#else
        go.AddComponent<StandaloneInputModule>(); // legacy
#endif
        Object.DontDestroyOnLoad(go);
        Debug.Log("[GESB] Created Global EventSystem (DontDestroyOnLoad).");
    }
}
