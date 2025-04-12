using UnityEngine;

namespace Sim.Features.PlayerSystem.Concrete
{
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerLookController : MonoBehaviour
    {
        [Header("Настройки камеры")]
        [SerializeField] private Transform _cameraHolder;
        [SerializeField] private float _lookSensitivity = 1f;
        [SerializeField] private float _lookSmoothing = 0.1f;
        [SerializeField] private float _lookXLimit = 80f;

        private PlayerInputHandler _inputHandler;
        private float _rotationX = 0f;

        // Значения для сглаживания
        private Vector2 _smoothLookInput;
        private Vector2 _lookInputVelocity;

        private void Awake()
        {
            _inputHandler = GetComponent<PlayerInputHandler>();

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
            _inputHandler.OnLookInputChanged += HandleLookInput;
        }

        private void OnDisable()
        {
            _inputHandler.OnLookInputChanged -= HandleLookInput;
        }

        private void Update()
        {
            UpdateLook();
        }

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
                _inputHandler.LookInput,
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

        // Метод для внешнего доступа к текущему углу обзора по вертикали
        public float GetVerticalAngle()
        {
            return _rotationX;
        }
    }
}