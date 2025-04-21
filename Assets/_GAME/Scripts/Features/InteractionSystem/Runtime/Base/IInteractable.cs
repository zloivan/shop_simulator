namespace Sim.Features.InteractionSystem.Base
{
    public enum InteractionType
    {
        Primary,
        Secondary,
        Tertiary,
        DropItem,
        OpenBox,
        Disasasemble,
        PlaceItem,
    }
    public interface IInteractable
    {
        void Interact(IInteractor playerFacade, InteractionType interactionType);
    }
}