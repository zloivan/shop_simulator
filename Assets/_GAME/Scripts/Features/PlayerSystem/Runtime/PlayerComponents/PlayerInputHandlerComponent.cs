using System;
using System.Collections.Generic;
using IKhom.EventBusSystem.Runtime;
using Sim.Features.InteractionSystem.Base;
using Sim.Features.PlayerSystem.Base;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sim.Features.PlayerSystem.PlayerComponents
{
    public class PlayerInputHandlerComponent : MonoBehaviour, IPlayerComponent
    {
        [SerializeField] private bool _isDebug;

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

        public void Initialize(Player facade)
        {
            _playerInputData = new PlayerInputData();
            _playerInputData.Enable();
            SetupInputHandlers();

            _interactionMap = new Dictionary<string, InteractionType>()
            {
                { nameof(PlayerInputData.PlayerActions.InteractPrimary), InteractionType.Primary },
                { nameof(PlayerInputData.PlayerActions.InteractSecondary), InteractionType.Secondary },
                { nameof(PlayerInputData.PlayerActions.DropItem), InteractionType.DropItem }
            };

            ;
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
            var moveInput = context.ReadValue<Vector2>();

            EventBus<PlayerEvents.PlayerMoveInput>.Raise(new PlayerEvents.PlayerMoveInput(moveInput));
        }

        private void OnLookInput(InputAction.CallbackContext context)
        {
            var lookInput = context.ReadValue<Vector2>();
            EventBus<PlayerEvents.PlayerLookInput>.Raise(new PlayerEvents.PlayerLookInput(lookInput));
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            EventBus<PlayerEvents.PlayerJumpInput>.Raise(new PlayerEvents.PlayerJumpInput(true));
        }

        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            EventBus<PlayerEvents.PlayerJumpInput>.Raise(new PlayerEvents.PlayerJumpInput(false));
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            if (!_interactionMap.TryGetValue(context.action.name, out var interactionType))
                return;

            if (_isDebug)
            {
                Debug.Log(interactionType);
            }

            EventBus<PlayerEvents.PlayerInteractInput>.Raise(new PlayerEvents.PlayerInteractInput(interactionType));
        }

        private void OnSprintPerformed(InputAction.CallbackContext context)
        {
            EventBus<PlayerEvents.PlayerSprintInput>.Raise(new PlayerEvents.PlayerSprintInput(true));
        }

        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            EventBus<PlayerEvents.PlayerSprintInput>.Raise(new PlayerEvents.PlayerSprintInput(false));
        }

        #endregion
    }
}