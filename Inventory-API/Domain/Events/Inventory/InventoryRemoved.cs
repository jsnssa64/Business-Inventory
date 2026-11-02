using Domain.Events.Inventory.Core;

namespace Domain.Events.Inventory
{
    public class InventoryRemoved { 
        public int Version { get; set; }
        public int Quantity { get; set; }
        public required InventoryEvent InventoryEvent { get; set; }
    } 
}
