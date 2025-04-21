using Sim.Features.InteractionSystem.Base;
using Sim.Features.PlayerSystem;
using UnityEngine;

namespace Sim.Features.InteractionSystem
{
    public class PickableInteractable : InteractableBase
    {
        public override void InteractInternal(IInteractor interactor)
        {
            Debug.Log($"Взаимодействие с {name} типа ({nameof(PickableInteractable)})");
            
            var playerFacade = interactor as PlayerFacade;
            if (playerFacade == null) 
                return;
        
            var handsController = playerFacade.HandsController;
            if (handsController != null && !handsController.HasItemInHands)
            {
                handsController.TakeItem(gameObject);
            }
        }
    }
}