using Sim.Features.InteractionSystem.Base;
using Sim.Features.PlayerSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Sim.Features.UISystem.Runtime.Concrete
{
    public class InteractionUI : MonoBehaviour
    {
        [SerializeField] private Image _crosshair;
        [SerializeField] private TextMeshProUGUI _interactText;
        [FormerlySerializedAs("_playerFacade")] [FormerlySerializedAs("_playerController")] [SerializeField] private Player _player;

        [Header("UI Colors")]
        [SerializeField] private Color _defaultCrosshairColor = Color.white;

        [SerializeField] private Color _interactableCrosshairColor = Color.green;


        [Header("UI Prompts")]
        [SerializeField] private string _primaryInteractPrompt = "Press Right-click or Left-click to interact";

        [SerializeField] private string _secondaryInteractPrompt = "Right-click to examine";

        private void Awake()
        {
            if (_player == null)
            {
                _player = FindObjectOfType<Player>();
            }
        }

        private void Update()
        {
            if (_player == null)
            {
                return;
            }

            var playerCameraTransform = _player.LookController.Camera.transform;
            
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out var hit,
                    _player.InteractionDistance))
            {
                if (hit.collider.GetComponent<IInteractable>() != null)
                {
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