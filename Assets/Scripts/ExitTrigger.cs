using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject endLevelPanel;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (endLevelPanel != null)
            {
                endLevelPanel.SetActive(true);
            }
        }
    }

}
