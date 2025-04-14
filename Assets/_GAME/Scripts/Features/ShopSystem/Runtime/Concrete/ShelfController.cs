using IKhom.EventBusSystem.Runtime;
using Sim.Features.InteractionSystem.Base;
using Sim.Features.InventorySystem.Concrete;
using Sim.Features.InventorySystem.Extensions;
using Sim.Features.PlayerSystem.Concrete;
using Sim.Features.ProductSystem.Concrete;
using Sim.Features.ProductSystem.Data;
using Sim.Features.ShopSystem.Configs;
using Sim.Features.ShopSystem.Events;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sim.Features.ShopSystem.Runtime.Concrete
{
    public class ShelfController : MonoBehaviour, IInteractable
    {
        [SerializeField] private ShelfConfig _shelfConfig;
        [SerializeField] private ProductFactory _productFactory;
        [SerializeField] private Renderer _shelfRenderer;

        
        [SerializeReference, ShowInInspector, ReadOnly]
        private Shelf _shelf;
        private EventBinding<ShopEvents.ProductAddedToShelfEvent> _productAddedBinding;
        private EventBinding<ShopEvents.ProductRemovedFromShelfEvent> _productRemovedBinding;

        private void Awake()
        {
            // Инициализация полки
            _shelf = Shelf.Create(_productFactory, _shelfConfig.ShelfId, _shelfConfig.Capacity);
            UpdateShelfVisual();
        }

        private void OnEnable()
        {
            // Подписка на события полки
            _productAddedBinding = new EventBinding<ShopEvents.ProductAddedToShelfEvent>(OnProductAdded);
            _productRemovedBinding = new EventBinding<ShopEvents.ProductRemovedFromShelfEvent>(OnProductRemoved);

            EventBus<ShopEvents.ProductAddedToShelfEvent>.Register(_productAddedBinding);
            EventBus<ShopEvents.ProductRemovedFromShelfEvent>.Register(_productRemovedBinding);
        }

        private void OnDisable()
        {
            // Отписка от событий полки
            EventBus<ShopEvents.ProductAddedToShelfEvent>.Deregister(_productAddedBinding);
            EventBus<ShopEvents.ProductRemovedFromShelfEvent>.Deregister(_productRemovedBinding);
        }

        private void OnProductAdded(ShopEvents.ProductAddedToShelfEvent evt)
        {
            if (evt.ShelfId == _shelf.Id)
            {
                UpdateShelfVisual();
            }
        }

        private void OnProductRemoved(ShopEvents.ProductRemovedFromShelfEvent evt)
        {
            if (evt.ShelfId == _shelf.Id)
            {
                UpdateShelfVisual();
            }
        }

        private void UpdateShelfVisual()
        {
            if (_shelfRenderer == null) return;

            // Обновление цвета полки в зависимости от заполненности
            var shelfColor = _shelf.CurrentCount == 0
                ? _shelfConfig.EmptyColor
                : _shelf.CurrentCount >= _shelf.Capacity
                    ? _shelfConfig.FullColor
                    : _shelfConfig.PartiallyFilledColor;

            _shelfRenderer.material.color = shelfColor;
        }

        public void InteractPrimary(FPSControllerNew player)
        {
            // Логика взятия товара с полки
            if (_shelf.Products.Count <= 0) 
                return;
            
            var product = _shelf.Products[0];
            var playerInventory = player.GetComponent<PlayerInventoryComponent>();

            if (playerInventory == null) 
                return;
            
            if (!playerInventory.Inventory.AddItem(product.ToInvetoryItem())) 
                return;
            
            _shelf.RemoveProduct(product.Id);
        }

        public void InteractSecondary(FPSControllerNew player)
        {
            // Получаем компонент инвентаря игрока
            var playerInventoryComponent = player.GetComponent<PlayerInventoryComponent>();
    
            if (playerInventoryComponent == null)
            {
                Debug.LogWarning("У игрока нет компонента инвентаря!");
                return;
            }

            var inventory = playerInventoryComponent.Inventory;

            // Если в инвентаре есть предметы и на полке есть место
            if (inventory.Items.Count <= 0 || _shelf.CurrentCount >= _shelf.Capacity) 
                return;
            
            // Берем первый предмет из инвентаря
            var inventoryItem = inventory.Items[0];

            // Пытаемся добавить на полку
            if (_shelf.AddProduct(inventoryItem.ToProduct()))
            {
                // Если удалось добавить, удаляем из инвентаря
                inventory.RemoveItem(inventoryItem.Id);
            }
        }
    }
}