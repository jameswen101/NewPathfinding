using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource; // BGM
    [SerializeField] private AudioSource correctPlacementSource; // correct placement SFX
    [SerializeField] private AudioSource spawnSource; // spawned unit/building SFX
    [SerializeField] private AudioSource attackedSource; // knife stabbing SFX after unit gets attacked
    [SerializeField] private AudioSource explosionSource; // explosion SFX after unit/building runs out of health
    [SerializeField] private AudioSource walkingSource; // SFX for when unit travels along the grid
    [SerializeField] private AudioSource hoverButtonSource; // SFX for when a button is hovered
    [SerializeField] private AudioSource incorrectPlacementSource; // SFX for when a unit/building is placed incorrectly
    [SerializeField] private AudioSource beepSource; // SFX for when a button is clicked

    [Header("Audio Clips")]
    [SerializeField] private List<AudioClip> musicClips;
    [SerializeField] private List<AudioClip> sfxClips;

    [SerializeField] private Dictionary<string, AudioClip> musicDict;
    [SerializeField] private Dictionary<string, AudioClip> sfxDict;

    // Mixer groups (optional, can leave commented if not using)
    //[SerializeField] private AudioMixerGroup placementGroup;
    //[SerializeField] private AudioMixerGroup footstepGroup;
    //[SerializeField] private AudioMixerGroup attackGroup;
    //[SerializeField] private AudioMixerGroup spawnGroup;

    private void Awake()
    {
        InitializeDictionaries();
    }

    private void InitializeDictionaries()
    {
        musicDict = new Dictionary<string, AudioClip>();
        sfxDict = new Dictionary<string, AudioClip>();

        foreach (var clip in musicClips)
        {
            if (!musicDict.ContainsKey(clip.name))
                musicDict.Add(clip.name, clip);
        }

        foreach (var clip in sfxClips)
        {
            if (!sfxDict.ContainsKey(clip.name))
                sfxDict.Add(clip.name, clip);
        }
    }

    public void PlayMusic(string name, bool loop = true)
    {
        if (musicDict.TryGetValue(name, out AudioClip clip))
        {
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Music '{name}' not found.");
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySFX(string name)
    {
        if (sfxDict.TryGetValue(name, out AudioClip clip))
        {
            correctPlacementSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SFX '{name}' not found.");
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        correctPlacementSource.volume = Mathf.Clamp01(volume);
    }
}
