using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public static class EventSystemGuard
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneLoaded += HandleSceneLoaded;
        KeepOneEventSystem();
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        KeepOneEventSystem();
    }

    private static void KeepOneEventSystem()
    {
        EventSystem[] eventSystems = Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        if (eventSystems.Length == 0)
        {
            Debug.LogWarning("No EventSystem found in the loaded scenes.");
            return;
        }

        EventSystem activeEventSystem = null;
        Scene activeScene = SceneManager.GetActiveScene();

        foreach (EventSystem eventSystem in eventSystems)
        {
            if (eventSystem.gameObject.scene == activeScene)
            {
                activeEventSystem = eventSystem;
                break;
            }
        }

        if (activeEventSystem == null)
        {
            activeEventSystem = eventSystems[0];
        }

        activeEventSystem.gameObject.SetActive(true);
        activeEventSystem.enabled = true;
        activeEventSystem.SetSelectedGameObject(null);

        foreach (EventSystem eventSystem in eventSystems)
        {
            if (eventSystem != activeEventSystem)
            {
                Object.Destroy(eventSystem.gameObject);
            }
        }
    }
}
