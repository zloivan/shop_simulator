using UnityEngine;

namespace Sim.Features.PlayerSystem.Concrete
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerMovementController : MonoBehaviour
    {
        [Header("Настройки скорости")]
        [SerializeField] private float _walkSpeed = 5f;

        [SerializeField] private float _runSpeed = 7f;
        [SerializeField] private float _sprintSpeed = 10f;

        [Header("Настройки физики")]
        [SerializeField] private float _jumpForce = 5f;

        [SerializeField] private float _gravity = -19.62f;
        [SerializeField] private float _airControl = 0.5f;

        private CharacterController _characterController;
        private PlayerInputHandler _inputHandler;
        private Transform _cameraTransform;

        // Состояние движения
        private Vector3 _verticalVelocity;
        private float _currentSpeed;
        private bool _isGrounded;
        private bool _wantsToJump;

        // Публичные свойства для внешнего доступа
        public Vector3 MovementDirection { get; private set; }
        public bool IsGrounded => _isGrounded;
        public bool IsRunning => _inputHandler.IsRunning;
        public bool IsSprinting => _inputHandler.IsSprintPressed;
        public float CurrentSpeed => _currentSpeed;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _inputHandler = GetComponent<PlayerInputHandler>();

            // Получаем трансформ камеры (предполагается, что он будет установлен в FPSController)
            _cameraTransform = Camera.main.transform.parent;

            _currentSpeed = _walkSpeed;
        }

        private void OnEnable()
        {
            // Подписка на события ввода
            _inputHandler.OnJumpPressed += HandleJumpPressed;
            _inputHandler.OnSprintPressed += HandleSprintPressed;
            _inputHandler.OnSprintReleased += HandleSprintReleased;
        }

        private void OnDisable()
        {
            // Отписка от событий ввода
            _inputHandler.OnJumpPressed -= HandleJumpPressed;
            _inputHandler.OnSprintPressed -= HandleSprintPressed;
            _inputHandler.OnSprintReleased -= HandleSprintReleased;
        }

        private void Update()
        {
            UpdateMovementSpeed();
            HandleMovement();
        }

        private void UpdateMovementSpeed()
        {
            // Обновляем скорость в зависимости от состояния спринта и бега
            if (_isGrounded)
            {
                if (_inputHandler.IsSprintPressed && _inputHandler.IsRunning)
                    _currentSpeed = _sprintSpeed;
                else if (_inputHandler.IsRunning)
                    _currentSpeed = _runSpeed;
                else
                    _currentSpeed = _walkSpeed;
            }
        }

        private void HandleMovement()
        {
            _isGrounded = _characterController.isGrounded;

            if (_isGrounded && _verticalVelocity.y < 0)
            {
                _verticalVelocity.y = -2f; // Небольшое отрицательное значение вместо нуля
            }

            // Получаем данные о движении от обработчика ввода
            Vector2 moveInput = _inputHandler.MoveInput;

            // Расчет направления движения относительно ориентации камеры
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            move = Vector3.ClampMagnitude(move,
                1f); // Нормализация вектора для предотвращения более быстрого диагонального движения
            MovementDirection = move;

            // Применение уменьшения управления в воздухе
            if (!_isGrounded)
            {
                move *= _airControl;
            }

            // Применение движения с текущей скоростью
            _characterController.Move(move * _currentSpeed * Time.deltaTime);

            // Обработка прыжка
            if (_wantsToJump && _isGrounded)
            {
                _verticalVelocity.y = Mathf.Sqrt(_jumpForce * -2f * _gravity);
                _wantsToJump = false; // Сброс для предотвращения непрерывных прыжков
            }

            // Применение гравитации
            _verticalVelocity.y += _gravity * Time.deltaTime;
            _characterController.Move(_verticalVelocity * Time.deltaTime);
        }

        private void HandleJumpPressed()
        {
            _wantsToJump = true;
        }

        private void HandleSprintPressed()
        {
            // Логика при начале спринта может быть расширена здесь
        }

        private void HandleSprintReleased()
        {
            // Логика при окончании спринта может быть расширена здесь
        }
    }
}