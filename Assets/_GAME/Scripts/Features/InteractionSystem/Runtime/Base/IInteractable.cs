using Sim.Features.PlayerSystem.Concrete;

namespace Sim.Features.InteractionSystem.Base
{
     public interface IInteractable
    {
        void InteractPrimary(FPSController player);
        void InteractSecondary(FPSController player);
    }
}