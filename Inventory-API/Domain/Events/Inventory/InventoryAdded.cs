using MediatR;

namespace InventoryApi.Model.Events.Inventory
{
    public record InventoryAdded: INotification
    {
        public int Version {  get; set; }

        public int Quantity { get; set; }
        
        public required InventoryEvent InventoryEvent { get; set; }
    }
}
