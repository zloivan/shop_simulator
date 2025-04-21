using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using IKhom.ExtensionsLibrary.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sim.Features.InteractionSystem.Base
{
    public class InteractableBase : MonoBehaviour, IInteractable
    {
        [SerializeField] private bool _shouldHighlight = true;
        [SerializeField] private InteractionType _interactionType;
        [SerializeField] private Outline _outline;
        private bool _canInteract;


        protected virtual void Awake()
        {
            ManageHighlight().Forget();
        }

        private async UniTask ManageHighlight()
        {
            if (!_shouldHighlight) return;

            if (_outline == null)
            {
                _outline = gameObject.GetOrAddComponent<Outline>();
            }

            await UniTask.WaitForEndOfFrame(this);
            _outline.enabled = false;
        }

        private void OnValidate()
        {
            if (_shouldHighlight)
            {
                if (_outline == null)
                {
                    _outline = gameObject.GetOrAddComponent<Outline>();
                }
            }
            else
            {
                if (gameObject.TryGetComponent<Outline>(out var outline))
                {
                    DestroyImmediate(outline);
                }
            }
        }

        public virtual void Interact(IInteractor playerFacade, InteractionType interactionType)
        {
            if (interactionType != _interactionType)
            {
                return;
            }

            // Логика взаимодействия
            Debug.Log($"Взаимодействие с {name} типа {interactionType}");
        }

        public bool CanInteract
        {
            get => _canInteract;
            set
            {
                _canInteract = value;
                
                if (_shouldHighlight)
                {
                    _outline.enabled = _canInteract;
                }
            }
        }
    }
}