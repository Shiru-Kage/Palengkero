using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class Cutscene : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VideoPlayer videoPlayer;

    private CharacterSelectionManager characterSelectionManager;
    private CharacterData selectedCharacter;   
    private Character_Cutscenes cutscenes;
    private bool hasViewed;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI skipButton;

    private bool isArchiveCutscene = false;
    private string archiveCharacterName;
    private string archiveCutsceneName;

    private void Start()
    {
        characterSelectionManager = CharacterSelectionManager.Instance;

        if (videoPlayer == null || characterSelectionManager == null)
        {
            Debug.LogError("Missing required references! VideoPlayer or CharacterSelectionManager.");
            return;
        }

        videoPlayer.loopPointReached += OnVideoEnd;
    }

    // --- Normal opening cutscene ---
    public void PlayCutsceneForSelectedCharacter()
    {
        isArchiveCutscene = false;

        if (characterSelectionManager.SelectedCharacterData != null)
        {
            selectedCharacter = characterSelectionManager.SelectedCharacterData;
            cutscenes = selectedCharacter.cutscene;

            if (cutscenes != null && cutscenes.openingCutscene != null)
            {
                videoPlayer.clip = cutscenes.openingCutscene;
                videoPlayer.Play();

                if (LevelStateManager.Instance.GetSkipCutsceneOnLoad())
                {
                    hasViewed = true;
                    LevelStateManager.Instance.SetSkipCutsceneOnLoad(false);
                    Debug.Log("Cutscene started after load → skip forced ON.");
                }
                else
                {
                    hasViewed = ArchiveManager.Instance.HasViewedCutscene(cutscenes.openingCutsceneName);
                }

                skipButton.gameObject.SetActive(hasViewed);

                Debug.Log($"Playing opening cutscene for {selectedCharacter.characterName}. First time? {!hasViewed}");
            }
            else
            {
                Debug.LogError("No opening video found for the selected character.");
                OnVideoEnd(videoPlayer);
            }
        }
        else
        {
            Debug.LogError("No selected character data found.");
        }
    }

    // --- Archive rewatch cutscene ---
    public void PlayArchivedCutscene(VideoClip clip, string characterName, string cutsceneName)
    {
        gameObject.SetActive(true);
        isArchiveCutscene = true;

        if (clip != null)
        {
            archiveCharacterName = characterName;
            archiveCutsceneName = cutsceneName;

            videoPlayer.clip = clip;
            videoPlayer.Play();

            skipButton.gameObject.SetActive(true);

            Debug.Log($"Playing archived cutscene: {cutsceneName} for {characterName}");
        }
        else
        {
            Debug.LogError("Archived cutscene video clip is null.");
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        skipButton.gameObject.SetActive(false);
        gameObject.SetActive(false);

        if (!isArchiveCutscene && cutscenes != null && !string.IsNullOrEmpty(cutscenes.openingCutsceneName))
        {
            ArchiveManager.Instance.UnlockCutscene(
                selectedCharacter.characterName,
                cutscenes.openingCutsceneName
            );

            ArchiveManager.Instance.MarkCutsceneAsViewed(cutscenes.openingCutsceneName);

            Debug.Log($"Unlocked opening cutscene: {cutscenes.openingCutsceneName}");
        }

        if (SceneChanger.instance != null && !isArchiveCutscene)
        {
            SceneChanger.instance.ChangeScene("LevelSelect");
        }
    }

    public void SkipCutscene()
    {
        videoPlayer.Stop();
        OnVideoEnd(videoPlayer);
    }
}
