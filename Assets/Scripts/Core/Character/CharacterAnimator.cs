using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private MonoBehaviour characterSource;

    private ICharacterAnimatorData data;

    private void Awake()
    {
        data = characterSource as ICharacterAnimatorData;

        if (data == null)
        {
            Debug.LogError($"{characterSource} does not implement ICharacterAnimatorData.");
        }
    }

    private void Update()
{
    if (data == null) return;

    Vector2 move = data.MoveInput;

    // Apply threshold to avoid very small movements triggering animation
    float threshold = 0.1f;
    animator.SetFloat("MoveX", Mathf.Abs(move.x) > threshold ? move.x : 0);
    animator.SetFloat("MoveY", Mathf.Abs(move.y) > threshold ? move.y : 0);
    animator.SetBool("IsMoving", move != Vector2.zero);
}



}
