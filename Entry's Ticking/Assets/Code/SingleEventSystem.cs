using UnityEngine;
using UnityEngine.EventSystems;

public class SingleEventSystem : MonoBehaviour
{
    private static EventSystem instance;

    void Awake()
    {
        var self = GetComponent<EventSystem>();

        if (instance == null)
        {
            instance = self;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != self)
        {
            // A duplicate EventSystem came in with a reloaded scene — remove it
            Destroy(gameObject);
        }
    }
}