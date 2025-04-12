using Sim.Features.PlayerSystem.Concrete;

namespace Sim.Features.InteractionSystem.Base
{
     public interface IInteractable
    {
        void InteractPrimary(FPSControllerNew player);
        void InteractSecondary(FPSControllerNew player);
    }
}