using Sim.Features.InteractionSystem.Base;
using Sim.Features.PlayerSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sim.Features.InventorySystem.Runtime.Testing
{
    public class InventoryTester : InteractableBase
    {
        [SerializeField] private string _itemId = "test_item";
        [SerializeField] private string _itemName = "Тестовый предмет";
        [SerializeField] private float _itemWeight = 1.0f;
        [SerializeField] private Sprite _itemIcon;

        private void OnValidate()
        {
            name = _itemName + " " + _itemId + $" {_itemWeight}" + " (InventoryTester)";
        }

        public override void InteractInternal(IInteractor playerFacade)
        {
            Debug.Log($"Попытка добавить предмет {_itemName} в инвентарь");

            // Получаем компонент инвентаря игрока
            var playerInventory = ((Player)playerFacade).Inventory;

            if (playerInventory == null)
            {
                Debug.LogError("У игрока нет компонента инвентаря!");
                return;
            }

            // Создаем тестовый предмет
            var itemData = InventoryItemData.Create(_itemId, _itemName, _itemWeight, _itemIcon);
            var item = new InventoryItem(itemData);

            // Добавляем в инвентарь
            bool added = playerInventory.Inventory.AddItem(item);

            if (added)
            {
                Debug.Log($"Предмет {_itemName} успешно добавлен в инвентарь");

                // Опционально деактивируем объект
                gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning($"Не удалось добавить предмет {_itemName} в инвентарь");
            }
        }

        public void InteractSecondary(IInteractor player)
        {
            Debug.Log($"Осмотр предмета: {_itemName}, вес: {_itemWeight}");
        }
    }
}