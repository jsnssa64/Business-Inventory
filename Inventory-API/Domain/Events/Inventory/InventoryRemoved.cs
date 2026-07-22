using Domain.Entities.Inventory;

namespace InventoryApi.Model.Events.Inventory
{
    public class InventoryRemoved { 
        public int Version { get; set; }
        public int Quantity { get; set; }
        public required InventoryEvent InventoryEvent { get; set; }
    } 
    //(int Version, InventoryItemIdentity InventoryItemIdentity, int Quantity) : InventoryEvent(Version, InventoryItemIdentity);
}
