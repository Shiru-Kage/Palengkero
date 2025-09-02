using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class Cutscene : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VideoPlayer videoPlayer;

    private CharacterSelectionManager characterSelectionManager;
    private CharacterData selectedCharacter;   // store reference
    private Character_Cutscenes cutscenes;     // store reference

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI skipButton;

    // 🔹 Flags and temp storage
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

                bool hasViewed = ArchiveManager.Instance.HasViewedCutscene(cutscenes.openingCutsceneName);

                // First time = locked skip button
                // After first time = allow skip
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
            // store just metadata (no ScriptableObject instantiation!)
            archiveCharacterName = characterName;
            archiveCutsceneName = cutsceneName;

            videoPlayer.clip = clip;
            videoPlayer.Play();

            // Always allow skipping in Archive
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

        // Only unlock and mark viewed if it was a normal playthrough
        if (!isArchiveCutscene && cutscenes != null && !string.IsNullOrEmpty(cutscenes.openingCutsceneName))
        {
            ArchiveManager.Instance.UnlockCutscene(
                selectedCharacter.characterName,
                cutscenes.openingCutsceneName
            );

            // ✅ Mark as viewed after first playthrough
            ArchiveManager.Instance.MarkCutsceneAsViewed(cutscenes.openingCutsceneName);

            Debug.Log($"Unlocked opening cutscene: {cutscenes.openingCutsceneName}");
        }

        // Only change scene after real opening cutscenes
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
