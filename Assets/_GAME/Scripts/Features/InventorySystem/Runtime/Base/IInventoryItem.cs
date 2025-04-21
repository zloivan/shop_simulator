namespace Sim.Features.InventorySystem.Runtime.Base
{
    public interface IInventoryItem
    {
        string Id { get; }
        string Name { get; }
        float Weight { get; }
    }
}