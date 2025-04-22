using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sim.Features.InteractionSystem.Base;
using Sim.Features.PlayerSystem.Base;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sim.Features.PlayerSystem.PlayerComponents
{
    public class PlayerInputHandlerComponent : MonoBehaviour, IPlayerComponent
    {
        // Событийная система для передачи ввода через фасад
        [PublicAPI] public event Action<Vector2> OnMoveInputChanged;
        [PublicAPI] public event Action<Vector2> OnLookInputChanged;
        [PublicAPI] public event Action OnJumpPressed;
        [PublicAPI] public event Action OnJumpReleased;
        [PublicAPI] public event Action<InteractionType> OnInteractPressed;
        [PublicAPI] public event Action OnSprintPressed;
        [PublicAPI] public event Action OnSprintReleased;

        // Свойства для доступа к значениям ввода через фасад
        [PublicAPI] public Vector2 MoveInput { get; private set; }
        [PublicAPI] public Vector2 LookInput { get; private set; }
        [PublicAPI] public bool IsJumpPressed { get; private set; }
        [PublicAPI] public bool IsSprintPressed { get; private set; }
        [PublicAPI] public bool IsRunning => MoveInput.magnitude > 0.1f;

        [SerializeField] private bool _isDebug;
        

        // Ссылка на фасад
        private PlayerFacade _facade;

        // Система ввода Unity
        private PlayerInputData _playerInputData;
        private Dictionary<string, InteractionType> _interactionMap = new();

        #region Unity Lifecycle

        private void OnEnable()
        {
            _playerInputData?.Enable();
        }

        private void OnDisable()
        {
            _playerInputData?.Disable();
        }

        #endregion

        #region IPlayerComponent Implementation

        public void Initialize(PlayerFacade facade)
        {
            _facade = facade;
            _playerInputData = new PlayerInputData();
            _playerInputData.Enable();
            SetupInputHandlers();

            _interactionMap = new Dictionary<string, InteractionType>()
            {
                { nameof(PlayerInputData.PlayerActions.InteractPrimary), InteractionType.Primary },
                { nameof(PlayerInputData.PlayerActions.InteractSecondary), InteractionType.Secondary },
                { nameof(PlayerInputData.PlayerActions.DropItem), InteractionType.DropItem }
            };
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

            _playerInputData.Player.InteractPrimary.performed += OnInteractPerformed;
            _playerInputData.Player.InteractSecondary.performed += OnInteractPerformed;
            _playerInputData.Player.DropItem.performed += OnInteractPerformed;

            _playerInputData.Player.Sprint.performed += OnSprintPerformed;
            _playerInputData.Player.Sprint.canceled += OnSprintCanceled;
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

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            if (_interactionMap.TryGetValue(context.action.name, out var interactionType))
            {
                if (_isDebug)
                {
                    Debug.Log(interactionType);
                }
                
                OnInteractPressed?.Invoke(interactionType);
            }
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