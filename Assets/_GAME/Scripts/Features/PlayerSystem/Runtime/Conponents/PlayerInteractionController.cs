using Sim.Features.InteractionSystem.Base;
using Sim.Features.PlayerSystem.Base;
using UnityEngine;

namespace Sim.Features.PlayerSystem.Conponents
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

        // Публичные свойства для доступа через фасад
        public float InteractionDistance => _interactionDistance;

        #region Unity Lifecycle

        private void Awake()
        {
            _playerCamera = _facade.PlayerCamera;
            
            if (_interactionRayOrigin == null)
            {
                _interactionRayOrigin = _playerCamera.transform;
            }
        }

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
        }

        #endregion

        #region Event Subscriptions

        private void SubscribeToEvents()
        {
            // Подписываемся на события фасада вместо прямого обращения к другим компонентам
            _facade.OnInteractPrimaryPressed += HandleInteractPrimary;
            _facade.OnInteractSecondaryPressed += HandleInteractSecondary;
        }

        private void UnsubscribeFromEvents()
        {
            if (_facade == null) return;

            _facade.OnInteractPrimaryPressed -= HandleInteractPrimary;
            _facade.OnInteractSecondaryPressed -= HandleInteractSecondary;
        }

        #endregion

        #region Interaction Logic

        private void HandleInteractPrimary()
        {
            Debug.Log("Первичное взаимодействие");
            TryInteractPrimary();
        }

        private void HandleInteractSecondary()
        {
            Debug.Log("Вторичное взаимодействие");
            TryInteractSecondary();
        }

        // Публично доступный метод для использования через фасад
        public void TryInteractPrimary()
        {
            if (!Physics.Raycast(_interactionRayOrigin.position, _interactionRayOrigin.forward, out var hit,
                    _interactionDistance, _interactionLayer)) return;
            
            if (!hit.collider.TryGetComponent<IInteractable>(out var interactable)) 
                return;
            
            // Взаимодействуем через фасад
            interactable.InteractPrimary(_facade);
            Debug.Log("Первичное взаимодействие с: " + hit.collider.gameObject.name);
        }

        // Публично доступный метод для использования через фасад
        public void TryInteractSecondary()
        {
            if (!Physics.Raycast(_interactionRayOrigin.position, _interactionRayOrigin.forward, out var hit,
                    _interactionDistance, _interactionLayer)) 
                return;
            
            if (!hit.collider.TryGetComponent<IInteractable>(out var interactable)) 
                return;
            
            // Взаимодействуем через фасад
            interactable.InteractSecondary(_facade);
            Debug.Log("Вторичное взаимодействие с: " + hit.collider.gameObject.name);
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