using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, ICharacterAnimatorData
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GameObject playerIndicator;

    [Header("AFK Settings")]
    [SerializeField] private float afkThreshold = 10f;
    [SerializeField] private HighlightEffect[] highlightEffects;

    private Timer timer;
    private float afkElapsed = 0f;
    private bool isAFK = false;

    private int selectedUIBobbingTweenId = -1;
    private int selectedUIScalingTweenId = -1;

    private Interactable currentInteractable;
    public Interactable CurrentInteractable => currentInteractable;

    private InputSystem_Actions inputActions;

    private Vector2 moveInput;
    public Vector2 MoveInput => moveInput;

    private Transform cachedTransform;
    private Vector2Int currentGridPos;
    private Vector2Int targetGridPos;

    private bool isMoving = false;
    private bool movementBlocked = false;
    public bool GetMovementBlocked() => movementBlocked;

    private void Awake()
    {
        inputActions = InputManager.GetInputActions();
        cachedTransform = transform;

        currentGridPos = PathfindingGrid.Instance.GetGridPosition(transform.position);
        targetGridPos = currentGridPos;
    }

    private void Start()
    {
        playerIndicator.SetActive(false);
        timer = Object.FindFirstObjectByType<Timer>();
        if (timer == null)
        {
            Debug.LogWarning("No Timer found in the scene!");
        }
    }

    private void OnEnable()
    {
        if (inputActions != null)
        {
            inputActions.Player.Move.performed += HandleMove;
            inputActions.Player.Move.canceled += HandleMove;
            inputActions.Player.Interact.performed += HandleInteract;
            inputActions.Player.Indicator.performed += HandleIndicator;
        }
    }

    private void OnDisable()
    {
        if (inputActions != null)
        {
            inputActions.Player.Move.performed -= HandleMove;
            inputActions.Player.Move.canceled -= HandleMove;
            inputActions.Player.Interact.performed -= HandleInteract;
            inputActions.Player.Indicator.performed -= HandleIndicator; 
        }
    }

    private void HandleIndicator(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            TriggerAFKIndicator(context);
        }
    }

    public void ToggleMovement(bool enable)
    {
        movementBlocked = !enable;
        if (!enable)
            moveInput = Vector2.zero;
    }

    private void HandleMove(InputAction.CallbackContext context)
    {
        if (movementBlocked) return;

        if (context.performed)
        {
            Vector2 movementInput = context.ReadValue<Vector2>();

            if (movementInput != Vector2.zero) 
            {
                if (TutorialManager.Instance != null)
                {
                    TutorialManager.Instance.NotifyAction("MovePlayer");
                }
            }
        }
        Vector2 input = context.ReadValue<Vector2>();

        if (input == Vector2.zero)
        {
            moveInput = Vector2.zero;
        }
        else
        {
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                moveInput = new Vector2(Mathf.Sign(input.x), 0f);
            }
            else
            {
                moveInput = new Vector2(0f, Mathf.Sign(input.y));
            }
        }

        if (moveInput != Vector2.zero)
        {
            afkElapsed = 0f;
            if (isAFK)
            {
                isAFK = false;
                playerIndicator.SetActive(false);
                LeanTween.cancel(playerIndicator);

                foreach (var effect in highlightEffects)
                {
                    if (effect != null)
                        effect.SetHighlight(false);
                }
            }
        }
    }

    private void HandleInteract(InputAction.CallbackContext context)
    {
        if (currentInteractable == null)
        {
            Debug.Log("No interactable in range.");
            return;
        }
        var stallCooldown = currentInteractable.GetComponent<StallCooldown>();
        if (stallCooldown != null && stallCooldown.isCoolingDown)
        {
            Debug.Log("Interactable is on cooldown.");
            return; 
        }

        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.NotifyAction("Interact");
        }

        var interactable = currentInteractable;
        Transform origin = interactable.interactionTransform ?? interactable.transform;

        Vector2 center = (Vector2)origin.position + interactable.boxOffset;
        Vector2 halfSize = interactable.boxSize * 0.5f;

        Vector2 playerPos = transform.position;
        bool isInside = Mathf.Abs(playerPos.x - center.x) <= halfSize.x &&
                        Mathf.Abs(playerPos.y - center.y) <= halfSize.y;

        if (isInside)
        {
            ToggleMovement(false);
            interactable.Interact();
        }
        else
        {
            Debug.Log("Too far to interact.");
        }
    }

    private void TriggerAFKIndicator(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!isAFK)
            {
                isAFK = true;
                AFK();
            }
        }
        else if (context.canceled)
        {
            isAFK = false;
            playerIndicator.SetActive(false);
            LeanTween.cancel(playerIndicator);

            foreach (var effect in highlightEffects)
            {
                if (effect != null)
                    effect.SetHighlight(false);
            }
        }
    }

    private void Update()
    {
        if (playerIndicator != null && isAFK)
        {
            Vector3 indicatorPosition = cachedTransform.position;
            indicatorPosition.y += 1f;

            playerIndicator.transform.position = indicatorPosition;
        }
        
        if (timer != null && timer.IsRunning && currentInteractable == null)
        {
            if (moveInput == Vector2.zero && !isMoving)
            {
                afkElapsed += Time.deltaTime;
                if (!isAFK && afkElapsed >= afkThreshold)
                {
                    isAFK = true;
                    AFK();
                }
            }
        }

        if (moveInput != Vector2.zero && !isMoving)
        {
            Vector2Int direction = new Vector2Int((int)moveInput.x, (int)moveInput.y);

            targetGridPos = currentGridPos + direction;

            if (PathfindingGrid.Instance.IsWalkable(targetGridPos.x, targetGridPos.y))
            {
                isMoving = true;
            }
        }

        if (isMoving)
        {
            Vector3 targetWorldPos = PathfindingGrid.Instance.GetWorldPosition(targetGridPos.x, targetGridPos.y);
            cachedTransform.position = Vector3.MoveTowards(cachedTransform.position, targetWorldPos, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(cachedTransform.position, targetWorldPos) < 0.1f)
            {
                currentGridPos = targetGridPos;
                isMoving = false;
                SnapToGrid();
            }
        }
    }

    private void SnapToGrid()
    {
        Vector3 targetWorldPos = PathfindingGrid.Instance.GetWorldPosition(currentGridPos.x, currentGridPos.y);
        cachedTransform.position = targetWorldPos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Interactable interactable))
        {
            currentInteractable = interactable;

            var cooldown = other.GetComponent<StallCooldown>();
            if (cooldown != null && cooldown.isCoolingDown)
                return;

            var stall = other.GetComponent<StallUI>();
            if (stall != null)
            {
                stall.SetHighlight(true);
                stall.isPlayerNearby = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Interactable interactable) && interactable == currentInteractable)
        {
            var stall = other.GetComponent<StallUI>();
            if (stall != null)
            {
                stall.SetHighlight(false);
                stall.isPlayerNearby = false;
            }

            currentInteractable = null;
        }
    }

    private void AFK()
    {
        playerIndicator.SetActive(true);

        foreach (var effect in highlightEffects)
        {
            if (effect != null)
                effect.SetBlinking(true);
        }

        if (selectedUIBobbingTweenId != -1)
        {
            LeanTween.cancel(selectedUIBobbingTweenId);
            selectedUIBobbingTweenId = -1;
        }

        if (selectedUIScalingTweenId != -1)
        {
            LeanTween.cancel(selectedUIScalingTweenId);
            selectedUIScalingTweenId = -1;
        }

        selectedUIBobbingTweenId = LeanTween.moveY(playerIndicator, playerIndicator.transform.position.y + 0.5f, 0.5f)
            .setEaseInOutSine()
            .setLoopPingPong()
            .id;

        selectedUIScalingTweenId = LeanTween.scale(playerIndicator, Vector3.one * 1.05f, 0.5f)
            .setEaseInOutSine()
            .setLoopPingPong()
            .id;
    }
}
