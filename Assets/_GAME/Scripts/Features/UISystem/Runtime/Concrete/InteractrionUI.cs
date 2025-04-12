using Sim.Features.InteractionSystem.Base;
using Sim.Features.PlayerSystem.Concrete;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Sim.Features.UISystem.Runtime.Concrete
{
    public class InteractionUI : MonoBehaviour
    {
        [FormerlySerializedAs("crosshair")] [SerializeField] private Image _crosshair;
        [FormerlySerializedAs("interactText")] [SerializeField] private TextMeshProUGUI _interactText;
        [FormerlySerializedAs("playerController")] [SerializeField] private FPSControllerNew _playerController;

        [FormerlySerializedAs("defaultCrosshairColor")]
        [Header("UI Colors")]
        [SerializeField] private Color _defaultCrosshairColor = Color.white;

        [FormerlySerializedAs("interactableCrosshairColor")] [SerializeField] private Color _interactableCrosshairColor = Color.green;

        [FormerlySerializedAs("primaryInteractPrompt")]
        [Header("UI Prompts")]
        [SerializeField] private string _primaryInteractPrompt = "Press Right-click or Left-click to interact";

        [FormerlySerializedAs("secondaryInteractPrompt")] [SerializeField] private string _secondaryInteractPrompt = "Right-click to examine";

        private void Awake()
        {
            if (_playerController == null)
            {
                _playerController = FindObjectOfType<FPSControllerNew>();
            }
        }

        private void Update()
        {
            if (_playerController == null)
            {
                return;
            }
            
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit,
                    _playerController.InteractionDistance))
            {
                Debug.Log("Hitting");
                if (hit.collider.GetComponent<IInteractable>() != null)
                {
                    Debug.Log("Interactable object detected");
                    // Change crosshair color and show text
                    _crosshair.color = _interactableCrosshairColor;

                    // Show both interaction prompts
                    _interactText.text = $"{_primaryInteractPrompt}\n{_secondaryInteractPrompt}";
                    _interactText.gameObject.SetActive(true);
                    return;
                }
            }

            // Default state
            _crosshair.color = _defaultCrosshairColor;
            _interactText.gameObject.SetActive(false);
        }
    }
}