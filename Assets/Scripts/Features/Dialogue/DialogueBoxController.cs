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
    private Coroutine typingCoroutine;

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
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        ClearText();
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

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(HandleSentence(sentence));
    }

    private IEnumerator HandleSentence(StoryScene.Sentence sentence)
    {
        CharacterData runtimeCharacterData = CharacterSelectionManager.Instance.SelectedRuntimeCharacter?.characterData;
        CharacterData activeSpeaker = null;

        if (sentence.speakers == null || sentence.speakers.Length == 0)
        {
            personNameTextDisplay.text = "";
            yield return StartCoroutine(TypeText(sentence.text));
            yield return new WaitForSeconds(1f);
            if (IsLastSentence())
            {
                events.OnDialogueEnd.Invoke();
            }
            else
            {
                PlayNextSentence();
            }
            yield break;
        }

        foreach (var speaker in sentence.speakers)
        {
            if (speaker == runtimeCharacterData)
            {
                activeSpeaker = speaker;
                break;
            }
        }

        activeSpeaker ??= sentence.speakers[0];

        personNameTextDisplay.text = activeSpeaker.characterName;
        personNameTextDisplay.color = activeSpeaker.textColor;

        PlayAudio(activeSpeaker, sentence.dialogueAudio);
        ActDialogueBoxShake(sentence);

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

        while (charIndex < text.Length)
        {
            if (skipAttempt)
            {
                barText.text = text;
                skipAttempt = false;
                state = State.COMPLETED;
                yield break;
            }

            barText.text += text[charIndex];
            charIndex++;
            yield return new WaitForSeconds(typingDelay);
        }

        state = State.COMPLETED;
    }


    public void SkipPrinting()
    {
        if (state == State.PLAYING)
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
