using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Interaction Area")]
    public Vector2 boxSize = new Vector2(3f, 3f);       // Width & height of the square
    public Vector2 boxOffset = Vector2.zero;            // X and Y offset from the origin
    public Transform interactionTransform;

    public virtual void Interact()
    {
        Debug.Log("Interact called on: " + gameObject.name);
    }

    private void OnDrawGizmosSelected()
    {
        if (interactionTransform == null)
            interactionTransform = transform;

        Gizmos.color = Color.yellow;

        Vector3 center = interactionTransform.position + (Vector3)boxOffset;
        Vector3 size = new Vector3(boxSize.x, boxSize.y, 0f);

        Gizmos.DrawWireCube(center, size);
    }
}
