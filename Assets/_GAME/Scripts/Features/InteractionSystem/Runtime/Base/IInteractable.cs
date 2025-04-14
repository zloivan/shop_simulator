using Sim.Features.PlayerSystem.Concrete;

namespace Sim.Features.InteractionSystem.Base
{
     public interface IInteractable
    {
        void InteractPrimary(PlayerFacade player);
        void InteractSecondary(PlayerFacade player);
    }
}