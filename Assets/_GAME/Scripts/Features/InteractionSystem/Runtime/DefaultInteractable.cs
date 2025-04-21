using Sim.Features.InteractionSystem.Base;
using UnityEngine;

namespace Sim.Features.InteractionSystem
{
    public class DefaultInteractable : InteractableBase
    {
        public override void InteractInternal(IInteractor playerFacade)
        {
            Debug.Log($"Взаимодействие с {name} типа ({nameof(DefaultInteractable)})");
        }
    }
}