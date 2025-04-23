using Sim.Features.InteractionSystem.Base;
using Sim.Features.PlayerSystem.Base;
using Sim.Features.PlayerSystem.PlayerComponents;
using UnityEngine;

namespace Sim.Features.PlayerSystem
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInputHandlerComponent))]
    [RequireComponent(typeof(PlayerMovementController))]
    [RequireComponent(typeof(PlayerLookController))]
    [RequireComponent(typeof(PlayerInteractionController))]
    [RequireComponent(typeof(PlayerInventoryComponent))]
    [RequireComponent(typeof(PlayerHandsController))]
    public class Player : MonoBehaviour, IInteractor
    {
        [Header("Настройки игрока")]
        [SerializeField] private Transform _cameraHolder;

        private PlayerInputHandlerComponent _inputHandlerComponent;
        private PlayerInteractionController _interactionController;
        //private Camera _playerCamera;
        
        public PlayerInventoryComponent Inventory { get; private set; }
        public PlayerHandsController HandsController { get; private set; }
        public PlayerLookController LookController { get; private set; }
        public Transform Transform => transform;
        public float InteractionDistance => _interactionController.InteractionDistance;
        public bool IsSprintPressed => _inputHandlerComponent.IsSprintPressed;
       // public bool IsRunning => _inputHandlerComponent.IsRunning;
     

        private void Awake()
        {
            // Получаем все необходимые компоненты
            _inputHandlerComponent = GetComponent<PlayerInputHandlerComponent>();
            _interactionController = GetComponent<PlayerInteractionController>();
            Inventory = GetComponent<PlayerInventoryComponent>();
            HandsController = GetComponent<PlayerHandsController>();
            LookController = GetComponent<PlayerLookController>();
            //_playerCamera = GetComponentInChildren<Camera>();

            if (_cameraHolder == null)
            {
                Debug.LogError("Camera holder not assigned in PlayerFacade!");
            }

            // Связываем компоненты с фасадом
            InitializeComponents();
        }

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
    }
}