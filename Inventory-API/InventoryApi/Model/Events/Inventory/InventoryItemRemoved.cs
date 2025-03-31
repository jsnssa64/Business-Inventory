namespace InventoryApi.Model.Events.Inventory
{
    public class InventoryItemRemoved : InventoryItemAction
    {
        public int Quantity { get; set; }
    }
}
