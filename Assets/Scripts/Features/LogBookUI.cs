using UnityEngine;
using System.Collections;
using TMPro;

public class LogBookUI : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject logPanel;
    [SerializeField] private Transform logParentObject;
    [SerializeField] private GameObject logPrefab;

    private bool isLogBookActive = true;
    private void Start()
    {
        if (logPrefab == null)
        {
            Debug.LogError("Log Prefab is not assigned!");
        }
    }

    public void AddLog(string logMessage)
    {
        if (logParentObject != null && logPrefab != null)
        {
            GameObject newLog = Instantiate(logPrefab, logParentObject);

            TextMeshProUGUI logText = newLog.GetComponent<TextMeshProUGUI>();

            if (logText != null)
            {
                logText.text = logMessage;
            }
            else
            {
                Debug.LogError("Log prefab does not have a TextMeshProUGUI component.");
            }
        }
        else
        {
            Debug.LogError("Log Parent Object or Log Prefab is missing!");
        }
    }

    public void SetLogBookActive(bool isActive)
    {
        isLogBookActive = isActive;

        if (!isActive)
        {
            logPanel.SetActive(false);
        }
        else
        {
            logPanel.SetActive(true);
        }
    }
}
