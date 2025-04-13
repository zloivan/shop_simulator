using Sim.Features.ProductSystem.Base;
using Sim.Features.ProductSystem.Data;
using UnityEngine;

namespace Sim.Features.ProductSystem.Concrete
{
    public class Product : IProduct
    {
        private readonly ProductData _data;
        
        public string Id => _data.Id;
        public string Name => _data.Name;
        public float Price => _data.CurrentPrice;
        public float Weight => _data.Weight;
        public Sprite Icon => _data.Icon;
        public string Description => _data.Description;
        public string Category => _data.Category;
        
        public Product(ProductData data)
        {
            _data = data ?? throw new System.ArgumentNullException(nameof(data));
        }
        
        // Метод для создания нового товара с изменённой ценой
        public Product WithPrice(float newPrice)
        {
            return new Product(_data.WithPrice(newPrice));
        }
    }
}