using System;
using System.Collections.Generic;
using System.Linq;
using Sim.Features.ProductSystem.Base;
using Sim.Features.ProductSystem.Configs;
using UnityEngine;

namespace Sim.Features.ProductSystem.Concrete
{
    /// <summary>
    /// Сериализуемая обертка для продукта
    /// </summary>
    [Serializable]
    public class SerializableProductWrapper
    {
        [SerializeField] private string _productId;
        [SerializeField] private string _productName;
        [SerializeField] private float _price;
        [SerializeField] private float _weight;

        public SerializableProductWrapper(IProduct product)
        {
            _productId = product.Id;
            _productName = product.Name;
            _price = product.Price;
            _weight = product.Weight;
        }

        public IProduct ToProduct(ProductFactory productFactory)
        {
            // Воссоздаем продукт через фабрику
            return productFactory.CreateProductById(_productId);
        }
    }

    /// <summary>
    /// Сериализуемая реализация коробки с товарами
    /// </summary>
    [Serializable]
    public class ProductBox : IProductBox
    {
        [SerializeField] private string _id;
        [SerializeField] private int _capacity = 6;

        // Сериализуемый список обверток продуктов
        [SerializeField] private List<SerializableProductWrapper> _serializedProducts;

        // Транзиентное поле для кэширования продуктов
        [NonSerialized]
        private List<IProduct> _cachedProducts;

        // Фабрика продуктов для восстановления
        [NonSerialized]
        private ProductFactory _productFactory;

        public string Id => _id;
        public int Capacity => _capacity;
        public int CurrentCount => GetProducts().Count;

        public IReadOnlyList<IProduct> Products
        {
            get
            {
                EnsureProductsLoaded();
                return _cachedProducts.AsReadOnly();
            }
        }

        // Конструктор для создания коробки
        public ProductBox(string id, int capacity = 6)
        {
            _id = string.IsNullOrEmpty(id)
                ? Guid.NewGuid().ToString().Substring(0, 8)
                : id;
            _capacity = capacity;
            _serializedProducts = new List<SerializableProductWrapper>();
        }

        // Метод для установки фабрики продуктов (вызывается извне)
        public void SetProductFactory(ProductFactory factory)
        {
            _productFactory = factory;
        }

        // Загрузка продуктов из сериализуемых обверток
        private void EnsureProductsLoaded()
        {
            if (_cachedProducts == null)
            {
                _cachedProducts = new List<IProduct>();

                if (_productFactory == null)
                {
                    Debug.LogWarning("ProductFactory не установлена для восстановления продуктов");
                    return;
                }

                foreach (var wrapper in _serializedProducts)
                {
                    var product = wrapper.ToProduct(_productFactory);
                    if (product != null)
                    {
                        _cachedProducts.Add(product);
                    }
                }
            }
        }

        // Получение списка продуктов с кэшированием
        private List<IProduct> GetProducts()
        {
            EnsureProductsLoaded();
            return _cachedProducts ?? new List<IProduct>();
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

            // Добавляем в сериализуемый список и кэш
            _serializedProducts.Add(new SerializableProductWrapper(product));

            if (_cachedProducts == null)
                _cachedProducts = new List<IProduct>();

            _cachedProducts.Add(product);
            return true;
        }

        public bool RemoveProduct(string productId)
        {
            // Удаление из сериализуемого списка
            var serializableToRemove = _serializedProducts
                .FirstOrDefault(p => p.ToProduct(_productFactory)?.Id == productId);

            if (serializableToRemove == null)
                return false;

            _serializedProducts.Remove(serializableToRemove);

            // Удаление из кэша
            if (_cachedProducts != null)
            {
                var productToRemove = _cachedProducts.FirstOrDefault(p => p.Id == productId);
                if (productToRemove != null)
                    _cachedProducts.Remove(productToRemove);
            }

            return true;
        }

        // Статический метод создания коробки
        public static ProductBox Create(
            string id,
            ProductFactory productFactory,
            IEnumerable<IProduct> initialProducts = null,
            int capacity = 6)
        {
            var box = new ProductBox(id, capacity);
            box.SetProductFactory(productFactory);

            if (initialProducts != null)
            {
                foreach (var product in initialProducts)
                {
                    if (!box.AddProduct(product))
                        break;
                }
            }

            return box;
        }
    }
}