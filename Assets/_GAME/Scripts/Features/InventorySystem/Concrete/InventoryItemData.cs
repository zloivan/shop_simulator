using Sim.Features.InventorySystem.Base;
using UnityEngine;

namespace Sim.Features.InventorySystem.Concrete
{
    // Иммутабельная реализация предмета инвентаря
    public class InventoryItemData
    {
        public string Id { get; }
        public string Name { get; }
        public float Weight { get; }
        public Sprite Icon { get; }
        public int StackLimit { get; }
        public bool IsStackable { get; }

        private InventoryItemData(string id, string name, float weight, Sprite icon, int stackLimit, bool isStackable)
        {
            Id = id;
            Name = name;
            Weight = weight;
            Icon = icon;
            StackLimit = stackLimit;
            IsStackable = isStackable;
        }

        // Фабричный метод с валидацией
        public static InventoryItemData Create(string id, string name, float weight, Sprite icon = null,
            int stackLimit = 1)
        {
            if (string.IsNullOrEmpty(id))
                throw new System.ArgumentException("ID не может быть пустым", nameof(id));

            if (string.IsNullOrEmpty(name))
                throw new System.ArgumentException("Имя не может быть пустым", nameof(name));

            if (weight < 0)
                throw new System.ArgumentException("Вес не может быть отрицательным", nameof(weight));

            return new InventoryItemData(
                id,
                name,
                weight,
                icon,
                stackLimit,
                stackLimit > 1
            );
        }
    }

    public class InventoryItem : IInventoryItem
    {
        private readonly InventoryItemData _data;

        public string Id => _data.Id;
        public string Name => _data.Name;
        public float Weight => _data.Weight;
        public Sprite Icon => _data.Icon;
        public int StackLimit => _data.StackLimit;
        public bool IsStackable => _data.IsStackable;

        public InventoryItem(InventoryItemData data)
        {
            _data = data ?? throw new System.ArgumentNullException(nameof(data));
        }
    }
}