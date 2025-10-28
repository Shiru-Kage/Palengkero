using UnityEngine;
using System.Collections;
public class SaveUI : MonoBehaviour
{
    [SerializeField] private GameObject savePrompt;
    [SerializeField] private GameObject savedConfirmation;

    public void ShowSavePrompt()
    {
        if (savePrompt != null)
            savePrompt.SetActive(true);
    }

    private void HideSavePrompt()
    {
        if (savePrompt != null)
            savePrompt.SetActive(false);
    }

    private void ShowSavedConfirmation()
    {
        if (savedConfirmation != null)
            savedConfirmation.SetActive(true);
    }

    private void HideSavedConfirmation(float delay = 2f)
    {
        if (savedConfirmation != null)
        {
            savedConfirmation.SetActive(true);
            StartCoroutine(HideSavedConfirmationRoutine(delay));
        }
    }

    private IEnumerator HideSavedConfirmationRoutine(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (savedConfirmation != null)
            savedConfirmation.SetActive(false);
    }

    public void TriggerSave(int slotIndex)
    {
        if (SaveGameManager.Instance != null)
        {
            SaveGameManager.Instance.SaveCurrentGame(slotIndex);
        }
        else
        {
            Debug.LogError("SaveGameManager instance is not found!");
        }
    }

    public void TriggerOnConfirmSave()
    {
        SaveGameManager.Instance.OnConfirmSave();
        HideSavedConfirmation();
        HideSavePrompt();
    }

    public void TriggerOnCancelSave()
    {
        SaveGameManager.Instance.OnCancelSave();
        HideSavePrompt();
    }
}