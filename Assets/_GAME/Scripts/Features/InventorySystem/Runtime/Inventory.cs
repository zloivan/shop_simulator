using System;
using System.Collections.Generic;
using System.Linq;
using IKhom.EventBusSystem.Runtime;
using IKhom.EventBusSystem.Runtime.abstractions;
using JetBrains.Annotations;
using Sim.Features.InventorySystem.Runtime.Base;

namespace Sim.Features.InventorySystem.Runtime
{
    // События инвентаря
    public struct ItemAddedEvent : IEvent
    {
        public string ItemId;
        public string InventoryId;
    }

    public struct ItemRemovedEvent : IEvent
    {
        public string ItemId;
        public string InventoryId;
    }

    [Serializable]
    public class Inventory : IInventory
    {
        private readonly List<IInventoryItem> _items = new();
        private readonly string _inventoryId;

        [PublicAPI] public IReadOnlyList<IInventoryItem> Items => _items.AsReadOnly();
        [PublicAPI] public float CurrentWeight => _items.Sum(i => i.Weight);
        [PublicAPI] public float MaxWeight { get; }

        [PublicAPI] public int Capacity { get; }

        public Inventory(string inventoryId, float maxWeight = float.MaxValue, int capacity = int.MaxValue)
        {
            _inventoryId = inventoryId;
            MaxWeight = maxWeight;
            Capacity = capacity;
        }

        [PublicAPI]
        public bool AddItem(IInventoryItem item)
        {
            // Проверка на лимиты веса и вместимости
            if (CurrentWeight + item.Weight > MaxWeight || _items.Count >= Capacity)
                return false;

            _items.Add(item);

            // Публикация события
            EventBus<ItemAddedEvent>.Raise(new ItemAddedEvent
            {
                ItemId = item.Id,
                InventoryId = _inventoryId
            });

            return true;
        }

        [PublicAPI]
        public bool RemoveItem(string itemId)
        {
            var item = _items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                return false;

            _items.Remove(item);

            // Публикация события
            EventBus<ItemRemovedEvent>.Raise(new ItemRemovedEvent
            {
                ItemId = itemId,
                InventoryId = _inventoryId
            });

            return true;
        }

        [PublicAPI]
        public bool HasItem(string itemId)
        {
            return _items.Any(i => i.Id == itemId);
        }

        [PublicAPI]
        public IInventoryItem GetItem(string itemId)
        {
            return _items.FirstOrDefault(i => i.Id == itemId);
        }
    }
}