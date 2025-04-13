using System.Collections.Generic;
using Sim.Features.ProductSystem.Configs;
using UnityEngine;

namespace Sim.Features.ProductSystem.Concrete
{
    public class ProductFactory : MonoBehaviour
    {
        [SerializeField] private List<ProductConfig> _availableProducts = new();

        private readonly Dictionary<string, ProductConfig> _productsById = new();
        private readonly Dictionary<string, List<ProductConfig>> _productsByCategory = new();

        private void Awake()
        {
            InitializeProductsCache();
        }

        private void InitializeProductsCache()
        {
            _productsById.Clear();
            _productsByCategory.Clear();

            foreach (var config in _availableProducts)
            {
                if (config == null) continue;

                // Кэширование по ID
                string id = config.CreateProductData().Id;
                _productsById[id] = config;

                // Кэширование по категории
                string category = config.CreateProductData().Category;
                if (!_productsByCategory.ContainsKey(category))
                    _productsByCategory[category] = new List<ProductConfig>();

                _productsByCategory[category].Add(config);
            }
        }

        // Создание товара по ID
        public Product CreateProductById(string id)
        {
            if (_productsById.TryGetValue(id, out var config))
                return config.CreateProduct();

            Debug.LogWarning($"Товар с ID {id} не найден");
            return null;
        }

        // Получение списка товаров по категории
        public List<Product> GetProductsByCategory(string category)
        {
            if (!_productsByCategory.TryGetValue(category, out var configs))
                return new List<Product>();

            var products = new List<Product>();
            foreach (var config in configs)
            {
                products.Add(config.CreateProduct());
            }

            return products;
        }

        // Получение всех доступных товаров
        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();
            foreach (var config in _availableProducts)
            {
                if (config != null)
                    products.Add(config.CreateProduct());
            }

            return products;
        }
    }
}