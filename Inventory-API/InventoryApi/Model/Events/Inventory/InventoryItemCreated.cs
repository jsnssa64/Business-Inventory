namespace InventoryApi.Model.Events.Inventory
{
    public class InventoryItemCreated: InventoryItemAction
    {
        public int Quantity { get; set; }
    }
}
