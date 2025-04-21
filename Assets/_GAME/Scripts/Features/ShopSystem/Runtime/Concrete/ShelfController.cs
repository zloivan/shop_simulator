using IKhom.EventBusSystem.Runtime;
using Sim.Core.Extentions;
using Sim.Features.InteractionSystem.Base;
using Sim.Features.PlayerSystem;
using Sim.Features.ProductSystem.Concrete;
using Sim.Features.ShopSystem.Configs;
using Sim.Features.ShopSystem.Events;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sim.Features.ShopSystem.Runtime.Concrete
{
    public class ShelfController : InteractableBase
    {
        [SerializeField] private ShelfConfig _shelfConfig;
        [SerializeField] private ProductFactory _productFactory;


        [SerializeReference, ShowInInspector, ReadOnly]
        private Shelf _shelf;

        private EventBinding<ShopEvents.ProductAddedToShelfEvent> _productAddedBinding;
        private EventBinding<ShopEvents.ProductRemovedFromShelfEvent> _productRemovedBinding;

        private void Awake()
        {
            // Инициализация полки
            _shelf = Shelf.Create(_productFactory, _shelfConfig.ShelfId, _shelfConfig.Capacity);
        }

        private void OnEnable()
        {
            // Подписка на события полки
            _productAddedBinding = new EventBinding<ShopEvents.ProductAddedToShelfEvent>(OnProductAdded);
            _productRemovedBinding = new EventBinding<ShopEvents.ProductRemovedFromShelfEvent>(OnProductRemoved);

            EventBus<ShopEvents.ProductAddedToShelfEvent>.Register(_productAddedBinding);
            EventBus<ShopEvents.ProductRemovedFromShelfEvent>.Register(_productRemovedBinding);
        }

        private void OnProductRemoved(ShopEvents.ProductRemovedFromShelfEvent obj)
        {
        }

        private void OnProductAdded(ShopEvents.ProductAddedToShelfEvent obj)
        {
        }

        private void OnDisable()
        {
            // Отписка от событий полки
            EventBus<ShopEvents.ProductAddedToShelfEvent>.Deregister(_productAddedBinding);
            EventBus<ShopEvents.ProductRemovedFromShelfEvent>.Deregister(_productRemovedBinding);
        }

        public override void InteractInternal(IInteractor playerFacade)
        {
            if (_shelf.Products.Count <= 0)
                return;

            var product = _shelf.Products[0];
            var playerInventory = ((PlayerFacade)playerFacade).Inventory;

            if (playerInventory == null)
                return;

            if (!playerInventory.Inventory.AddItem(product.ToInvetoryItem()))
                return;

            _shelf.RemoveProduct(product.Id);
        }

        // public void InteractSecondary(IInteractor player)
        // {
        //     // Получаем компонент инвентаря игрока
        //     var playerInventoryComponent = ((PlayerFacade)player).Inventory;
        //
        //     if (playerInventoryComponent == null)
        //     {
        //         Debug.LogWarning("У игрока нет компонента инвентаря!");
        //         return;
        //     }
        //
        //     var inventory = playerInventoryComponent.Inventory;
        //
        //     // Если в инвентаре есть предметы и на полке есть место
        //     if (inventory.Items.Count <= 0 || _shelf.CurrentCount >= _shelf.Capacity)
        //         return;
        //
        //     // Берем первый предмет из инвентаря
        //     var inventoryItem = inventory.Items[0];
        //
        //     // Пытаемся добавить на полку
        //     if (_shelf.AddProduct(inventoryItem.ToProduct()))
        //     {
        //         // Если удалось добавить, удаляем из инвентаря
        //         inventory.RemoveItem(inventoryItem.Id);
        //     }
        // }
    }
}