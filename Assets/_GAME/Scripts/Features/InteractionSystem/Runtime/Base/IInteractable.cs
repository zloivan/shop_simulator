using Sim.Features.PlayerSystem;

namespace Sim.Features.InteractionSystem.Base
{
     public interface IInteractable
    {
        void InteractPrimary(PlayerFacade playerFacade);
        void InteractSecondary(PlayerFacade player);
    }
}