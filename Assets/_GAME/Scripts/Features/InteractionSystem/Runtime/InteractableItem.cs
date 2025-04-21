using Sim.Features.InteractionSystem.Base;
using Sim.Features.PlayerSystem;
using UnityEngine;

namespace Sim.Features.InteractionSystem
{
    public class InteractableItem : MonoBehaviour, IInteractable
    {
        [SerializeField] private bool _hasHighlight;
        
        public virtual void InteractPrimary(PlayerFacade playerFacade)
        {
            
        }

        public virtual void InteractSecondary(PlayerFacade player)
        {
            
        }
    }
}