using System;
using System.Collections.Generic;
using IKhom.EventBusSystem.Runtime;
using Sim.Features.ProductSystem.Base;
using Sim.Features.ShopSystem.Events;
using UnityEngine;

namespace Sim.Features.ShopSystem.Runtime.Base
{
    /// <summary>
    /// Базовая абстрактная реализация полки
    /// </summary>
    [Serializable]
    public abstract class ShelfBase : IShelf
    {
        [SerializeField] protected string _id;
        [SerializeField] protected int _capacity = 6;
        
        // Сериализуемый список для хранения товаров
        [SerializeField] protected List<string> _productIds = new();

        protected List<IProduct> CachedProducts;
        public string Id => _id;
        public int Capacity => _capacity;
        public int CurrentCount => _productIds.Count;

        public IReadOnlyList<IProduct> Products 
        {
            get 
            {
                EnsureProductsLoaded();
                return CachedProducts?.AsReadOnly() ?? new List<IProduct>().AsReadOnly();
            }
        }

        protected ShelfBase(string id = null, int capacity = 6)
        {
            _id = id ?? Guid.NewGuid().ToString().Substring(0, 8);
            _capacity = capacity;
        }

        public bool CanAddProduct(IProduct product)
        {
            if (product == null)
                return false;

            return CurrentCount < Capacity;
        }

        public bool AddProduct(IProduct product)
        {
            if (!CanAddProduct(product))
                return false;

            _productIds.Add(product.Id);
    
            // Публикация события добавления товара
            EventBus<ShopEvents.ProductAddedToShelfEvent>.Raise(new ShopEvents.ProductAddedToShelfEvent 
            { 
                ShelfId = Id, 
                ProductId = product.Id 
            });
    
            // Сбрасываем кэш для принудительной перезагрузки
            CachedProducts = null;

            return true;
        }

        public bool RemoveProduct(string productId)
        {
            var removed = _productIds.Remove(productId);


            if (!removed) 
                return false;
            
            // Событие удаления здесь
            EventBus<ShopEvents.ProductRemovedFromShelfEvent>.Raise(new ShopEvents.ProductRemovedFromShelfEvent 
            { 
                ShelfId = Id, 
                ProductId = productId 
            });
                
            // Сбрасываем кэш для принудительной перезагрузки
            CachedProducts = null;

            return true;
        }

        // Метод для загрузки продуктов должен быть реализован в наследниках
        protected abstract void EnsureProductsLoaded();
    }
}