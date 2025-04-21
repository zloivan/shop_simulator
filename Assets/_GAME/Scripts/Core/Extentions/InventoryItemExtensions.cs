using Sim.Features.InventorySystem.Runtime;
using Sim.Features.InventorySystem.Runtime.Base;
using Sim.Features.ProductSystem.Base;
using Sim.Features.ProductSystem.Concrete;
using Sim.Features.ProductSystem.Data;

namespace Sim.Core.Extentions
{
    public static class InventoryItemExtensions
    {
        public static Product ToProduct(this IInventoryItem inventoryItem)
        {
            if (inventoryItem == null) 
                return null;

            return new Product(ProductData.Create(
                inventoryItem.Id,
                inventoryItem.Name,
                0, // Базовая цена, можно заменить на нужное значение
                inventoryItem.Weight
            ));
        }

        public static InventoryItemData ToInventoryData(this IProduct product)
        {
            var inventoryItem = InventoryItemData.Create(
                product.Id,
                product.Name,
                product.Weight,
                product.Icon
            );

            return inventoryItem;
        }
        
        public static IInventoryItem ToInvetoryItem(this IProduct product)
        {
            var inventoryItem = InventoryItemData.Create(
                product.Id,
                product.Name,
                product.Weight,
                product.Icon
            );

            var item = new InventoryItem(inventoryItem);
            return item;
        }
    }
}