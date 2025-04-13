using UnityEngine;

namespace Sim.Features.InventorySystem.Base
{
    public interface IInventoryItem
    {
        string Id { get; }
        string Name { get; }
        float Weight { get; }
        
        
    }
}