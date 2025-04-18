using System;
using Sim.Features.PlayerSystem.Base;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sim.Features.PlayerSystem.Concrete
{
    public class PlayerInputHandler : MonoBehaviour, IPlayerComponent
    {
        // Событийная система для передачи ввода через фасад
        public event Action<Vector2> OnMoveInputChanged;
        public event Action<Vector2> OnLookInputChanged;
        public event Action OnJumpPressed;
        public event Action OnJumpReleased;
        public event Action OnInteractPrimaryPressed;
        public event Action OnInteractSecondaryPressed;
        public event Action OnSprintPressed;
        public event Action OnSprintReleased;

        // Свойства для доступа к значениям ввода через фасад
        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool IsJumpPressed { get; private set; }
        public bool IsSprintPressed { get; private set; }
        public bool IsRunning => MoveInput.magnitude > 0.1f;

        // Ссылка на фасад
        private PlayerFacade _facade;

        // Система ввода Unity
        private PlayerInputData _playerInputData;

        #region Unity Lifecycle

        private void Awake()
        {
            _playerInputData = new PlayerInputData();
        }

        private void OnEnable()
        {
            _playerInputData.Enable();
            SetupInputHandlers();
        }

        private void OnDisable()
        {
            _playerInputData.Disable();
            RemoveInputHandlers();
        }

        #endregion

        #region IPlayerComponent Implementation

        public void Initialize(PlayerFacade facade)
        {
            _facade = facade;
        }

        #endregion

        #region Input Setup

        private void SetupInputHandlers()
        {
            // Настройка обработчиков событий ввода
            _playerInputData.Player.Move.performed += OnMoveInput;
            _playerInputData.Player.Move.canceled += OnMoveInput;

            _playerInputData.Player.Look.performed += OnLookInput;
            _playerInputData.Player.Look.canceled += OnLookInput;

            _playerInputData.Player.Jump.performed += OnJumpPerformed;
            _playerInputData.Player.Jump.canceled += OnJumpCanceled;

            _playerInputData.Player.InteractPrimary.performed += OnInteractPrimaryPerformed;
            _playerInputData.Player.InteractSecondary.performed += OnInteractSecondaryPerformed;

            _playerInputData.Player.Sprint.performed += OnSprintPerformed;
            _playerInputData.Player.Sprint.canceled += OnSprintCanceled;
        }

        private void RemoveInputHandlers()
        {
            // Удаление обработчиков событий ввода
            _playerInputData.Player.Move.performed -= OnMoveInput;
            _playerInputData.Player.Move.canceled -= OnMoveInput;

            _playerInputData.Player.Look.performed -= OnLookInput;
            _playerInputData.Player.Look.canceled -= OnLookInput;

            _playerInputData.Player.Jump.performed -= OnJumpPerformed;
            _playerInputData.Player.Jump.canceled -= OnJumpCanceled;

            _playerInputData.Player.InteractPrimary.performed -= OnInteractPrimaryPerformed;
            _playerInputData.Player.InteractSecondary.performed -= OnInteractSecondaryPerformed;

            _playerInputData.Player.Sprint.performed -= OnSprintPerformed;
            _playerInputData.Player.Sprint.canceled -= OnSprintCanceled;
        }

        #endregion

        #region Input Handlers

        private void OnMoveInput(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
            OnMoveInputChanged?.Invoke(MoveInput);
        }

        private void OnLookInput(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
            OnLookInputChanged?.Invoke(LookInput);
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            IsJumpPressed = true;
            OnJumpPressed?.Invoke();
        }

        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            IsJumpPressed = false;
            OnJumpReleased?.Invoke();
        }

        private void OnInteractPrimaryPerformed(InputAction.CallbackContext context)
        {
            OnInteractPrimaryPressed?.Invoke();
        }

        private void OnInteractSecondaryPerformed(InputAction.CallbackContext context)
        {
            OnInteractSecondaryPressed?.Invoke();
        }

        private void OnSprintPerformed(InputAction.CallbackContext context)
        {
            IsSprintPressed = true;
            OnSprintPressed?.Invoke();
        }

        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            IsSprintPressed = false;
            OnSprintReleased?.Invoke();
        }

        #endregion
    }
}