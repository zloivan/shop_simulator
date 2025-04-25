using IKhom.EventBusSystem.Runtime.abstractions;
using Sim.Features.InteractionSystem.Base;
using UnityEngine;

namespace Sim.Features.PlayerSystem.PlayerComponents
{
    public static class PlayerEvents
    {
        public readonly struct PlayerMoveInput : IEvent
        {
            public readonly Vector2 MoveInputValue;

            public PlayerMoveInput(Vector2 moveInputValue)
            {
                MoveInputValue = moveInputValue;
            }
        }

        public readonly struct PlayerLookInput : IEvent
        {
            public readonly Vector2 LookInputValue;

            public PlayerLookInput(Vector2 lookInputValue)
            {
                this.LookInputValue = lookInputValue;
            }
        }

        public readonly struct PlayerJumpInput : IEvent
        {
            public readonly bool IsJumpPressed;

            public PlayerJumpInput(bool isJumpPressed)
            {
                IsJumpPressed = isJumpPressed;
            }
        }

        public readonly struct PlayerSprintInput : IEvent
        {
            public readonly bool IsSprintPressed;

            public PlayerSprintInput(bool isSprintPressed)
            {
                IsSprintPressed = isSprintPressed;
            }
        }

        public readonly struct PlayerInteractInput : IEvent
        {
            public readonly InteractionType InteractionType;

            public PlayerInteractInput(InteractionType interactionType)
            {
                InteractionType = interactionType;
            }
        }

        public readonly struct OnItemTaken : IEvent
        {
            public readonly GameObject Item;

            public OnItemTaken(GameObject item)
            {
                Item = item;
            }
        }

        public readonly struct ItemDropped : IEvent
        {
            public readonly GameObject Item;

            public ItemDropped(GameObject droppedItem)
            {
                Item = droppedItem;
            }
        }
    }
}