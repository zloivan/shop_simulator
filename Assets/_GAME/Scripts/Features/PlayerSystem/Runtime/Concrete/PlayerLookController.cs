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

        private void Update()
        {
            UpdateLook();
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
            _facade.OnLookInputChanged += HandleLookInput;
        }

        private void UnsubscribeFromEvents()
        {
            if (_facade == null) return;

            _facade.OnLookInputChanged -= HandleLookInput;
        }

        #endregion

        #region Look Logic

        private void HandleLookInput(Vector2 lookInput)
        {
            // Данный метод вызывается при изменении ввода взгляда
            // Здесь можно добавить дополнительную логику обработки, если необходимо
        }

        private void UpdateLook()
        {
            // Применяем сглаживание к вводу взгляда
            _smoothLookInput = Vector2.SmoothDamp(
                _smoothLookInput,
                _facade.LookInput,
                ref _lookInputVelocity,
                _lookSmoothing
            );

            // Расчет вращения камеры с учетом чувствительности
            float mouseX = _smoothLookInput.x * _lookSensitivity;
            float mouseY = _smoothLookInput.y * _lookSensitivity;

            // Вращение по вертикали (наклон камеры)
            _rotationX -= mouseY;
            _rotationX = Mathf.Clamp(_rotationX, -_lookXLimit, _lookXLimit);
            _cameraHolder.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);

            // Вращение по горизонтали (поворот игрока)
            transform.rotation *= Quaternion.Euler(0f, mouseX, 0f);
        }

        #endregion

        #region Public Methods

        // Метод для внешнего доступа к текущему углу обзора по вертикали
        public float GetVerticalAngle()
        {
            return _rotationX;
        }

        #endregion
    }
}