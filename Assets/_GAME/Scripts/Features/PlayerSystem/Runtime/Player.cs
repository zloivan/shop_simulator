using Sim.Features.InteractionSystem.Base;
using Sim.Features.PlayerSystem.Base;
using Sim.Features.PlayerSystem.PlayerComponents;
using UnityEngine;

namespace Sim.Features.PlayerSystem
{
    [RequireComponent(typeof(PlayerLookController))]
    [RequireComponent(typeof(PlayerInteractionController))]
    [RequireComponent(typeof(PlayerInventoryComponent))]
    [RequireComponent(typeof(PlayerHandsController))]
    public class Player : MonoBehaviour, IInteractor
    {
        [Header("Настройки игрока")]
        [SerializeField] private Transform _cameraHolder;

        private PlayerInteractionController _interactionController;
        public PlayerInventoryComponent Inventory { get; private set; }
        public PlayerHandsController HandsController { get; private set; }
        public PlayerLookController LookController { get; private set; }
        public float InteractionDistance => _interactionController.InteractionDistance;

        private void Awake()
        {
            // Получаем все необходимые компоненты
            _interactionController = GetComponent<PlayerInteractionController>();
            Inventory = GetComponent<PlayerInventoryComponent>();
            HandsController = GetComponent<PlayerHandsController>();
            LookController = GetComponent<PlayerLookController>();

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