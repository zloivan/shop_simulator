using UnityEngine.InputSystem;

namespace Sim.Features.InteractionSystem.Base
{
    public interface IInteractable
    {
        void Interact(IInteractor playerFacade, InputAction.CallbackContext callbackContext);
    }
}