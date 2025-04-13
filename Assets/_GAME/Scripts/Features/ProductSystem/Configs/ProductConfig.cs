using Sim.Features.ProductSystem.Concrete;
using Sim.Features.ProductSystem.Data;
using UnityEngine;

namespace Sim.Features.ProductSystem.Configs
{
    [CreateAssetMenu(fileName = "New Product", menuName = "Simulator/Product Config")]
    public class ProductConfig : ScriptableObject
    {
        [Header("Идентификация")]
        [SerializeField] private string _id;
        [SerializeField] private string _name;

        [Header("Экономика")]
        [Min(0)]
        [SerializeField] private float _basePrice;

        [Min(0)]
        [SerializeField] private float _weight;

        [Header("Отображение")]
        [SerializeField] private Sprite _icon;

        [TextArea(3, 5)]
        [SerializeField] private string _description;

        [SerializeField] private string _category = "Default";

        // Проверка данных при вызове в редакторе
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(_id))
                _id = System.Guid.NewGuid().ToString().Substring(0, 8);

            if (string.IsNullOrEmpty(_name))
                _name = "Новый товар";
        }

        // Метод для создания экземпляра данных товара
        public ProductData CreateProductData()
        {
            return ProductData.Create(
                _id,
                _name,
                _basePrice,
                _weight,
                _icon,
                _description,
                _category
            );
        }

        // Метод для создания готового товара
        public Product CreateProduct()
        {
            return new Product(CreateProductData());
        }
    }
}