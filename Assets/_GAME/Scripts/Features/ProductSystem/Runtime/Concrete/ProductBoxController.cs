using System;
using System.Linq;
using Sim.Features.InteractionSystem.Base;
using Sim.Features.InventorySystem.Concrete;
using Sim.Features.PlayerSystem.Concrete;
using UnityEngine;

namespace Sim.Features.ProductSystem.Concrete
{
    /// <summary>
    /// Компонент для физического представления коробки с товарами
    /// </summary>
    public class ProductBoxController : MonoBehaviour, IInteractable
    {
        [SerializeReference] private ProductBox _productBox;
        [SerializeField] private ProductFactory _productFactory;

        [Header("Визуальные настройки")]
        [SerializeField] private Renderer _boxRenderer;
        [SerializeField] private Color _emptyBoxColor = Color.gray;
        [SerializeField] private Color _filledBoxColor = Color.green;
        [SerializeField] private Color _fullColor = Color.red;

        // private void Awake()
        // {
        //     _productFactory = FindObjectOfType<ProductFactory>();
        // }

        private void Start()
        {
            // Инициализация коробки, если не установлена
            if (_productBox == null || _productBox.Products.Count == 0)
            {
                _productBox = ProductBox.Create(
                    string.Empty,
                    _productFactory
                );
            }

            // Обновление визуального представления
            UpdateBoxVisual();
        }

        private void UpdateBoxVisual()
        {
            if (_boxRenderer != null)
            {
                _boxRenderer.material.color = _productBox.CurrentCount == _productBox.Capacity 
                    ? _fullColor 
                    : _productBox.CurrentCount > 0 
                        ? _filledBoxColor 
                        : _emptyBoxColor;
            }
        }

        public void InteractPrimary(PlayerFacade player)
        {
            var playerInventory = player.GetComponent<PlayerInventoryComponent>();

            if (playerInventory == null)
            {
                Debug.LogWarning("У игрока нет компонента инвентаря!");
                return;
            }

            // Проверяем, есть ли товары в коробке
            if (_productBox.Products.Count == 0)
            {
                Debug.LogWarning("Коробка пуста!");
                return;
            }

            // Берём первый товар
            var product = _productBox.Products.First();

            // Создаём объект для инвентаря
            var inventoryItem = new InventoryItem(
                InventoryItemData.Create(
                    product.Id,
                    product.Name,
                    product.Weight,
                    product.Icon
                )
            );

            // Пытаемся добавить товар в инвентарь
            if (playerInventory.Inventory.AddItem(inventoryItem))
            {
                // Удаляем товар из коробки
                _productBox.RemoveProduct(product.Id);
                Debug.Log($"Товар {product.Name} добавлен в инвентарь.");
            }
            else
            {
                Debug.LogWarning("Не удалось добавить товар в инвентарь!");
            }

            // Обновляем визуальное представление коробки
            UpdateBoxVisual();
        }

        public void InteractSecondary(PlayerFacade player)
        {
            if (_productFactory == null)
            {
                Debug.LogWarning("Фабрика продуктов не установлена!");
                return;
            }

            // Получаем все доступные продукты
            var allProducts = _productFactory.GetAllProducts();

            if (allProducts == null || allProducts.Count == 0)
            {
                Debug.LogWarning("Нет доступных продуктов для добавления!");
                return;
            }

            // Выбираем случайный продукт
            var randomProduct = allProducts[UnityEngine.Random.Range(0, allProducts.Count)];

            // Пытаемся добавить продукт в коробку
            if (_productBox.AddProduct(randomProduct))
            {
                Debug.Log($"Продукт {randomProduct.Name} добавлен в коробку.");
            }
            else
            {
                Debug.LogWarning("Не удалось добавить продукт в коробку!");
            }

            // Обновляем визуальное представление коробки
            UpdateBoxVisual();
        }
    }
}