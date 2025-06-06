using IKhom.EventBusSystem.Runtime;
using JetBrains.Annotations;
using Sim.Features.PlayerSystem.Base;
using UnityEngine;

namespace Sim.Features.PlayerSystem.PlayerComponents
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovementController : MonoBehaviour, IPlayerComponent
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
        private Player _facade;

        // Состояние движения
        private Vector3 _verticalVelocity;
        private bool _wantsToJump;
        private bool _isMovementDisabled = false;

        // Публичные свойства для доступа через фасад
        [PublicAPI] public Vector3 MovementDirection { get; private set; }
        [PublicAPI] public bool IsGrounded { get; private set; }

        [PublicAPI] public bool IsRunning =>
            EventBus<PlayerEvents.PlayerMoveInput>.GetLastEvent().MoveInputValue.magnitude > 0.1f;
        [PublicAPI] public bool IsSprinting => 
            EventBus<PlayerEvents.PlayerSprintInput>.GetLastEvent().IsSprintPressed;
        [PublicAPI] public float CurrentSpeed { get; private set; }

        #region Unity Lifecycle

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            CurrentSpeed = _walkSpeed;
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        private void Update()
        {
            UpdateMovementSpeed();
            HandleMovement();
        }

        #endregion

        #region IPlayerComponent Implementation

        public void Initialize(Player facade)
        {
            _facade = facade;
            SubscribeToEvents();
        }

        #endregion

        #region Event Subscriptions

        private void SubscribeToEvents()
        {
            EventBus<PlayerEvents.PlayerJumpInput>.Register(
                new EventBinding<PlayerEvents.PlayerJumpInput>(HandleJumpPressed));
            
            EventBus<PlayerEvents.PlayerMovementDisabled>.Register(
                new EventBinding<PlayerEvents.PlayerMovementDisabled>(HandleMovementDisabled));
        }

        private void UnsubscribeFromEvents()
        {
            if (_facade == null) return;

            EventBus<PlayerEvents.PlayerJumpInput>.Deregister(
                new EventBinding<PlayerEvents.PlayerJumpInput>(HandleJumpPressed));
            
            EventBus<PlayerEvents.PlayerMovementDisabled>.Deregister(
                new EventBinding<PlayerEvents.PlayerMovementDisabled>(HandleMovementDisabled));
        }
        
        private void HandleMovementDisabled(PlayerEvents.PlayerMovementDisabled evt)
        {
            _isMovementDisabled = evt.IsDisabled;
        }

        #endregion

        #region Movement Logic

        private void UpdateMovementSpeed()
        {
            // Обновляем скорость в зависимости от состояния спринта и бега
            if (!IsGrounded)
                return;

            if (IsSprinting && IsRunning)
                CurrentSpeed = _sprintSpeed;
            else if (IsRunning)
                CurrentSpeed = _runSpeed;
            else
                CurrentSpeed = _walkSpeed;
        }

        private void HandleMovement()
        {
            if (_isMovementDisabled) return;
            
            IsGrounded = _characterController.isGrounded;

            if (IsGrounded && _verticalVelocity.y < 0)
            {
                _verticalVelocity.y = -2f; // Небольшое отрицательное значение вместо нуля
            }

            // Получаем данные о движении через фасад
            var moveInput = EventBus<PlayerEvents.PlayerMoveInput>.GetLastEvent().MoveInputValue;

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

        #endregion

        #region Input Handlers

        private void HandleJumpPressed(PlayerEvents.PlayerJumpInput playerJumpInput)
        {
            if (playerJumpInput.IsJumpPressed && IsGrounded)
            {
                _wantsToJump = true;
            }
        }

        #endregion
    }
}