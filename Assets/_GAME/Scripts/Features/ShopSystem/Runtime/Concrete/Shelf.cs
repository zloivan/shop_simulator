using System;
using System.Collections.Generic;
using Sim.Features.ProductSystem.Base;
using Sim.Features.ProductSystem.Concrete;
using Sim.Features.ShopSystem.Runtime.Base;
using UnityEngine;

namespace Sim.Features.ShopSystem.Runtime.Concrete
{
    /// <summary>
    /// Конкретная реализация полки с товарами
    /// </summary>
    [Serializable]
    public class Shelf : ShelfBase
    {
        [NonSerialized] private ProductFactory _productFactory;

        public void Initialize(ProductFactory productFactory)
        {
            _productFactory = productFactory ?? 
                              throw new ArgumentNullException(nameof(productFactory));
        }

        protected override void EnsureProductsLoaded()
        {
            if (CachedProducts == null)
            {
                if (_productFactory == null)
                {
                    Debug.LogWarning("ProductFactory не установлена для восстановления продуктов");
                    return;
                }

                CachedProducts = new List<IProduct>();
                foreach (var productId in _productIds)
                {
                    var product = _productFactory.CreateProductById(productId);
                    if (product != null)
                    {
                        CachedProducts.Add(product);
                    }
                }
            }
        }

        // Статический метод создания полки
        public static Shelf Create(
            ProductFactory productFactory, 
            string id = null, 
            int capacity = 6, 
            IEnumerable<IProduct> initialProducts = null)
        {
            var shelf = new Shelf(id, capacity);
            shelf.Initialize(productFactory);

            if (initialProducts != null)
            {
                foreach (var product in initialProducts)
                {
                    if (!shelf.AddProduct(product))
                        break;
                }
            }

            return shelf;
        }

        // Приватный конструктор для использования в статическом методе создания
        private Shelf(string id = null, int capacity = 6) : base(id, capacity)
        {
        }
    }
}