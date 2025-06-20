using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float radius = 3f;
    public Transform interactionTransform;
    private bool hasInteracted = false;
    private Transform player;

    void Start()
    {
        PlayerController playerController = Object.FindFirstObjectByType<PlayerController>();
        if (playerController != null)
        {
            player = playerController.transform;
        }
        else
        {
            Debug.LogWarning("No PlayerController found in the scene!");
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, interactionTransform.position);

        if (distance <= radius && !hasInteracted)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                hasInteracted = true;
                Interact();
            }
        }
        else if (distance > radius)
        {
            hasInteracted = false;
        }
    }

    public virtual void Interact()
    {
        // This method is meant to be overwritten
    }

    void OnDrawGizmosSelected()
    {
        if (interactionTransform == null)
            interactionTransform = transform;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(interactionTransform.position, radius);
    }
}
