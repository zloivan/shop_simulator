using System;
using IKhom.EventBusSystem.Runtime;
using Sim.Features.InventorySystem.Runtime;
using Sim.Features.InventorySystem.Runtime.Events;
using Sim.Features.PlayerSystem.Base;
using UnityEngine;

namespace Sim.Features.PlayerSystem.PlayerComponents
{
    public class PlayerInventoryComponent : MonoBehaviour, IPlayerComponent
    {
        [SerializeField] private float _maxInventoryWeight = 10f;
        [SerializeField] private int _inventoryCapacity = 20;

        private PlayerFacade _facade;
        private EventBinding<ItemAddedEvent> _itemAddedBinding;
        private EventBinding<ItemRemovedEvent> _itemRemovedBinding;

        // События, которые будут перенаправляться через фасад
        public event Action<string> OnItemAdded;
        public event Action<string> OnItemRemoved;

        // Публичное свойство для доступа через фасад
        public Inventory Inventory { get; private set; }

        #region Unity Lifecycle

        private void Awake()
        {
            // Создаем инвентарь с уникальным ID для игрока
            Inventory = new Inventory("player_inventory", _maxInventoryWeight, _inventoryCapacity);
        }

        private void OnEnable()
        {
            // Подписываемся на глобальные события инвентаря
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            // Отписываемся от глобальных событий
            UnsubscribeFromEvents();
        }

        #endregion

        #region IPlayerComponent Implementation

        public void Initialize(PlayerFacade facade)
        {
            _facade = facade;
        }

        #endregion

        #region Event Subscriptions

        private void SubscribeToEvents()
        {
            _itemAddedBinding = new EventBinding<ItemAddedEvent>(OnItemAddedHandler);
            _itemRemovedBinding = new EventBinding<ItemRemovedEvent>(OnItemRemovedHandler);

            EventBus<ItemAddedEvent>.Register(_itemAddedBinding);
            EventBus<ItemRemovedEvent>.Register(_itemRemovedBinding);
        }

        private void UnsubscribeFromEvents()
        {
            EventBus<ItemAddedEvent>.Deregister(_itemAddedBinding);
            EventBus<ItemRemovedEvent>.Deregister(_itemRemovedBinding);
        }

        #endregion

        #region Event Handlers

        private void OnItemAddedHandler(ItemAddedEvent evt)
        {
            if (evt.InventoryId != "player_inventory")
                return;
            
            Debug.Log($"Добавлен предмет в инвентарь: {evt.ItemId}");
            PrintInventoryStatus();
            
            // Вызываем событие, которое будет перенаправлено через фасад
            OnItemAdded?.Invoke(evt.ItemId);
        }

        private void OnItemRemovedHandler(ItemRemovedEvent evt)
        {
            if (evt.InventoryId != "player_inventory")
                return;
                
            Debug.Log($"Удален предмет из инвентаря: {evt.ItemId}");
            PrintInventoryStatus();
            
            // Вызываем событие, которое будет перенаправлено через фасад
            OnItemRemoved?.Invoke(evt.ItemId);
        }

        #endregion

        #region Utility Methods

        private void PrintInventoryStatus()
        {
            Debug.Log(
                $"Инвентарь: {Inventory.Items.Count}/{Inventory.Capacity} предметов, {Inventory.CurrentWeight}/{Inventory.MaxWeight} вес");
        }

        #endregion
    }
}