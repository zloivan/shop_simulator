using System;
using System.Collections.Generic;
using Sim.Features.InteractionSystem.Base;
using Sim.Features.InventorySystem.Base;
using Sim.Features.PlayerSystem.Base;
using UnityEngine;

namespace Sim.Features.PlayerSystem.Concrete
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInputHandler))]
    [RequireComponent(typeof(PlayerMovementController))]
    [RequireComponent(typeof(PlayerLookController))]
    [RequireComponent(typeof(PlayerInteractionController))]
    [RequireComponent(typeof(PlayerInventoryComponent))]
    public class PlayerFacade : MonoBehaviour, IInteractor
    {
        [Header("Настройки игрока")]
        [SerializeField] private Transform _cameraHolder;

        // Скрытые ссылки на компоненты (недоступны извне)
        private PlayerInputHandler _inputHandler;
        private PlayerMovementController _movementController;
        private PlayerLookController _lookController;
        private PlayerInteractionController _interactionController;
        private PlayerInventoryComponent _inventoryComponent;
        private Camera _playerCamera;

        #region События (перенаправляются от компонентов)

        // События ввода
        public event Action<Vector2> OnMoveInputChanged;
        public event Action<Vector2> OnLookInputChanged;
        public event Action OnJumpPressed;
        public event Action OnJumpReleased;
        public event Action OnInteractPrimaryPressed;
        public event Action OnInteractSecondaryPressed;
        public event Action OnSprintPressed;
        public event Action OnSprintReleased;

        // События инвентаря
        public event Action<string> OnItemAddedToInventory;
        public event Action<string> OnItemRemovedFromInventory;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Получаем все необходимые компоненты
            _inputHandler = GetComponent<PlayerInputHandler>();
            _movementController = GetComponent<PlayerMovementController>();
            _lookController = GetComponent<PlayerLookController>();
            _interactionController = GetComponent<PlayerInteractionController>();
            _inventoryComponent = GetComponent<PlayerInventoryComponent>();
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
            _inputHandler.OnMoveInputChanged += input => OnMoveInputChanged?.Invoke(input);
            _inputHandler.OnLookInputChanged += input => OnLookInputChanged?.Invoke(input);
            _inputHandler.OnJumpPressed += () => OnJumpPressed?.Invoke();
            _inputHandler.OnJumpReleased += () => OnJumpReleased?.Invoke();
            _inputHandler.OnInteractPrimaryPressed += () => OnInteractPrimaryPressed?.Invoke();
            _inputHandler.OnInteractSecondaryPressed += () => OnInteractSecondaryPressed?.Invoke();
            _inputHandler.OnSprintPressed += () => OnSprintPressed?.Invoke();
            _inputHandler.OnSprintReleased += () => OnSprintReleased?.Invoke();
            
            // События инвентаря (если нужны)
            _inventoryComponent.OnItemAdded += itemId => OnItemAddedToInventory?.Invoke(itemId);
            _inventoryComponent.OnItemRemoved += itemId => OnItemRemovedFromInventory?.Invoke(itemId);
        }

        #endregion

        #region IInteractor Implementation

        // Реализация IInteractor (для системы взаимодействия)
        public Transform Transform => transform;
        public Camera PlayerCamera => _playerCamera;
        public float InteractionDistance => _interactionController.InteractionDistance;

        #endregion

        #region Public API - Input Properties

        // Публичное API для доступа к состоянию ввода
        public Vector2 MoveInput => _inputHandler.MoveInput;
        public Vector2 LookInput => _inputHandler.LookInput;
        public bool IsJumpPressed => _inputHandler.IsJumpPressed;
        public bool IsSprintPressed => _inputHandler.IsSprintPressed;
        public bool IsRunning => _inputHandler.IsRunning;

        #endregion

        #region Public API - Movement Properties

        // Публичное API для доступа к состоянию движения
        public Vector3 MovementDirection => _movementController.MovementDirection;
        public bool IsGrounded => _movementController.IsGrounded;
        public bool IsSprinting => _movementController.IsSprinting;
        public float CurrentSpeed => _movementController.CurrentSpeed;

        #endregion

        #region Public API - Inventory Properties

        // Публичное API для доступа к инвентарю
        public IReadOnlyList<IInventoryItem> InventoryItems => _inventoryComponent.Inventory.Items;
        public float InventoryCurrentWeight => _inventoryComponent.Inventory.CurrentWeight;
        public float InventoryMaxWeight => _inventoryComponent.Inventory.MaxWeight;
        public int InventoryCapacity => _inventoryComponent.Inventory.Capacity;

        #endregion

        #region Public API - Methods

        // Методы взаимодействия с системой ввода
        public void SimulateJump() => OnJumpPressed?.Invoke();
        public void SimulateInteractPrimary() => OnInteractPrimaryPressed?.Invoke();
        public void SimulateInteractSecondary() => OnInteractSecondaryPressed?.Invoke();
        
        public void SimulateSprint(bool active)
        {
            if (active)
                OnSprintPressed?.Invoke();
            else
                OnSprintReleased?.Invoke();
        }

        // Методы взаимодействия с инвентарем
        public bool AddItemToInventory(IInventoryItem item) => _inventoryComponent.Inventory.AddItem(item);
        public bool RemoveItemFromInventory(string itemId) => _inventoryComponent.Inventory.RemoveItem(itemId);
        public bool HasItemInInventory(string itemId) => _inventoryComponent.Inventory.HasItem(itemId);
        public IInventoryItem GetItemFromInventory(string itemId) => _inventoryComponent.Inventory.GetItem(itemId);

        // Методы взаимодействия с внешними объектами
        public void TryInteractWithTarget() => _interactionController.TryInteractPrimary();
        public void TryExamineTarget() => _interactionController.TryInteractSecondary();

        #endregion
    }
}