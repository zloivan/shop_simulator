using System;
using Sim.Features.PlayerSystem.Base;
using Sim.Features.PlayerSystem.Conponents;
using UnityEngine;

namespace Sim.Features.PlayerSystem
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInputHandlerComponent))]
    [RequireComponent(typeof(PlayerMovementController))]
    [RequireComponent(typeof(PlayerLookController))]
    [RequireComponent(typeof(PlayerInteractionController))]
    [RequireComponent(typeof(PlayerInventoryComponent))]
    public class PlayerFacade : MonoBehaviour
    {
        #region События (перенаправляются от компонентов)

        public event Action OnInteractPrimaryPressed;
        public event Action OnInteractSecondaryPressed;
        public event Action OnJumpPressed;

        #endregion

        [Header("Настройки игрока")]
        [SerializeField] private Transform _cameraHolder;

        // Скрытые ссылки на компоненты (недоступны извне)
        private PlayerInputHandlerComponent _inputHandlerComponent;
        private PlayerMovementController _movementController;
        private PlayerInteractionController _interactionController;
        private Camera _playerCamera;


        #region IInteractor Implementation

        public Transform Transform => transform;
        public Camera PlayerCamera => _playerCamera;
        public float InteractionDistance => _interactionController.InteractionDistance;

        #endregion

        #region Public API - Input Properties

        // Публичное API для доступа к состоянию ввода
        public Vector2 MoveInput => _inputHandlerComponent.MoveInput;
        public Vector2 LookInput => _inputHandlerComponent.LookInput;
        public bool IsSprintPressed => _inputHandlerComponent.IsSprintPressed;
        public bool IsRunning => _inputHandlerComponent.IsRunning;

        #endregion

        #region Public API - Movement Properties

        // Публичное API для доступа к состоянию движения
        public Vector3 MovementDirection => _movementController.MovementDirection;
        public bool IsGrounded => _movementController.IsGrounded;
        public bool IsSprinting => _movementController.IsSprinting;
        public float CurrentSpeed => _movementController.CurrentSpeed;
        public bool WantsToJump { get; set; }

        #endregion


        #region Unity Lifecycle

        private void Awake()
        {
            // Получаем все необходимые компоненты
            _inputHandlerComponent = GetComponent<PlayerInputHandlerComponent>();
            _movementController = GetComponent<PlayerMovementController>();
            _interactionController = GetComponent<PlayerInteractionController>();
            _playerCamera = GetComponentInChildren<Camera>();

            if (_cameraHolder == null)
            {
                Debug.LogError("Camera holder not assigned in PlayerFacade!");
            }

            // Связываем компоненты с фасадом
            InitializeComponents();

            // Связываем события компонентов с событиями фасада
            InitializeEvents();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Инициализирует все компоненты игрока
        /// </summary>
        private void InitializeComponents()
        {
            // Находим все компоненты, реализующие IPlayerComponent
            var components = GetComponents<IPlayerComponent>();
            foreach (var component in components)
            {
                component.Initialize(this);
            }
        }

        /// <summary>
        /// Связывает события компонентов с событиями фасада
        /// </summary>
        private void InitializeEvents()
        {
            // Перенаправляем события ввода через фасад
            _inputHandlerComponent.OnInteractPrimaryPressed += () => OnInteractPrimaryPressed?.Invoke();
            _inputHandlerComponent.OnInteractSecondaryPressed += () => OnInteractSecondaryPressed?.Invoke();
            _inputHandlerComponent.OnJumpPressed += () => OnJumpPressed?.Invoke();
        }

        #endregion
    }
}