using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject endLevelPanel;
    [Header("Layers to pause on contact")]
    [SerializeField] private LayerMask playerLayerMask;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsInLayerMask(other.gameObject, playerLayerMask))
        {
            if (endLevelPanel != null)
            {
                endLevelPanel.SetActive(true);
                var pauseManager = Object.FindAnyObjectByType<PauseManager>();
                if (pauseManager != null && !pauseManager.IsPaused())
                {
                    pauseManager.ApplyTimePause(true);
                }
            }
        }
    }
    private bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return (mask.value & (1 << obj.layer)) != 0;
    }

}
