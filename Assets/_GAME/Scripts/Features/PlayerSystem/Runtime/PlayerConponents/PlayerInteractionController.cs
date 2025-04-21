using Sim.Features.InteractionSystem.Base;
using Sim.Features.PlayerSystem.Base;
using UnityEngine;

namespace Sim.Features.PlayerSystem.PlayerConponents
{
    public class PlayerInteractionController : MonoBehaviour, IPlayerComponent
    {
        [Header("Настройки взаимодействия")]
        [SerializeField] private float _interactionDistance = 2.5f;

        [SerializeField] private LayerMask _interactionLayer = ~0; // Всё по умолчанию
        [SerializeField] private Transform _interactionRayOrigin;

        [Header("Отладка")]
        [SerializeField] private bool _showInteractionRays = true;

        [SerializeField] private Color _primaryRayColor = Color.blue;
        [SerializeField] private Color _secondaryRayColor = Color.red;

        private PlayerFacade _facade;
        private Camera _playerCamera;

        private InteractableBase _interactableBase;

        // Публичные свойства для доступа через фасад
        public float InteractionDistance => _interactionDistance;

        #region Unity Lifecycle

        private void OnEnable()
        {
            if (_facade != null)
            {
                SubscribeToEvents();
            }
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        #endregion

        #region IPlayerComponent Implementation

        public void Initialize(PlayerFacade facade)
        {
            _facade = facade;
            SubscribeToEvents();

            _playerCamera = _facade.PlayerCamera;

            if (_interactionRayOrigin == null)
            {
                _interactionRayOrigin = _playerCamera.transform;
            }
        }

        #endregion

        #region Event Subscriptions

        private void SubscribeToEvents()
        {
            // Подписываемся на события фасада вместо прямого обращения к другим компонентам
            _facade.OnInteractPressed += HandleInteractPressed;
        }

        private void UnsubscribeFromEvents()
        {
            if (_facade == null) return;

            _facade.OnInteractPressed -= HandleInteractPressed;
        }

        #endregion

        #region Interaction Logic

        private void HandleInteractPressed(InteractionType callbackContext)
        {
            TryInteract(callbackContext);
        }

        // Публично доступный метод для использования через фасад
        private bool TryInteract(InteractionType interactionType)
        {
            if (!Physics.Raycast(_interactionRayOrigin.position, _interactionRayOrigin.forward, out var hit,
                    _interactionDistance, _interactionLayer)) return false;

            var interactables = hit.collider.GetComponents<InteractableBase>();
            foreach (var interactable in interactables)
            {
                interactable.Interact(_facade, interactionType);
            }

            return true;
        }

        private void Update()
        {
            if (!Physics.Raycast(_interactionRayOrigin.position, _interactionRayOrigin.forward, out var hit,
                    _interactionDistance, _interactionLayer))
            {
                if (_interactableBase is not null)
                {
                    _interactableBase.CanInteract = false;
                }

                return;
            }


            if (!hit.collider.TryGetComponent<InteractableBase>(out var interactable))
            {
                if (_interactableBase is not null)
                {
                    _interactableBase.CanInteract = false;
                }

                return;
            }


            if (_interactableBase is not null)
            {
                _interactableBase.CanInteract = false;
            }

            _interactableBase = interactable;
            _interactableBase.CanInteract = true;
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmos()
        {
            if (!_showInteractionRays || _interactionRayOrigin == null) return;

            // Рисуем луч первичного взаимодействия
            Gizmos.color = _primaryRayColor;
            Gizmos.DrawRay(_interactionRayOrigin.position, _interactionRayOrigin.forward * _interactionDistance);

            // Рисуем луч вторичного взаимодействия (может иметь другие параметры)
            Gizmos.color = _secondaryRayColor;
            Gizmos.DrawRay(_interactionRayOrigin.position, _interactionRayOrigin.forward * _interactionDistance);
        }

        #endregion
    }
}