namespace InventoryApi.Model.Events.Inventory
{
    public class InventoryItemAction
    {
        public static string StreamName = "InventoryItem";
        public int InventoryItemId { get; set; }
    }
}
