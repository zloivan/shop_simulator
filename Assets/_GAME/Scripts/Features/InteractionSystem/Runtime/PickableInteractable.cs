using Sim.Features.InteractionSystem.Base;
using UnityEngine;

namespace Sim.Features.InteractionSystem
{
    public class PickableInteractable : InteractableBase
    {
        public override void InteractInternal(IInteractor playerFacade, InteractionType interactionType)
        {
            Debug.Log($"Взаимодействие с {name} типа ({nameof(PickableInteractable)}) типа {interactionType}");
        }
    }
}