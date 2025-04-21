using Sim.Features.PlayerSystem.Base;
using UnityEngine;

namespace Sim.Features.PlayerSystem.Concrete
{
    public class PlayerLookController : MonoBehaviour, IPlayerComponent
    {
        [Header("Настройки камеры")]
        [SerializeField] private Transform _cameraHolder;

        [SerializeField] private float _lookSensitivity = 1f;
        [SerializeField] private float _lookSmoothing = 0.1f;
        [SerializeField] private float _lookXLimit = 80f;

        private PlayerFacade _facade;
        private float _rotationX = 0f;

        // Значения для сглаживания
        private Vector2 _smoothLookInput;
        private Vector2 _lookInputVelocity;

        #region Unity Lifecycle

        private void Awake()
        {
            if (_cameraHolder == null)
            {
                Debug.LogError("Camera holder is not assigned in PlayerLookController!");
            }

            // Блокировка и скрытие курсора
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            UpdateLook();
        }

        #endregion

        #region IPlayerComponent Implementation
        public void Initialize(PlayerFacade facade)
        {
            _facade = facade;
        }

        #endregion

        #region Look Logic
        
        private void UpdateLook()
        {
            _smoothLookInput = Vector2.SmoothDamp(
                _smoothLookInput,
                _facade.LookInput,
                ref _lookInputVelocity,
                _lookSmoothing
            );

            var mouseX = _smoothLookInput.x * _lookSensitivity;
            var mouseY = _smoothLookInput.y * _lookSensitivity;

            _rotationX -= mouseY;
            _rotationX = Mathf.Clamp(_rotationX, -_lookXLimit, _lookXLimit);
            _cameraHolder.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);

            transform.rotation *= Quaternion.Euler(0f, mouseX, 0f);
        }

        #endregion
    }
}