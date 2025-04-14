using IKhom.EventBusSystem.Runtime;
using Sim.Features.InventorySystem.Concrete;
using UnityEngine;

namespace Sim.Features.PlayerSystem.Concrete
{
    public class PlayerInventoryComponent : MonoBehaviour
    {
        [SerializeField] private float _maxInventoryWeight = 10f;
        [SerializeField] private int _inventoryCapacity = 20;

        private EventBinding<ItemAddedEvent> _itemAddedBinding;
        private EventBinding<ItemRemovedEvent> _itemRemovedBinding;

        public Inventory Inventory { get; private set; }

        private void Awake()
        {
            Inventory = new Inventory("player_inventory", _maxInventoryWeight, _inventoryCapacity);
        }

        private void OnEnable()
        {
            // Подписываемся на события инвентаря
            _itemAddedBinding = new EventBinding<ItemAddedEvent>(OnItemAdded);
            _itemRemovedBinding = new EventBinding<ItemRemovedEvent>(OnItemRemoved);

            EventBus<ItemAddedEvent>.Register(_itemAddedBinding);
            EventBus<ItemRemovedEvent>.Register(_itemRemovedBinding);
        }

        private void OnDisable()
        {
            // Отписываемся от событий
            EventBus<ItemAddedEvent>.Deregister(_itemAddedBinding);
            EventBus<ItemRemovedEvent>.Deregister(_itemRemovedBinding);
        }

        private void OnItemAdded(ItemAddedEvent evt)
        {
            if (evt.InventoryId != "player_inventory")
                return;
            
            Debug.Log($"Добавлен предмет в инвентарь: {evt.ItemId}");
            PrintInventoryStatus();
        }

        private void OnItemRemoved(ItemRemovedEvent evt)
        {
            if (evt.InventoryId != "player_inventory")
                return;
            Debug.Log($"Удален предмет из инвентаря: {evt.ItemId}");
            PrintInventoryStatus();
        }

        private void PrintInventoryStatus()
        {
            Debug.Log(
                $"Инвентарь: {Inventory.Items.Count}/{Inventory.Capacity} предметов, {Inventory.CurrentWeight}/{Inventory.MaxWeight} вес");
        }
    }
}