using System;
using IKhom.EventBusSystem.Runtime;
using Sim.Features.InteractionSystem.Base;
using Sim.Features.PlayerSystem;
using Sim.Features.PlayerSystem.PlayerComponents;
using UnityEngine;

namespace Sim.Features.ConveyorBeltSystem
{
    public class ConveyorBelt : InteractableBase
    {
        [SerializeField] private Transform _cameraPosition;
        [SerializeField] private float _lookXLimit = 30f; // Ограничение по горизонтали в градусах
        [SerializeField] private float _lookYLimit = 20f; // Ограничение по вертикали в градусах

        private Player _player;
        private Transform _originalCameraTransform;
        private bool _isPlayerAtBelt;
        private PlayerLookController _lookController;
        private Vector3 _originalPlayerPosition;
        private Quaternion _originalPlayerRotation;

        public override void InteractInternal(IInteractor playerFacade)
        {
            if (_isPlayerAtBelt) return;

            _player = playerFacade as Player;
            if (_player == null) return;

            _lookController = _player.LookController;
            if (_lookController == null) return;

            // Сохраняем оригинальную позицию и поворот игрока
            _originalPlayerPosition = _player.transform.position;
            _originalPlayerRotation = _player.transform.rotation;

            // Переключаем игрока в режим работы за кассой
            EnterCashierMode();

            EventBus<PlayerEvents.PlayerInteractInput>.Register(
                new EventBinding<PlayerEvents.PlayerInteractInput>(HandleInteractInput));
            
            Debug.Log("Игрок встал за кассу");
        }

        private void EnterCashierMode()
        {
            _isPlayerAtBelt = true;
            InteractionsEnabled = false;
    
            // Отключаем передвижение игрока
            EventBus<PlayerEvents.PlayerMovementDisabled>.Raise(new PlayerEvents.PlayerMovementDisabled(true));
    
            // Перемещаем игрока к кассе
            _player.transform.position = _cameraPosition.position;
            _player.transform.rotation = _cameraPosition.rotation;

            // Устанавливаем ограничения поворота камеры, используя _cameraPosition как опорную точку
            _lookController.SetLookRestrictions(
                -_lookXLimit, 
                _lookXLimit, 
                -_lookYLimit, 
                _lookYLimit, 
                _cameraPosition); // Передаем опорный трансформ
        }

        private void HandleInteractInput(PlayerEvents.PlayerInteractInput interactInput)
        {
            if (interactInput.InteractionType == InteractionType.Cancel)
            {
                ExitCashierMode();
            }
        }

        private void ExitCashierMode()
        {
            if (!_isPlayerAtBelt) return;
            InteractionsEnabled = true;
            
            EventBus<PlayerEvents.PlayerInteractInput>.Deregister(
                new EventBinding<PlayerEvents.PlayerInteractInput>(HandleInteractInput));
            
            _isPlayerAtBelt = false;

            // Снимаем ограничения поворота камеры
            _lookController.ClearLookRestrictions();

            // Возвращаем игрока в исходную позицию
            _player.transform.position = _originalPlayerPosition;
            _player.transform.rotation = _originalPlayerRotation;

            // Включаем передвижение игрока
            EventBus<PlayerEvents.PlayerMovementDisabled>.Raise(new PlayerEvents.PlayerMovementDisabled(false));
        }
    }
}