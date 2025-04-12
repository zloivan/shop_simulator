using Sim.Features.InteractionSystem.Base;
using UnityEngine;

namespace Sim.Features.PlayerSystem.Concrete
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInputHandler))]
    [RequireComponent(typeof(PlayerMovementController))]
    [RequireComponent(typeof(PlayerLookController))]
    [RequireComponent(typeof(PlayerInteractionController))]
    public class FPSControllerNew : MonoBehaviour, IInteractor
    {
        [Header("Ссылки на компоненты")]
        [SerializeField] private Transform _cameraHolder;

        // Компоненты системы
        private PlayerInputHandler _inputHandler;
        private PlayerMovementController _movementController;
        private PlayerLookController _lookController;
        private PlayerInteractionController _interactionController;

        // Реализация интерфейса IInteractor (делегирование)
        public Transform Transform => _interactionController.Transform;
        public Camera PlayerCamera => _interactionController.PlayerCamera;
        public float InteractionDistance => _interactionController.InteractionDistance;

        private void Awake()
        {
            // Получаем компоненты
            _inputHandler = GetComponent<PlayerInputHandler>();
            _movementController = GetComponent<PlayerMovementController>();
            _lookController = GetComponent<PlayerLookController>();
            _interactionController = GetComponent<PlayerInteractionController>();

            // Убедимся, что камера настроена
            if (_cameraHolder == null)
            {
                Debug.LogError("Camera holder not assigned in FPSController!");
            }
        }

        // Публичные методы для внешнего доступа (сохранены для обратной совместимости)
        public Vector3 GetMovementDirection() => _movementController.MovementDirection;
        public bool IsGrounded() => _movementController.IsGrounded;
        public bool IsRunning() => _movementController.IsRunning;
        public bool IsSprinting() => _movementController.IsSprinting;
        public float GetCurrentSpeed() => _movementController.CurrentSpeed;
    }
}