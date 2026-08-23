using System.Collections.Generic;
using UnityEngine;

public class Manage_Sounds : MonoBehaviour
{
    public List<AudioClip> themes;
    public List<AudioClip> sfx;

    private static Dictionary<string, AudioClip> themeDictionary;
    private static Dictionary<string, AudioClip> sfxDictionary;

    void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();

        themeDictionary = new Dictionary<string, AudioClip>();

        foreach (AudioClip theme in themes)
        {
            themeDictionary[theme.name] = theme;
        }

        sfxDictionary = new Dictionary<string, AudioClip>();

        foreach (AudioClip sfxClip in sfx)
        {
            sfxDictionary[sfxClip.name] = sfxClip;
        }

        PlayTheme("Timer", audioSource);
    }

    public static void PlayTheme(string themeName, AudioSource source)
    {
        if (themeDictionary.ContainsKey(themeName))
        {
            source.clip = themeDictionary[themeName];
            source.Play();
            source.loop = true;
            source.volume = 1.0f;
        }
        else
            Debug.LogWarning("Theme not found: " + themeName);
    }

    public static void PlaySFX(string sfxName, GameObject gameObject, bool loop = false)
    {
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();

        if (audioSource == null) {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = loop;
            audioSource.volume = 1.0f;
        }
        if (sfxDictionary.ContainsKey(sfxName))
        {
            AudioClip clip = sfxDictionary[sfxName];

            if (audioSource.isPlaying && audioSource.clip == clip)
                return;

            audioSource.clip = clip;
            audioSource.Play();
        }
        else
            Debug.LogWarning("SFX not found: " + sfxName);
    }

    public static void StopSFX(GameObject gameObject)
    {
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource != null)
            audioSource.Stop();
    }
}
