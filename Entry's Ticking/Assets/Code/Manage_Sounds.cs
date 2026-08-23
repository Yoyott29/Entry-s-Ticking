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

    public static void PlaySFX(string sfxName, GameObject gameObject, bool randomPitch, bool loop = false)
    {

        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        
        if (audioSource == null) {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = 1.0f;
        }

        audioSource.loop = loop;

        if (sfxDictionary.ContainsKey(sfxName))
        {
            audioSource.clip = sfxDictionary[sfxName];
            audioSource.pitch = randomPitch ? Random.Range(0.8f, 1.2f) : 1.0f;
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
