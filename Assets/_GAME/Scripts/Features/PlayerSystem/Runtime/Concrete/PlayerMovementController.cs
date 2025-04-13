using JetBrains.Annotations;
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

        // Состояние движения
        private Vector3 _verticalVelocity;
        private bool _wantsToJump;

        // Публичные свойства для внешнего доступа
        [PublicAPI] public Vector3 MovementDirection { get; private set; }
        [PublicAPI] public bool IsGrounded { get; private set; }
        [PublicAPI] public bool IsRunning => _inputHandler.IsRunning;
        [PublicAPI] public bool IsSprinting => _inputHandler.IsSprintPressed;
        [PublicAPI] public float CurrentSpeed { get; private set; }

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _inputHandler = GetComponent<PlayerInputHandler>();


            CurrentSpeed = _walkSpeed;
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
            if (!IsGrounded)
                return;

            if (_inputHandler.IsSprintPressed && _inputHandler.IsRunning)
                CurrentSpeed = _sprintSpeed;
            else if (_inputHandler.IsRunning)
                CurrentSpeed = _runSpeed;
            else
                CurrentSpeed = _walkSpeed;
        }

        private void HandleMovement()
        {
            IsGrounded = _characterController.isGrounded;

            if (IsGrounded && _verticalVelocity.y < 0)
            {
                _verticalVelocity.y = -2f; // Небольшое отрицательное значение вместо нуля
            }

            // Получаем данные о движении от обработчика ввода
            var moveInput = _inputHandler.MoveInput;

            // Расчет направления движения относительно ориентации камеры
            var move = transform.right * moveInput.x + transform.forward * moveInput.y;
            move = Vector3.ClampMagnitude(move,
                1f); // Нормализация вектора для предотвращения более быстрого диагонального движения
            MovementDirection = move;

            // Применение уменьшения управления в воздухе
            if (!IsGrounded)
            {
                move *= _airControl;
            }

            // Применение движения с текущей скоростью
            _characterController.Move(move * CurrentSpeed * Time.deltaTime);

            // Обработка прыжка
            if (_wantsToJump && IsGrounded)
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
            Debug.Log("Sprinting");
        }

        private void HandleSprintReleased()
        {
            // Логика при окончании спринта может быть расширена здесь
            Debug.Log("Sprinting Stopped");
        }
    }
}