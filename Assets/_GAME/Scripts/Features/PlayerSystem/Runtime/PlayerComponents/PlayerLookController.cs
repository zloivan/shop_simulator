using IKhom.EventBusSystem.Runtime;
using Sim.Features.PlayerSystem.Base;
using UnityEngine;

namespace Sim.Features.PlayerSystem.PlayerComponents
{
    public class PlayerLookController : MonoBehaviour, IPlayerComponent
    {
        [Header("Настройки камеры")]
        [SerializeField] private Transform _cameraHolder;
        [SerializeField] private float _lookSensitivity = 1f;
        [SerializeField] private float _lookSmoothing = 0.1f;
        [SerializeField] private float _lookXLimit = 80f;
        [SerializeField] private Camera _camera;
        
        private Player _facade;
        private float _rotationX = 0f;

        // Значения для сглаживания
        private Vector2 _smoothLookInput;
        private Vector2 _lookInputVelocity;
        // Добавляем новые поля для ограничений
        private bool _hasLookRestrictions;
        private float _restrictedLookXMin;
        private float _restrictedLookXMax;
        private float _restrictedLookYMin;
        private float _restrictedLookYMax;
        public Camera Camera => _camera;

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
        public void Initialize(Player facade)
        {
            _facade = facade;
        }

        #endregion

        #region Look Logic
        
        private void UpdateLook()
        {
            var lookInput = EventBus<PlayerEvents.PlayerLookInput>.GetLastEvent().LookInputValue;
            _smoothLookInput = Vector2.SmoothDamp(
                _smoothLookInput,
                lookInput,
                ref _lookInputVelocity,
                _lookSmoothing
            );

            var mouseX = _smoothLookInput.x * _lookSensitivity;
            var mouseY = _smoothLookInput.y * _lookSensitivity;

            _rotationX -= mouseY;
    
            // Применяем ограничения, если они установлены
            if (_hasLookRestrictions)
            {
                // Ограничиваем вертикальный угол
                _rotationX = Mathf.Clamp(_rotationX, _restrictedLookYMin, _restrictedLookYMax);
        
                // Применяем вертикальный угол к держателю камеры
                _cameraHolder.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);
        
                // Для горизонтального поворота требуется другой подход
                float currentYRotation = transform.eulerAngles.y;
                // Нормализуем угол
                if (currentYRotation > 180)
                    currentYRotation -= 360;
            
                float newYRotation = currentYRotation + mouseX;
                newYRotation = Mathf.Clamp(newYRotation, _restrictedLookXMin, _restrictedLookXMax);
        
                // Устанавливаем горизонтальный поворот
                transform.rotation = Quaternion.Euler(0f, newYRotation, 0f);
            }
            else
            {
                // Стандартное поведение без ограничений
                _rotationX = Mathf.Clamp(_rotationX, -_lookXLimit, _lookXLimit);
                _cameraHolder.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);
                transform.rotation *= Quaternion.Euler(0f, mouseX, 0f);
            }
        }

        #endregion
        
     
    
        // Публичные методы для установки и снятия ограничений
        public void SetLookRestrictions(float xMin, float xMax, float yMin, float yMax)
        {
            _hasLookRestrictions = true;
            _restrictedLookXMin = xMin;
            _restrictedLookXMax = xMax;
            _restrictedLookYMin = yMin;
            _restrictedLookYMax = yMax;
        }
        
        public void ClearLookRestrictions()
        {
            _hasLookRestrictions = false;
        }
    }
}