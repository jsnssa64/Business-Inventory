namespace InventoryApi.Model.Events.Inventory
{
    public class InventoryItemAction
    {
        public static string StreamName = "InventoryItem";
        public string InventoryItemId { get; set; }
    }
}
