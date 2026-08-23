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
            Destroy(gameObject);
    }

    public static void ResetInstance()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        instance = null;
    }
}