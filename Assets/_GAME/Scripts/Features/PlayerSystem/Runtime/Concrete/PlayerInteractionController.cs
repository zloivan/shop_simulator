using Sim.Features.InteractionSystem.Base;
using UnityEngine;

namespace Sim.Features.PlayerSystem.Concrete
{
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerInteractionController : MonoBehaviour, IInteractor
    {
        [Header("Настройки взаимодействия")]
        [SerializeField] private float _interactionDistance = 2.5f;

        [SerializeField] private LayerMask _interactionLayer = ~0; // Всё по умолчанию
        [SerializeField] private Transform _interactionRayOrigin;

        [Header("Отладка")]
        [SerializeField] private bool _showInteractionRays = true;

        [SerializeField] private Color _primaryRayColor = Color.blue;
        [SerializeField] private Color _secondaryRayColor = Color.red;

        private PlayerInputHandler _inputHandler;
        private Camera _playerCamera;

        // Реализация интерфейса IInteractor
        public Transform Transform => transform;
        public Camera PlayerCamera => _playerCamera;
        public float InteractionDistance => _interactionDistance;

        private void Awake()
        {
            _inputHandler = GetComponent<PlayerInputHandler>();
            _playerCamera = GetComponentInChildren<Camera>();

            if (_interactionRayOrigin == null)
            {
                _interactionRayOrigin = _playerCamera.transform;
            }
        }

        private void OnEnable()
        {
            // Подписка на события взаимодействия
            _inputHandler.OnInteractPrimaryPressed += HandleInteractPrimary;
            _inputHandler.OnInteractSecondaryPressed += HandleInteractSecondary;
        }

        private void OnDisable()
        {
            // Отписка от событий взаимодействия
            _inputHandler.OnInteractPrimaryPressed -= HandleInteractPrimary;
            _inputHandler.OnInteractSecondaryPressed -= HandleInteractSecondary;
        }

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

        private void TryInteractPrimary()
        {
            if (Physics.Raycast(_interactionRayOrigin.position, _interactionRayOrigin.forward, out RaycastHit hit,
                    _interactionDistance, _interactionLayer))
            {
                if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
                {
                    interactable.InteractPrimary(GetComponentInParent<PlayerFacade>());
                    Debug.Log("Первичное взаимодействие с: " + hit.collider.gameObject.name);
                }
            }
        }

        private void TryInteractSecondary()
        {
            if (Physics.Raycast(_interactionRayOrigin.position, _interactionRayOrigin.forward, out RaycastHit hit,
                    _interactionDistance, _interactionLayer))
            {
                if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
                {
                    interactable.InteractSecondary(GetComponentInParent<PlayerFacade>());
                    Debug.Log("Вторичное взаимодействие с: " + hit.collider.gameObject.name);
                }
            }
        }

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
    }
}