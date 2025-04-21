using UnityEngine;

namespace Sim.Features.ShopSystem.Configs
{
    [CreateAssetMenu(fileName = "New Shelf Config", menuName = "Simulator/Shelf Configuration")]
    public class ShelfConfig : ScriptableObject
    {
        [Header("Основные параметры")]
        [SerializeField] private string _shelfId;
        [SerializeField] private int _capacity = 6;

        // [Header("Визуальные настройки")]
        // [SerializeField] private Color _emptyColor = Color.gray;
        // [SerializeField] private Color _partiallyFilledColor = Color.yellow;
        // [SerializeField] private Color _fullColor = Color.green;

        public string ShelfId => string.IsNullOrEmpty(_shelfId) 
            ? System.Guid.NewGuid().ToString().Substring(0, 8) 
            : _shelfId;

        public int Capacity => _capacity;
        // public Color EmptyColor => _emptyColor;
        // public Color PartiallyFilledColor => _partiallyFilledColor;
        // public Color FullColor => _fullColor;∂

        private void OnValidate()
        {
            // Автоматическая валидация параметров
            _capacity = Mathf.Clamp(_capacity, 1, 20);
        }
    }
}