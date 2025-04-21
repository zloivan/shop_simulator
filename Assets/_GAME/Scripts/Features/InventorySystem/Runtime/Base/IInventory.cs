using System.Collections.Generic;

namespace Sim.Features.InventorySystem.Runtime.Base
{
    public interface IInventory
    {
        // Основные свойства
        IReadOnlyList<IInventoryItem> Items { get; }
        float CurrentWeight { get; }
        float MaxWeight { get; }
        int Capacity { get; }
        
        // Основные методы
        bool AddItem(IInventoryItem item);
        bool RemoveItem(string itemId);
        bool HasItem(string itemId);
        IInventoryItem GetItem(string itemId);
    }
}