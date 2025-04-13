using IKhom.EventBusSystem.Runtime.abstractions;

namespace Sim.Features.InventorySystem.Events
{
    public struct ItemAddedEvent : IEvent
    {
        public string ItemId;
        public string InventoryId;
    }
    
    public struct ItemRemovedEvent : IEvent
    {
        public string ItemId;
        public string InventoryId;
    }
    
    public struct InventoryFullEvent : IEvent
    {
        public string InventoryId;
    }
    
    public struct InventoryWeightLimitEvent : IEvent
    {
        public string InventoryId;
        public float CurrentWeight;
        public float MaxWeight;
    }
}