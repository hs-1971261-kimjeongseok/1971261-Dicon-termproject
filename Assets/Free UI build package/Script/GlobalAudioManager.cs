using System.Collections.Generic;
using UnityEngine;

public class GlobalAudioManager : MonoBehaviour
{
    public static GlobalAudioManager Instance { get; private set; }

    private List<AudioSource> audioSources = new List<AudioSource>();
    private float globalVolume = 1.0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterAudioSource(AudioSource audioSource)
    {
        if (!audioSources.Contains(audioSource))
        {
            audioSources.Add(audioSource);
            audioSource.volume *= globalVolume;
        }
    }

    public void UnregisterAudioSource(AudioSource audioSource)
    {
        if (audioSources.Contains(audioSource))
        {
            audioSources.Remove(audioSource);
        }
    }

    public void SetGlobalVolume(float volume)
    {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        globalVolume = volume;
        foreach (var audioSource in audioSources)
        {
            audioSource.volume = volume;
        }
    }

    public float GetGlobalVolume()
    {
        return globalVolume;
    }
}
