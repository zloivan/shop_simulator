using IKhom.ExtensionsLibrary.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sim.Features.InteractionSystem.Base
{
    public class InteractableBase : MonoBehaviour, IInteractable
    {
        [SerializeField] private bool _shouldHighlight = true;

        [SerializeField] [ShowIf(nameof(_shouldHighlight))]
        private Color _highlightColor = Color.green;

        [SerializeField] [ShowIf(nameof(_shouldHighlight))]
        private float _highlightWidth = 2f;


        [SerializeField, HideInInspector] private Outline _outline;
        private bool _canInteract;


        protected virtual void Awake()
        {
            if (_shouldHighlight && _outline == null)
            {
                _outline = gameObject.GetOrAddComponent<Outline>();
            }
        }

        private void Start()
        {
            _outline.OutlineColor = _highlightColor;
            _outline.OutlineWidth = _highlightWidth;
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

        public virtual void Interact(IInteractor playerFacade, InputAction.CallbackContext callbackContext)
        {
        }

        public bool CanInteract
        {
            get => _canInteract;
            set
            {
                _canInteract = value;
                _outline.enabled = _shouldHighlight && _canInteract;
            }
        }
    }
}