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
                // Поворачиваем в локальном пространстве относительно опорной точки
                Quaternion invRefRotation = Quaternion.Inverse(_referenceRotation);
                Quaternion localRotation = invRefRotation * transform.rotation;
    
                // Извлекаем углы Эйлера из локального вращения
                Vector3 localEulerAngles = localRotation.eulerAngles;
    
                // Нормализуем углы в диапазон -180..180
                float localYaw = localEulerAngles.y > 180 ? localEulerAngles.y - 360 : localEulerAngles.y;
    
                // Применяем ограничения к локальным углам
                float newLocalYaw = Mathf.Clamp(localYaw + mouseX, _restrictedLookXMin, _restrictedLookXMax);
    
                // Создаем новое локальное вращение и преобразуем обратно в глобальное
                Quaternion newLocalRotation = Quaternion.Euler(0, newLocalYaw, 0);
                transform.rotation = _referenceRotation * newLocalRotation;
    
                // Обрабатываем вертикальное вращение камеры
                _rotationX = Mathf.Clamp(_rotationX - mouseY, _restrictedLookYMin, _restrictedLookYMax);
                _cameraHolder.localRotation = Quaternion.Euler(_rotationX, 0, 0);
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
        private Quaternion _referenceRotation;

        public void SetLookRestrictions(float xMin, float xMax, float yMin, float yMax, Transform referenceTransform = null)
        {
            _hasLookRestrictions = true;
            _restrictedLookXMin = xMin;
            _restrictedLookXMax = xMax;
            _restrictedLookYMin = yMin;
            _restrictedLookYMax = yMax;
    
            // Если задан опорный трансформ, используем его поворот как базовый
            if (referenceTransform != null)
                _referenceRotation = referenceTransform.rotation;
            else
                _referenceRotation = Quaternion.identity;
        }
        
        public void ClearLookRestrictions()
        {
            _hasLookRestrictions = false;
        }
    }
}