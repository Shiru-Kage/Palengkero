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

        animator.SetFloat("MoveX", move.x);
        animator.SetFloat("MoveY", move.y);
        animator.SetBool("IsMoving", move != Vector2.zero);
    }
}
