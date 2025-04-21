using System.Collections.Generic;

namespace Sim.Features.ProductSystem.Base
{
    /// <summary>
    /// Интерфейс для коробок с товарами
    /// </summary>
    public interface IProductBox
    {
        /// <summary>
        /// Уникальный идентификатор коробки
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Список товаров в коробке
        /// </summary>
        IReadOnlyList<IProduct> Products { get; }

        /// <summary>
        /// Максимальное количество товаров в коробке
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// Текущее количество товаров в коробке
        /// </summary>
        int CurrentCount { get; }

        /// <summary>
        /// Добавить товар в коробку
        /// </summary>
        bool AddProduct(IProduct product);

        /// <summary>
        /// Удалить товар из коробки
        /// </summary>
        bool RemoveProduct(string productId);

        /// <summary>
        /// Проверить, можно ли добавить товар
        /// </summary>
        bool CanAddProduct(IProduct product);
    }
}