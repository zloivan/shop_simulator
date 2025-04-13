using UnityEngine;

namespace Sim.Features.ProductSystem.Data
{
    public class ProductData
    {
        public string Id { get; }
        public string Name { get; }
        public float BasePrice { get; }
        public float CurrentPrice { get; }
        public float Weight { get; }
        public Sprite Icon { get; }
        public string Description { get; }
        public string Category { get; }
        
        private ProductData(string id, string name, float basePrice, float currentPrice, 
                         float weight, Sprite icon, string description, string category)
        {
            Id = id;
            Name = name;
            BasePrice = basePrice;
            CurrentPrice = currentPrice;
            Weight = weight;
            Icon = icon;
            Description = description;
            Category = category;
        }
        
        // Фабричный метод
        public static ProductData Create(string id, string name, float basePrice, float weight, 
                                      Sprite icon = null, string description = "", string category = "Default")
        {
            ValidateProductData(id, name, basePrice, weight);
            
            return new ProductData(
                id,
                name,
                basePrice,
                basePrice, // Изначально текущая цена равна базовой
                weight,
                icon,
                description,
                category
            );
        }
        
        // Метод для создания товара со скидкой/наценкой
        public ProductData WithPrice(float newPrice)
        {
            if (newPrice < 0)
                throw new System.ArgumentException("Цена не может быть отрицательной", nameof(newPrice));
                
            return new ProductData(
                Id,
                Name,
                BasePrice,
                newPrice,
                Weight,
                Icon,
                Description,
                Category
            );
        }
        
        private static void ValidateProductData(string id, string name, float basePrice, float weight)
        {
            if (string.IsNullOrEmpty(id))
                throw new System.ArgumentException("ID не может быть пустым", nameof(id));
                
            if (string.IsNullOrEmpty(name))
                throw new System.ArgumentException("Название не может быть пустым", nameof(name));
                
            if (basePrice < 0)
                throw new System.ArgumentException("Базовая цена не может быть отрицательной", nameof(basePrice));
                
            if (weight < 0)
                throw new System.ArgumentException("Вес не может быть отрицательным", nameof(weight));
        }
    }
}