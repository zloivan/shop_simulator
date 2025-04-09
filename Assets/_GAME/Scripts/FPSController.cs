using _GAME.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;

    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -19.62f;
    [SerializeField] private float airControl = 0.5f;

    [Header("Look Settings")]
    [SerializeField] private float lookSensitivity = 1f;

    [SerializeField] private float lookSmoothing = 0.1f;
    [SerializeField] private float lookXLimit = 80f;

    [Header("Interaction Settings")]
    [SerializeField] private float interactionDistance = 2.5f;

    [SerializeField] private LayerMask interactionLayer = ~0; // Everything by default

    [Header("References")]
    [SerializeField] private Transform cameraHolder;

    [SerializeField] private Transform interactionRayOrigin;

    private PlayerControls playerControls;
    private CharacterController characterController;
    private Camera playerCamera;

    // Input values
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isJumpPressed;
    private bool isRunning;
    private bool isSprinting;

    // Movement state
    private Vector3 moveDirection;
    private Vector3 verticalVelocity;
    private float currentSpeed;
    private float rotationX = 0f;
    private bool isGrounded;

    // Smoothing values
    private Vector2 smoothLookInput;
    private Vector2 lookInputVelocity;

    private void Awake()
    {
        playerControls = new PlayerControls();
        characterController = GetComponent<CharacterController>();
        playerCamera = cameraHolder.GetComponentInChildren<Camera>();

        if (interactionRayOrigin == null)
            interactionRayOrigin = playerCamera.transform;

        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentSpeed = walkSpeed;
    }

    private void OnEnable()
    {
        playerControls.Enable();

        // Set up input callbacks
        playerControls.Player.Move.performed += OnMoveInput;
        playerControls.Player.Move.canceled += OnMoveInput;

        playerControls.Player.Look.performed += OnLookInput;
        playerControls.Player.Look.canceled += OnLookInput;

        playerControls.Player.Jump.performed += OnJumpPerformed;
        playerControls.Player.Jump.canceled += OnJumpCanceled;

        playerControls.Player.InteractPrimary.performed += OnInteractPrimary;
        playerControls.Player.InteractSecondary.performed += OnInteractSecondary;

        playerControls.Player.Sprint.performed += OnSprintPerformed;
        playerControls.Player.Sprint.canceled += OnSprintCanceled;
    }

    private void OnDisable()
    {
        playerControls.Disable();

        // Remove input callbacks
        playerControls.Player.Move.performed -= OnMoveInput;
        playerControls.Player.Move.canceled -= OnMoveInput;

        playerControls.Player.Look.performed -= OnLookInput;
        playerControls.Player.Look.canceled -= OnLookInput;

        playerControls.Player.Jump.performed -= OnJumpPerformed;
        playerControls.Player.Jump.canceled -= OnJumpCanceled;

        playerControls.Player.InteractPrimary.performed -= OnInteractPrimary;
        playerControls.Player.InteractSecondary.performed -= OnInteractSecondary;

        playerControls.Player.Sprint.performed -= OnSprintPerformed;
        playerControls.Player.Sprint.canceled -= OnSprintCanceled;
    }

    #region Input Callbacks

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        // Determine if we're running
        isRunning = moveInput.magnitude > 0.1f;
    }

    private void OnLookInput(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        isJumpPressed = true;
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        isJumpPressed = false;
    }

    private void OnInteractPrimary(InputAction.CallbackContext context)
    {
        Debug.Log("Interacting primary");
        // Primary interaction (E key or Left Mouse Button)
        TryInteractPrimary();
    }

    private void OnInteractSecondary(InputAction.CallbackContext context)
    {
        Debug.Log("Interacting secondary");
        // Secondary interaction (Right Mouse Button)
        TryInteractSecondary();
    }

    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        isSprinting = true;
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        isSprinting = false;
    }

    #endregion

    private void Update()
    {
        UpdateMovementSpeed();
        HandleMovement();
        HandleLook();
    }

    private void UpdateMovementSpeed()
    {
        // Update speed based on sprint and running states
        if (isGrounded)
        {
            if (isSprinting && isRunning)
                currentSpeed = sprintSpeed;
            else if (isRunning)
                currentSpeed = runSpeed;
            else
                currentSpeed = walkSpeed;
        }
    }

    private void HandleMovement()
    {
        isGrounded = characterController.isGrounded;

        if (isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f; // Small negative value instead of zero
        }

        // Calculate movement direction relative to camera orientation
        float moveX = moveInput.x;
        float moveZ = moveInput.y;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move = Vector3.ClampMagnitude(move, 1f); // Normalize the vector to prevent faster diagonal movement

        // Apply air control reduction when not grounded
        if (!isGrounded)
        {
            move *= airControl;
        }

        // Apply movement with current speed
        characterController.Move(move * currentSpeed * Time.deltaTime);

        // Handle jumping
        if (isJumpPressed && isGrounded)
        {
            verticalVelocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            isJumpPressed = false; // Reset to prevent continuous jumping
        }

        // Apply gravity
        verticalVelocity.y += gravity * Time.deltaTime;
        characterController.Move(verticalVelocity * Time.deltaTime);
    }

    private void HandleLook()
    {
        // Apply smoothing to the look input
        smoothLookInput = Vector2.SmoothDamp(
            smoothLookInput,
            lookInput,
            ref lookInputVelocity,
            lookSmoothing
        );

        // Calculate camera rotation with sensitivity
        float mouseX = smoothLookInput.x * lookSensitivity;
        float mouseY = smoothLookInput.y * lookSensitivity;

        // Rotate vertically (camera pitch)
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        cameraHolder.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // Rotate horizontally (player yaw)
        transform.rotation *= Quaternion.Euler(0f, mouseX, 0f);
    }

    private void TryInteractPrimary()
    {
        if (Physics.Raycast(interactionRayOrigin.position, interactionRayOrigin.forward, out RaycastHit hit,
                interactionDistance, interactionLayer))
        {
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                interactable.InteractPrimary(this);
                Debug.Log("Primary interaction with: " + hit.collider.gameObject.name);
            }
        }
    }

    private void TryInteractSecondary()
    {
        if (Physics.Raycast(interactionRayOrigin.position, interactionRayOrigin.forward, out RaycastHit hit,
                interactionDistance, interactionLayer))
        {
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                interactable.InteractSecondary(this);
                Debug.Log("Secondary interaction with: " + hit.collider.gameObject.name);
            }
        }
    }

    // Public methods for external scripts to access information
    public Vector3 GetMovementDirection() => moveDirection;
    public bool IsGrounded() => isGrounded;
    public bool IsRunning() => isRunning;
    public bool IsSprinting() => isSprinting;
    public float GetCurrentSpeed() => currentSpeed;
}