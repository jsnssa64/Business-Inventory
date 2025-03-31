namespace InventoryApi.Model.Events.Inventory
{
    public class InventoryItemRestocked : InventoryItemAction
    {
        public int Quantity { get; set; }
    }
}
