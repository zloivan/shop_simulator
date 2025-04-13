using UnityEngine;

namespace Sim.Features.ProductSystem.Base
{
    public interface IProduct
    {
        string Id { get; }
        string Name { get; }
        float Price { get; }
        float Weight { get; }
        Sprite Icon { get; }
        string Description { get; }
        string Category { get; }
    }
}
