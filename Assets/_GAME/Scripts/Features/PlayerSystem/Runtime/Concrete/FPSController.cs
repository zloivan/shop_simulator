using Sim.Features.InteractionSystem.Base;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Sim.Features.PlayerSystem.Concrete
{
    public class FPSController : MonoBehaviour, IInteractor
    {
        [FormerlySerializedAs("walkSpeed")]
        [Header("Movement Settings")]
        [SerializeField] private float _walkSpeed = 5f;

        [FormerlySerializedAs("runSpeed")] [SerializeField]
        private float _runSpeed = 7f;

        [FormerlySerializedAs("sprintSpeed")] [SerializeField]
        private float _sprintSpeed = 10f;

        [FormerlySerializedAs("jumpForce")] [SerializeField]
        private float _jumpForce = 5f;

        [FormerlySerializedAs("gravity")] [SerializeField]
        private float _gravity = -19.62f;

        [FormerlySerializedAs("airControl")] [SerializeField]
        private float _airControl = 0.5f;

        [FormerlySerializedAs("lookSensitivity")]
        [Header("Look Settings")]
        [SerializeField] private float _lookSensitivity = 1f;

        [FormerlySerializedAs("lookSmoothing")] [SerializeField]
        private float _lookSmoothing = 0.1f;

        [FormerlySerializedAs("lookXLimit")] [SerializeField]
        private float _lookXLimit = 80f;

        [FormerlySerializedAs("interactionDistance")]
        [Header("Interaction Settings")]
        [SerializeField] private float _interactionDistance = 2.5f;

        [FormerlySerializedAs("interactionLayer")] [SerializeField]
        private LayerMask _interactionLayer = ~0; // Everything by default

        [FormerlySerializedAs("cameraHolder")]
        [Header("References")]
        [SerializeField] private Transform _cameraHolder;

        [FormerlySerializedAs("interactionRayOrigin")] [SerializeField]
        private Transform _interactionRayOrigin;


        [FormerlySerializedAs("showInteractionRays")]
        [Header("Gizmos Settings")]
        [SerializeField] private bool _showInteractionRays = true; // Включение/выключение Gizmos

        [FormerlySerializedAs("primaryRayColor")] [SerializeField]
        private Color _primaryRayColor = Color.blue;

        [FormerlySerializedAs("secondaryRayColor")] [SerializeField]
        private Color _secondaryRayColor = Color.red;

        private PlayerControls _playerControls;
        private CharacterController _characterController;


        // Input values
        private Vector2 _moveInput;
        private Vector2 _lookInput;
        private bool _isJumpPressed;
        private bool _isRunning;
        private bool _isSprinting;

        // Movement state
        private Vector3 _moveDirection;
        private Vector3 _verticalVelocity;
        private float _currentSpeed;
        private float _rotationX = 0f;
        private bool _isGrounded;

        // Smoothing values
        private Vector2 _smoothLookInput;
        private Vector2 _lookInputVelocity;

        public Transform Transform => transform;
        public Camera PlayerCamera { get; private set; }

        public float InteractionDistance => _interactionDistance;

        private void Awake()
        {
            _playerControls = new PlayerControls();
            _characterController = GetComponent<CharacterController>();
            PlayerCamera = _cameraHolder.GetComponentInChildren<Camera>();

            if (_interactionRayOrigin == null)
                _interactionRayOrigin = PlayerCamera.transform;

            // Lock and hide cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _currentSpeed = _walkSpeed;
        }

        private void OnEnable()
        {
            _playerControls.Enable();

            // Set up input callbacks
            _playerControls.Player.Move.performed += OnMoveInput;
            _playerControls.Player.Move.canceled += OnMoveInput;

            _playerControls.Player.Look.performed += OnLookInput;
            _playerControls.Player.Look.canceled += OnLookInput;

            _playerControls.Player.Jump.performed += OnJumpPerformed;
            _playerControls.Player.Jump.canceled += OnJumpCanceled;

            _playerControls.Player.InteractPrimary.performed += OnInteractPrimary;
            _playerControls.Player.InteractSecondary.performed += OnInteractSecondary;

            _playerControls.Player.Sprint.performed += OnSprintPerformed;
            _playerControls.Player.Sprint.canceled += OnSprintCanceled;
        }

        private void OnDisable()
        {
            _playerControls.Disable();

            // Remove input callbacks
            _playerControls.Player.Move.performed -= OnMoveInput;
            _playerControls.Player.Move.canceled -= OnMoveInput;

            _playerControls.Player.Look.performed -= OnLookInput;
            _playerControls.Player.Look.canceled -= OnLookInput;

            _playerControls.Player.Jump.performed -= OnJumpPerformed;
            _playerControls.Player.Jump.canceled -= OnJumpCanceled;

            _playerControls.Player.InteractPrimary.performed -= OnInteractPrimary;
            _playerControls.Player.InteractSecondary.performed -= OnInteractSecondary;

            _playerControls.Player.Sprint.performed -= OnSprintPerformed;
            _playerControls.Player.Sprint.canceled -= OnSprintCanceled;
        }

        #region Input Callbacks

        private void OnMoveInput(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();

            // Determine if we're running
            _isRunning = _moveInput.magnitude > 0.1f;
        }

        private void OnLookInput(InputAction.CallbackContext context)
        {
            _lookInput = context.ReadValue<Vector2>();
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            _isJumpPressed = true;
        }

        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            _isJumpPressed = false;
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
            _isSprinting = true;
        }

        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            _isSprinting = false;
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
            if (_isGrounded)
            {
                if (_isSprinting && _isRunning)
                    _currentSpeed = _sprintSpeed;
                else if (_isRunning)
                    _currentSpeed = _runSpeed;
                else
                    _currentSpeed = _walkSpeed;
            }
        }

        private void HandleMovement()
        {
            _isGrounded = _characterController.isGrounded;

            if (_isGrounded && _verticalVelocity.y < 0)
            {
                _verticalVelocity.y = -2f; // Small negative value instead of zero
            }

            // Calculate movement direction relative to camera orientation
            float moveX = _moveInput.x;
            float moveZ = _moveInput.y;

            Vector3 move = transform.right * moveX + transform.forward * moveZ;
            move = Vector3.ClampMagnitude(move, 1f); // Normalize the vector to prevent faster diagonal movement

            // Apply air control reduction when not grounded
            if (!_isGrounded)
            {
                move *= _airControl;
            }

            // Apply movement with current speed
            _characterController.Move(move * _currentSpeed * Time.deltaTime);

            // Handle jumping
            if (_isJumpPressed && _isGrounded)
            {
                _verticalVelocity.y = Mathf.Sqrt(_jumpForce * -2f * _gravity);
                _isJumpPressed = false; // Reset to prevent continuous jumping
            }

            // Apply gravity
            _verticalVelocity.y += _gravity * Time.deltaTime;
            _characterController.Move(_verticalVelocity * Time.deltaTime);
        }

        private void HandleLook()
        {
            // Apply smoothing to the look input
            _smoothLookInput = Vector2.SmoothDamp(
                _smoothLookInput,
                _lookInput,
                ref _lookInputVelocity,
                _lookSmoothing
            );

            // Calculate camera rotation with sensitivity
            float mouseX = _smoothLookInput.x * _lookSensitivity;
            float mouseY = _smoothLookInput.y * _lookSensitivity;

            // Rotate vertically (camera pitch)
            _rotationX -= mouseY;
            _rotationX = Mathf.Clamp(_rotationX, -_lookXLimit, _lookXLimit);
            _cameraHolder.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);

            // Rotate horizontally (player yaw)
            transform.rotation *= Quaternion.Euler(0f, mouseX, 0f);
        }

        private void TryInteractPrimary()
        {
            if (Physics.Raycast(_interactionRayOrigin.position, _interactionRayOrigin.forward, out RaycastHit hit,
                    _interactionDistance, _interactionLayer))
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
            if (Physics.Raycast(_interactionRayOrigin.position, _interactionRayOrigin.forward, out var hit,
                    _interactionDistance, _interactionLayer))
            {
                if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
                {
                    interactable.InteractSecondary(this);
                    Debug.Log("Secondary interaction with: " + hit.collider.gameObject.name);
                }
            }
        }

        // Public methods for external scripts to access information
        public Vector3 GetMovementDirection() => _moveDirection;
        public bool IsGrounded() => _isGrounded;
        public bool IsRunning() => _isRunning;
        public bool IsSprinting() => _isSprinting;
        public float GetCurrentSpeed() => _currentSpeed;

        private void OnDrawGizmos()
        {
            if (!_showInteractionRays || _interactionRayOrigin == null) return;

            Gizmos.color = _primaryRayColor;
            Gizmos.DrawRay(_interactionRayOrigin.position, _interactionRayOrigin.forward * _interactionDistance);

            Gizmos.color = _secondaryRayColor;
            Gizmos.DrawRay(_interactionRayOrigin.position, _interactionRayOrigin.forward * _interactionDistance);
        }
    }
}