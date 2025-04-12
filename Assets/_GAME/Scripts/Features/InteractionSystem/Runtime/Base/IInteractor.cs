using UnityEngine;

namespace Sim.Features.InteractionSystem.Base
{
    public interface IInteractor
    {
        Transform Transform { get; }
        Camera PlayerCamera { get; }
        float InteractionDistance { get; }
    }
}