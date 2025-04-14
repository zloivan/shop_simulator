using System.Collections.Generic;
using Sim.Features.ProductSystem.Base;

namespace Sim.Features.ShopSystem.Runtime.Base
{
    /// <summary>
    /// Интерфейс полки для хранения и отображения товаров
    /// </summary>
    public interface IShelf
    {
        /// <summary>
        /// Уникальный идентификатор полки
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Максимальная вместимость полки
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// Текущее количество товаров на полке
        /// </summary>
        int CurrentCount { get; }

        /// <summary>
        /// Список товаров на полке
        /// </summary>
        IReadOnlyList<IProduct> Products { get; }

        /// <summary>
        /// Проверка возможности добавления товара
        /// </summary>
        bool CanAddProduct(IProduct product);

        /// <summary>
        /// Добавление товара на полку
        /// </summary>
        bool AddProduct(IProduct product);

        /// <summary>
        /// Удаление товара с полки
        /// </summary>
        bool RemoveProduct(string productId);
    }
}