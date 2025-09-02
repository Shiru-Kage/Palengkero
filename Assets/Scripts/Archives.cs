using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections.Generic;
using TMPro;

public class Archives : MonoBehaviour
{
    [Header("Parents")]
    [SerializeField] private Transform openings;
    [SerializeField] private Transform endings;

    [Header("Prefab")]
    [SerializeField] private GameObject archiveSlot;

    [Header("Data Source")]
    [SerializeField] private CharacterData[] allCharacters;

    [Header("Cutscene Player")]
    [SerializeField] private Cutscene cutscenePlayer; // 🔹 Reference to your Cutscene script

    private void Start()
    {
        GenerateArchives();
    }

    private void GenerateArchives()
    {
        foreach (Transform child in openings) Destroy(child.gameObject);
        foreach (Transform child in endings) Destroy(child.gameObject);

        HashSet<string> addedOpenings = new HashSet<string>();
        HashSet<string> addedEndings = new HashSet<string>();

        foreach (CharacterData character in allCharacters)
        {
            if (character == null || character.cutscene == null)
                continue;

            var cutsceneData = character.cutscene;

            if (!string.IsNullOrEmpty(cutsceneData.openingCutsceneName) &&
                cutsceneData.openingCutscene != null &&
                !addedOpenings.Contains(cutsceneData.openingCutsceneName))
            {
                CreateArchiveSlot(
                    openings,
                    character.characterName,
                    cutsceneData.openingCutsceneName,
                    cutsceneData.openingIcon,
                    cutsceneData.openingCutscene // 🔹 pass the clip
                );

                addedOpenings.Add(cutsceneData.openingCutsceneName);
            }
        }

        foreach (EndingType type in System.Enum.GetValues(typeof(EndingType)))
        {
            foreach (CharacterData character in allCharacters)
            {
                if (character == null || character.cutscene == null) continue;

                var cutsceneData = character.cutscene;
                if (cutsceneData.endingCutscenes == null) continue;

                foreach (var ending in cutsceneData.endingCutscenes)
                {
                    if (ending == null) continue;

                    string endingName = ending.cutsceneName;

                    if (ending.endingType == type &&
                        !string.IsNullOrEmpty(endingName) &&
                        ending.cutsceneVideo != null &&
                        !addedEndings.Contains(endingName))
                    {
                        CreateArchiveSlot(
                            endings,
                            character.characterName,
                            endingName,
                            ending.cutsceneIcon,
                            ending.cutsceneVideo // 🔹 pass the clip
                        );

                        addedEndings.Add(endingName);
                    }
                }
            }
        }
    }

    private void CreateArchiveSlot(Transform parent, string characterName, string cutsceneName, Sprite icon, VideoClip clip)
    {
        GameObject slot = Instantiate(archiveSlot, parent);

        Transform container = slot.transform.Find("Container");
        Image previewImage = container?.Find("Image")?.GetComponent<Image>();
        GameObject lockObj = container?.Find("Lock")?.gameObject;
        TextMeshProUGUI title = slot.transform.Find("Title")?.GetComponentInChildren<TextMeshProUGUI>();
        Button imageButton = previewImage?.GetComponent<Button>();

        if (title != null)
            title.text = $"{cutsceneName}";

        bool unlocked = ArchiveManager.Instance.IsCutsceneUnlocked(characterName, cutsceneName);

        if (lockObj != null)
            lockObj.SetActive(!unlocked);

        if (previewImage != null)
        {
            previewImage.sprite = icon;
            previewImage.color = unlocked ? Color.white : new Color(0.3f, 0.3f, 0.3f);
        }

        if (imageButton != null)
        {
            imageButton.interactable = unlocked;
            imageButton.onClick.AddListener(() =>
            {
                Debug.Log($"Play archived cutscene: {cutsceneName}");
                if (cutscenePlayer != null)
                {
                    cutscenePlayer.PlayArchivedCutscene(clip, characterName, cutsceneName);
                    gameObject.SetActive(false); // 🔹 Hide Archives menu when playing
                }
                else
                {
                    Debug.LogError("Cutscene player not assigned in Archives!");
                }
            });
        }
    }
}
