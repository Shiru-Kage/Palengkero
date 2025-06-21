using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class DialogueBoxController : MonoBehaviour
{
    public TextMeshProUGUI barText;
    public TextMeshProUGUI personNameTextDisplay;

    private int sentenceIndex = -1;
    private StoryScene currentScene;
    private State state = State.COMPLETED;
    private bool skipAttempt = false;
    private float typingDelay = 0.05f;
    private AudioSource audioSource;
    private RectTransform rectTransform;
    private Vector2 originalPosition;

    public Events events;

    [System.Serializable]
    public struct Events
    {
        public UnityEvent OnDialogueStart;
        public UnityEvent OnDialogueNext;
        public UnityEvent OnDialogueEnd;
    }

    private enum State
    {
        IDLE, PLAYING, COMPLETED
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void ClearText()
    {
        barText.text = "";
        personNameTextDisplay.text = "";
    }

    public void PlayScene(StoryScene scene)
    {
        events.OnDialogueStart.Invoke();
        currentScene = scene;
        sentenceIndex = -1;
        PlayNextSentence();
    }

    public void PlayNextSentence()
    {
        sentenceIndex++;
        if (sentenceIndex >= currentScene.sentences.Count)
        {
            Debug.LogWarning("No more sentences to play.");
            events.OnDialogueEnd.Invoke();
            return;
        }

        events.OnDialogueNext.Invoke();
        var sentence = currentScene.sentences[sentenceIndex];

        StartCoroutine(HandleSentence(sentence));
    }

    private IEnumerator HandleSentence(StoryScene.Sentence sentence)
    {
        CharacterData runtimeCharacterData = CharacterSelectionManager.Instance.SelectedRuntimeCharacter?.characterData;
        CharacterData activeSpeaker = null;

        if (sentence.speakers == null || sentence.speakers.Length == 0)
        {
            // If no speakers, clear speaker UI and auto-advance
            personNameTextDisplay.text = "";
            yield return StartCoroutine(TypeText(sentence.text));
            yield return new WaitForSeconds(1f);
            if (IsLastSentence())
            {
                events.OnDialogueEnd.Invoke(); // ✅ ensure scene ends
            }
            else
            {
                PlayNextSentence(); // continue if there are more sentences
            }
            yield break;
        }

        // Determine the speaker before typing starts
        foreach (var speaker in sentence.speakers)
        {
            if (speaker == runtimeCharacterData)
            {
                activeSpeaker = speaker;
                break;
            }
        }

        // Fallback to first speaker if runtime character is not in list
        activeSpeaker ??= sentence.speakers[0];

        // Immediately assign name and color
        personNameTextDisplay.text = activeSpeaker.characterName;
        personNameTextDisplay.color = activeSpeaker.textColor;

        // Play audio and effects before/while text types
        PlayAudio(activeSpeaker, sentence.dialogueAudio);
        ActDialogueBoxShake(sentence);

        // Start typing text
        yield return StartCoroutine(TypeText(sentence.text));
    }


    public int GetCurrentSentenceIndex()
    {
        return sentenceIndex;
    }

    public bool IsCompleted()
    {
        return state == State.COMPLETED;
    }

    public bool IsPrinting()
    {
        return state == State.PLAYING;
    }

    public bool IsLastSentence()
    {
        return sentenceIndex + 1 == currentScene.sentences.Count;
    }

    private IEnumerator TypeText(string text)
    {
        barText.text = "";
        state = State.PLAYING;
        int charIndex = 0;

        while (state != State.COMPLETED)
        {
            if (skipAttempt)
            {
                barText.text = text;
                skipAttempt = false;
                break; // Exit loop early to avoid overflow
            }

            if (charIndex >= text.Length)
            {
                state = State.COMPLETED;
                break;
            }

            char currentChar = text[charIndex];
            barText.text += currentChar;
            charIndex++;

            yield return new WaitForSeconds(typingDelay);
        }

        state = State.COMPLETED;
    }


    public void SkipPrinting()
    {
        if (state != State.PLAYING) return;
        skipAttempt = true;
    }

    private void PlayAudio(CharacterData speaker, AudioClip overrideClip = null)
    {
        AudioClip newClip = overrideClip ?? speaker.voiceClip;
        if (newClip != null && (audioSource.clip != newClip || !audioSource.isPlaying))
        {
            audioSource.clip = newClip;
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }

    private void ActDialogueBoxShake(StoryScene.Sentence sentence)
    {
        switch (sentence.dialogueBoxActionTypes)
        {
            case StoryScene.Sentence.DialogueBoxActionType.SHAKE:
                ShakeDialogueBox(0.5f, 5f);
                break;

            case StoryScene.Sentence.DialogueBoxActionType.SIDEWAYS:
                ShakeSideways(0.5f, 5f);
                break;
        }
    }

    private void ShakeDialogueBox(float duration, float intensity)
    {
        LeanTween.value(gameObject, 0f, 1f, duration).setOnUpdate((float value) =>
        {
            float randomX = Random.Range(-intensity, intensity);
            float randomY = Random.Range(-intensity, intensity);
            rectTransform.anchoredPosition = new Vector2(originalPosition.x + randomX, originalPosition.y + randomY);
        }).setOnComplete(() =>
        {
            rectTransform.anchoredPosition = originalPosition;
        });
    }

    private void ShakeSideways(float duration, float intensity)
    {
        LeanTween.value(gameObject, 0f, 1f, duration).setOnUpdate((float value) =>
        {
            float randomX = Random.Range(-intensity, intensity);
            rectTransform.anchoredPosition = new Vector2(originalPosition.x + randomX, originalPosition.y);
        }).setOnComplete(() =>
        {
            rectTransform.anchoredPosition = originalPosition;
        });
    }
}
