using Sim.Features.InteractionSystem.Base;
using Sim.Features.PlayerSystem.Concrete;
using UnityEngine;

namespace Sim.Features.InteractionSystem.Concrete
{
    public class InteractableItem : MonoBehaviour, IInteractable
    {
        [SerializeField] private string itemName = "Item";
        [SerializeField] private string primaryInteractionMessage = "You picked up an item";
        [SerializeField] private string secondaryInteractionMessage = "You examined the item";

        [SerializeField] private bool canPickUp = true;
        [SerializeField] private bool destroyOnPickup = true;

        [SerializeField] private AudioClip pickupSound;
        [SerializeField] private AudioClip examineSound;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            
            if (audioSource == null && (pickupSound != null || examineSound != null))
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        public void InteractPrimary(FPSControllerNew player)
        {
            // Primary interaction (e.g., pick up)
            Debug.Log(primaryInteractionMessage);

            if (pickupSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(pickupSound);
            }

            if (canPickUp)
            {
                // Add to inventory or perform other actions
                
                // Optionally destroy the object
                if (destroyOnPickup)
                {
                    // Wait for sound to finish if there is one
                    if (pickupSound != null && audioSource != null)
                    {
                        Destroy(gameObject, pickupSound.length);
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }

        public void InteractSecondary(FPSControllerNew player)
        {
            // Secondary interaction (e.g., examine)
            Debug.Log(secondaryInteractionMessage);

            if (examineSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(examineSound);
            }

            // Implement examination behavior
            // For example, display item details UI, show a tooltip, etc.
        }
    }
}