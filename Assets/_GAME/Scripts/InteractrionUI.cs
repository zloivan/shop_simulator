using _GAME.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] private Image crosshair;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private FPSController playerController;
    [SerializeField] private float interactDistance = 2.5f;

    [Header("UI Colors")]
    [SerializeField] private Color defaultCrosshairColor = Color.white;

    [SerializeField] private Color interactableCrosshairColor = Color.green;

    [Header("UI Prompts")]
    [SerializeField] private string primaryInteractPrompt = "Press E or Left-click to interact";

    [SerializeField] private string secondaryInteractPrompt = "Right-click to examine";

    private void Update()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit,
                interactDistance))
        {
            if (hit.collider.GetComponent<IInteractable>() != null)
            {
                // Change crosshair color and show text
                crosshair.color = interactableCrosshairColor;

                // Show both interaction prompts
                interactText.text = $"{primaryInteractPrompt}\n{secondaryInteractPrompt}";
                interactText.gameObject.SetActive(true);
                return;
            }
        }

        // Default state
        crosshair.color = defaultCrosshairColor;
        interactText.gameObject.SetActive(false);
    }
}