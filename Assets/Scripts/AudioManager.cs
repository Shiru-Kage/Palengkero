using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSourcePrefab;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private int poolSize = 10;  // Number of AudioSources to pool for SFX

    private Queue<AudioSource> availableSources = new Queue<AudioSource>();

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize the pool of audio sources
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource audioSource = Instantiate(sfxSourcePrefab, transform);
            audioSource.gameObject.SetActive(false);  // Disable the audio source initially
            availableSources.Enqueue(audioSource);
        }
    }

    // Plays an SFX
    public void PlaySFX(AudioClip clip)
    {
        AudioSource audioSource = GetAvailableAudioSource();
        if (audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.volume = sfxVolume;
            audioSource.Play();
            StartCoroutine(ReturnAudioSourceAfterPlay(audioSource, clip.length));  // Return to pool after clip finishes
        }
        else
        {
            Debug.LogWarning("No available audio sources in pool.");
        }
    }

    // Starts a music track with fade-in effect
    public void PlayMusicWithFadeIn(AudioClip clip, float fadeDuration = 1f)
    {
        if (musicSource.isPlaying)
        {
            StartCoroutine(FadeOutMusic(fadeDuration));
        }
        StartCoroutine(FadeInMusic(clip, fadeDuration));
    }

    // Fades the music in
    private IEnumerator FadeInMusic(AudioClip clip, float fadeDuration)
    {
        musicSource.clip = clip;
        musicSource.volume = 0;
        musicSource.Play();

        float startVolume = 0f;
        while (musicSource.volume < musicVolume)
        {
            musicSource.volume = Mathf.Lerp(startVolume, musicVolume, Time.deltaTime / fadeDuration);
            yield return null;
        }
    }

    // Fades the music out
    private IEnumerator FadeOutMusic(float fadeDuration)
    {
        float startVolume = musicSource.volume;
        while (musicSource.volume > 0)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, Time.deltaTime / fadeDuration);
            yield return null;
        }
        musicSource.Stop();  // Stop music after fading out
    }

    // Retrieves an available audio source from the pool
    private AudioSource GetAvailableAudioSource()
    {
        if (availableSources.Count > 0)
        {
            AudioSource audioSource = availableSources.Dequeue();
            audioSource.gameObject.SetActive(true);  // Enable the audio source
            return audioSource;
        }
        else
        {
            Debug.LogWarning("Audio source pool is empty. Consider increasing pool size.");
            return null;
        }
    }

    private IEnumerator ReturnAudioSourceAfterPlay(AudioSource audioSource, float clipLength)
    {
        yield return new WaitForSeconds(clipLength);  // Wait for the clip to finish
        audioSource.gameObject.SetActive(false);     // Disable the audio source
        availableSources.Enqueue(audioSource);       // Return the source to the pool
    }

    // Plays multiple SFX clips concurrently
    public void PlayMultipleSFX(List<AudioClip> clips)
    {
        foreach (var clip in clips)
        {
            PlaySFX(clip);  // Reuse the existing PlaySFX method for each clip
        }
    }

    // Optional: Set volume dynamically
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (musicSource != null) musicSource.volume = volume;  // Update music source volume immediately
    }
}
