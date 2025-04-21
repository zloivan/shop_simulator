using Sim.Features.InteractionSystem.Base;
using UnityEngine;

namespace Sim.Features.InteractionSystem
{
    public class DefaultInteractable : InteractableBase
    {
        public override void InteractInternal(IInteractor playerFacade, InteractionType interactionType)
        {
            // Логика взаимодействия по умолчанию
            Debug.Log($"Взаимодействие с {name} типа {interactionType}");

            // Здесь можно добавить дополнительную логику, если необходимо
        }
    }
}