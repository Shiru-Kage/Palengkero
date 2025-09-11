using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSourcePrefab;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private int poolSize = 10;

    private Queue<AudioSource> availableSources = new Queue<AudioSource>();

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;

    [SerializeField] private AudioClip backgroundMusicClip;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < poolSize; i++)
        {
            AudioSource audioSource = Instantiate(sfxSourcePrefab, transform);
            audioSource.gameObject.SetActive(false);
            availableSources.Enqueue(audioSource);
        }
        PlayBackgroundMusic(backgroundMusicClip);
    }

    public void PlaySFX(AudioClip clip)
    {
        AudioSource audioSource = GetAvailableAudioSource();
        if (audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.volume = sfxVolume;
            audioSource.Play();
            StartCoroutine(ReturnAudioSourceAfterPlay(audioSource, clip.length));
        }
        else
        {
            Debug.LogWarning("No available audio sources in pool.");
        }
    }

    public void PlayMusicWithFadeIn(AudioClip clip, float fadeDuration = 1f)
    {
        // If the music is already playing, fade it out first
        if (musicSource.isPlaying)
        {
            StartCoroutine(FadeOutMusic(fadeDuration));
        }

        // Set up the music source with the new clip and start fading in
        StartCoroutine(FadeInMusic(clip, fadeDuration));
    }

    private IEnumerator FadeInMusic(AudioClip clip, float fadeDuration)
    {
        musicSource.clip = clip;
        musicSource.volume = 0f;  // Start volume at 0
        musicSource.Play();

        float elapsedTime = 0f;  // Track elapsed time for fading
        while (elapsedTime < fadeDuration)
        {
            // Gradually increase the volume over time
            musicSource.volume = Mathf.Lerp(0f, musicVolume, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;  // Increment time
            yield return null;
        }

        musicSource.volume = musicVolume;
    }

    private IEnumerator FadeOutMusic(float fadeDuration)
    {
        float startVolume = musicSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        musicSource.volume = 0f;
        musicSource.Stop();
    }

    public void PlayBackgroundMusic(AudioClip clip)
    {
        if (clip == null) return;

        // Only play the clip if it's not already playing
        if (musicSource.clip == clip && musicSource.isPlaying)
        {
            return;
        }

        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.Play();

        backgroundMusicClip = clip;  // Store the current background music clip
        Debug.Log("Playing background music: " + clip.name);
    }

    private AudioSource GetAvailableAudioSource()
    {
        if (availableSources.Count > 0)
        {
            AudioSource audioSource = availableSources.Dequeue();
            audioSource.gameObject.SetActive(true);
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
        yield return new WaitForSecondsRealtime(clipLength);
        audioSource.gameObject.SetActive(false);
        availableSources.Enqueue(audioSource);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (musicSource != null) musicSource.volume = volume;
    }
    public bool IsMusicPlaying(AudioClip clip)
    {
        return musicSource.clip == clip && musicSource.isPlaying;
    }
    
    public AudioClip MainTheme()
    {
        return Resources.Load<AudioClip>("Audio/ParuParongBukid");
    }
}
