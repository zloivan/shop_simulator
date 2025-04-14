using IKhom.EventBusSystem.Runtime.abstractions;

namespace Sim.Features.ShopSystem.Events
{
    public class ShopEvents
    {
        /// <summary>
        /// Событие добавления товара на полку
        /// </summary>
        public struct ProductAddedToShelfEvent : IEvent
        {
            public string ShelfId;
            public string ProductId;
        }

        /// <summary>
        /// Событие удаления товара с полки
        /// </summary>
        public struct ProductRemovedFromShelfEvent : IEvent
        {
            public string ShelfId;
            public string ProductId;
        }

        /// <summary>
        /// Событие заполнения полки
        /// </summary>
        public struct ShelfFullEvent : IEvent
        {
            public string ShelfId;
        }
    }
}