using System.Threading;
using Cysharp.Threading.Tasks;
using IKhom.EventBusSystem.Runtime;
using IKhom.StateMachineSystem.Runtime;
using IKhom.StateMachineSystem.Runtime.abstractions;
using Sim.Features.PlayerSystem.Base;
using UnityEngine;

namespace Sim.Features.PlayerSystem.PlayerComponents
{
    /// <summary>
    /// Перечисление возможных состояний камеры для PlayerLookController
    /// </summary>
    public enum CameraState
    {
        /// <summary>
        /// Обычное состояние камеры от первого лица с полной свободой движения
        /// </summary>
        FirstPerson,

        /// <summary>
        /// Ограниченное состояние камеры с возможностью поворота в заданных пределах
        /// </summary>
        Restricted,

        /// <summary>
        /// Полностью заблокированная камера без возможности поворота
        /// </summary>
        Locked
    }

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

        // Данные для ограничений камеры
        private CameraRestriction _currentRestriction;

        // Данные для фиксированной камеры
        private Transform _lockFocusPoint;

        // State Machine
        private StateMachine<CameraState> _stateMachine;

        // Публичные свойства
        public Camera Camera => _camera;
        public CameraState CurrentState => _stateMachine.CurrentState;
        public Transform CameraHolder => _cameraHolder;

        public float RotationX
        {
            get => _rotationX;
            set => _rotationX = value;
        }

        public float LookSensitivity => _lookSensitivity;
        public float LookSmoothing => _lookSmoothing;
        public float LookXLimit => _lookXLimit;

        // Вспомогательный класс для хранения параметров ограничения
        public class CameraRestriction
        {
            public float XMin { get; }
            public float XMax { get; }
            public float YMin { get; }
            public float YMax { get; }
            public Transform ReferenceTransform { get; }
            public Quaternion ReferenceRotation { get; }

            public CameraRestriction(float xMin, float xMax, float yMin, float yMax, Transform reference = null)
            {
                XMin = xMin;
                XMax = xMax;
                YMin = yMin;
                YMax = yMax;
                ReferenceTransform = reference;
                ReferenceRotation = reference != null ? reference.rotation : Quaternion.identity;
            }
        }

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

            // Инициализация State Machine
            InitializeStateMachine();
        }

        private void Update()
        {
            _stateMachine.Update();
        }

        #endregion

        #region IPlayerComponent Implementation

        public void Initialize(Player facade)
        {
            _facade = facade;
        }

        #endregion

        #region State Machine Initialization

        private void InitializeStateMachine()
        {
            _stateMachine = new StateMachine<CameraState>();

            // Добавляем состояния
            _stateMachine.AddStateWithContext(CameraState.FirstPerson, context =>
                new FirstPersonCameraState(this, context));

            _stateMachine.AddStateWithContext(CameraState.Restricted, context =>
                new RestrictedCameraState(this, context));

            _stateMachine.AddStateWithContext(CameraState.Locked, context =>
                new LockedCameraState(this, context));

            // Настраиваем отслеживание изменений состояний
            _stateMachine.StateChanged += OnCameraStateChanged;

            // Устанавливаем начальное состояние
            _stateMachine.SetInitialState(CameraState.FirstPerson);
        }

        private void OnCameraStateChanged(CameraState oldState, CameraState newState)
        {
            Debug.Log($"Камера изменила состояние с {oldState} на {newState}");
        }

        #endregion

        #region Public Camera State Control Methods

        /// <summary>
        /// Переключает камеру в режим ограниченного обзора
        /// </summary>
        public void RestrictCamera(float xMin, float xMax, float yMin, float yMax, Transform reference = null)
        {
            _currentRestriction = new CameraRestriction(xMin, xMax, yMin, yMax, reference);
            _stateMachine.ChangeState(CameraState.Restricted);
        }

        /// <summary>
        /// Фиксирует камеру полностью в заданной точке
        /// </summary>
        public void LockCamera(Transform focusPoint)
        {
            _lockFocusPoint = focusPoint;
            _stateMachine.ChangeState(CameraState.Locked);
        }

        /// <summary>
        /// Возвращает камеру в режим от первого лица
        /// </summary>
        public void ReturnToFirstPerson()
        {
            _stateMachine.ChangeState(CameraState.FirstPerson);
        }

        /// <summary>
        /// Возвращает камеру к предыдущему состоянию
        /// </summary>
        public void ReturnToPreviousState()
        {
            _stateMachine.UndoLastTransition();
        }

        #endregion

        #region Internal State Access Methods

        // Методы для доступа состояний StateMachine к внутренним параметрам контроллера

        public Vector2 GetSmoothLookInput()
        {
            var lookInput = EventBus<PlayerEvents.PlayerLookInput>.GetLastEvent().LookInputValue;
            _smoothLookInput = Vector2.SmoothDamp(
                _smoothLookInput,
                lookInput,
                ref _lookInputVelocity,
                _lookSmoothing
            );

            return _smoothLookInput;
        }

        public CameraRestriction GetCurrentRestriction()
        {
            return _currentRestriction;
        }

        public Transform GetLockFocusPoint()
        {
            return _lockFocusPoint;
        }

        #endregion
    }

    /// <summary>
    /// Базовый класс для состояний камеры
    /// </summary>
    public abstract class BaseCameraState : IState<CameraState>
    {
        protected readonly PlayerLookController Controller;
        protected readonly IStateContext<CameraState> Context;

        protected BaseCameraState(PlayerLookController controller, IStateContext<CameraState> context)
        {
            Controller = controller;
            Context = context;
        }

        public virtual UniTask EnterAsync(CancellationToken cancellationToken = default)
        {
            return UniTask.CompletedTask;
        }

        public abstract UniTask UpdateAsync(CancellationToken cancellationToken = default);

        public virtual UniTask ExitAsync(CancellationToken cancellationToken = default)
        {
            return UniTask.CompletedTask;
        }
    }

    /// <summary>
    /// Состояние свободной камеры от первого лица
    /// </summary>
    public class FirstPersonCameraState : BaseCameraState
    {
        public FirstPersonCameraState(PlayerLookController controller, IStateContext<CameraState> context)
            : base(controller, context)
        {
        }

        public override UniTask UpdateAsync(CancellationToken cancellationToken = default)
        {
            // Получаем сглаженный ввод мыши
            var smoothLookInput = Controller.GetSmoothLookInput();
            var mouseX = smoothLookInput.x * Controller.LookSensitivity;
            var mouseY = smoothLookInput.y * Controller.LookSensitivity;

            // Обновляем вертикальный поворот камеры (ограничен по X)
            Controller.RotationX -= mouseY;
            Controller.RotationX = Mathf.Clamp(Controller.RotationX, -Controller.LookXLimit, Controller.LookXLimit);
            Controller.CameraHolder.localRotation = Quaternion.Euler(Controller.RotationX, 0f, 0f);

            // Обновляем горизонтальный поворот (неограничен)
            Controller.transform.rotation *= Quaternion.Euler(0f, mouseX, 0f);

            return UniTask.CompletedTask;
        }
    }

    /// <summary>
    /// Состояние камеры с ограниченным углом поворота
    /// </summary>
    public class RestrictedCameraState : BaseCameraState
    {
        private Vector3 _initialEulerAngles;
        private Quaternion _referenceInverse;

        public RestrictedCameraState(PlayerLookController controller, IStateContext<CameraState> context)
            : base(controller, context)
        {
        }

        public override UniTask EnterAsync(CancellationToken cancellationToken = default)
        {
            var restriction = Controller.GetCurrentRestriction();
            if (restriction == null)
            {
                Debug.LogError("Trying to enter Restricted camera state without restrictions set!");
                Context.ChangeState(CameraState.FirstPerson);
                return UniTask.CompletedTask;
            }

            _referenceInverse = Quaternion.Inverse(restriction.ReferenceRotation);

            // Запоминаем начальные углы
            var localRotation = _referenceInverse * Controller.transform.rotation;
            _initialEulerAngles = localRotation.eulerAngles;

            return UniTask.CompletedTask;
        }

        public override UniTask UpdateAsync(CancellationToken cancellationToken = default)
        {
            var restriction = Controller.GetCurrentRestriction();
            if (restriction == null) return UniTask.CompletedTask;

            // Получаем сглаженный ввод мыши
            var smoothLookInput = Controller.GetSmoothLookInput();
            var mouseX = smoothLookInput.x * Controller.LookSensitivity;
            var mouseY = smoothLookInput.y * Controller.LookSensitivity;

            // Работаем в локальном пространстве относительно опорной точки
            Quaternion localRotation = _referenceInverse * Controller.transform.rotation;

            // Получаем углы Эйлера и нормализуем их в диапазон -180..180
            Vector3 eulerAngles = localRotation.eulerAngles;
            float yaw = eulerAngles.y > 180 ? eulerAngles.y - 360 : eulerAngles.y;

            // Применяем ограничения к горизонтальному повороту
            float newYaw = Mathf.Clamp(yaw + mouseX, restriction.XMin, restriction.XMax);

            // Создаем новое локальное вращение и преобразуем обратно в глобальное
            Quaternion newLocalRotation = Quaternion.Euler(0, newYaw, 0);
            Controller.transform.rotation = restriction.ReferenceRotation * newLocalRotation;

            // Обновляем вертикальный поворот камеры с ограничениями
            Controller.RotationX = Mathf.Clamp(Controller.RotationX - mouseY, restriction.YMin, restriction.YMax);
            Controller.CameraHolder.localRotation = Quaternion.Euler(Controller.RotationX, 0f, 0f);

            return UniTask.CompletedTask;
        }
    }

    /// <summary>
    /// Состояние полностью фиксированной камеры
    /// </summary>
    public class LockedCameraState : BaseCameraState
    {
        private Quaternion _originalRotation;
        private Quaternion _originalCameraHolderRotation;

        public LockedCameraState(PlayerLookController controller, IStateContext<CameraState> context)
            : base(controller, context)
        {
        }

        public override UniTask EnterAsync(CancellationToken cancellationToken = default)
        {
            // Запоминаем текущее вращение для возможного восстановления
            _originalRotation = Controller.transform.rotation;
            _originalCameraHolderRotation = Controller.CameraHolder.localRotation;

            var focusPoint = Controller.GetLockFocusPoint();
            if (focusPoint != null)
            {
                // Устанавливаем камеру точно в заданное положение
                Controller.transform.rotation = focusPoint.rotation;
                Controller.CameraHolder.localRotation = Quaternion.identity;
                Controller.RotationX = 0;
            }

            return UniTask.CompletedTask;
        }

        public override UniTask UpdateAsync(CancellationToken cancellationToken = default)
        {
            // В состоянии блокировки камера не реагирует на ввод,
            // поэтому нам не нужно обрабатывать никакие движения мыши здесь
            return UniTask.CompletedTask;
        }

        public override UniTask ExitAsync(CancellationToken cancellationToken = default)
        {
            // При выходе из состояния блокировки мы не восстанавливаем вращение,
            // так как это может быть неожиданно для игрока
            // Восстановление позиции игрока обычно происходит на уровне взаимодействующего объекта
            return UniTask.CompletedTask;
        }
    }
}