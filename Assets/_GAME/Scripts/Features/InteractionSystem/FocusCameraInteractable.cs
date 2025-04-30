using IKhom.EventBusSystem.Runtime;
using Sim.Features.InteractionSystem.Base;
using Sim.Features.PlayerSystem;
using Sim.Features.PlayerSystem.PlayerComponents;
using UnityEngine;

namespace Sim.Features.ConveyorBeltSystem
{
    public class FocusCameraInteractable : InteractableBase
    {
        [Header("Настройки фокусировки")]
        [SerializeField] private Transform _cameraFocusPoint;

        [Tooltip("Мировой UI, который будет активирован при взаимодействии")]
        [SerializeField] private GameObject _worldSpaceUI;

        private Player _player;
        private bool _isPlayerInFocusMode;

        // Сохраняем состояние игрока для восстановления
        private Vector3 _originalPlayerPosition;
        private Quaternion _originalPlayerRotation;
        private EventBinding<PlayerEvents.PlayerInteractInput> _interactInputBinding;

        protected override void Awake()
        {
            base.Awake();
            if (_worldSpaceUI != null)
            {
                _worldSpaceUI.SetActive(false);
            }

            if (_cameraFocusPoint == null)
            {
                _cameraFocusPoint = transform;
                Debug.LogWarning($"[{name}] Camera focus point not set, using transform as default");
            }
        }

        public override void InteractInternal(IInteractor playerFacade)
        {
            if (_isPlayerInFocusMode) return;

            _player = playerFacade as Player;
            if (_player == null) return;

            // Сохраняем оригинальную позицию и поворот игрока
            _originalPlayerPosition = _player.transform.position;
            _originalPlayerRotation = _player.transform.rotation;

            // Переключаем игрока в режим фокусировки
            EnterFocusMode();

            Debug.Log($"Игрок вошел в режим фокусировки на {gameObject.name}");
        }

        private void EnterFocusMode()
        {
            _isPlayerInFocusMode = true;
            InteractionsEnabled = false;

            // Отключаем передвижение игрока
            EventBus<PlayerEvents.PlayerMovementDisabled>.Raise(new PlayerEvents.PlayerMovementDisabled(true));

            // Перемещаем игрока к точке фокусировки
            _player.transform.position = _cameraFocusPoint.position;
            _player.transform.rotation = _cameraFocusPoint.rotation;

            // Фиксируем камеру через StateMachine в LookController
            _player.LookController.LockCamera(_cameraFocusPoint);

            // Подписываемся на нажатие Escape для выхода из режима фокусировки
            _interactInputBinding = new EventBinding<PlayerEvents.PlayerInteractInput>(HandleInteractInput);
            EventBus<PlayerEvents.PlayerInteractInput>.Register(_interactInputBinding);

            // Включаем курсор и делаем его видимым
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Активируем World Space UI, если он присутствует
            if (_worldSpaceUI != null)
            {
                _worldSpaceUI.SetActive(true);
            }
        }

        private void HandleInteractInput(PlayerEvents.PlayerInteractInput interactInput)
        {
            if (interactInput.InteractionType == InteractionType.Cancel)
            {
                ExitFocusMode();
            }
        }

        private void ExitFocusMode()
        {
            if (!_isPlayerInFocusMode) return;

            // Отключаем World Space UI
            if (_worldSpaceUI != null)
            {
                _worldSpaceUI.SetActive(false);
            }

            // Отписываемся от событий
            EventBus<PlayerEvents.PlayerInteractInput>.Deregister(_interactInputBinding);

            _isPlayerInFocusMode = false;
            InteractionsEnabled = true;

            // Скрываем курсор и блокируем его
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Возвращаем камеру в предыдущее состояние через StateMachine
            _player.LookController.ReturnToPreviousState();

            // Возвращаем игрока в исходную позицию
            _player.transform.position = _originalPlayerPosition;
            _player.transform.rotation = _originalPlayerRotation;

            // Включаем передвижение игрока
            EventBus<PlayerEvents.PlayerMovementDisabled>.Raise(new PlayerEvents.PlayerMovementDisabled(false));

            Debug.Log($"Игрок вышел из режима фокусировки на {gameObject.name}");
        }

        private void OnDestroy()
        {
            // Если объект уничтожается, когда игрок находится в режиме фокусировки,
            // нужно вернуть игрока в нормальное состояние
            if (_isPlayerInFocusMode)
            {
                ExitFocusMode();
            }
        }
    }
}