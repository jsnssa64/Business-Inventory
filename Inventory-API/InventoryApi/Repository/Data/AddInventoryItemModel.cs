namespace InventoryApi.Repository.Data
{
    public static class AddInventoryItemModel
    {
        public static string Name { get; set; } = string.Empty;
        public static string Description { get; set; } = string.Empty;
        public static float Price { get; set; }
        public static string CurrencyCode { get; set; } = string.Empty;
        public static int Quantity { get; set; }        
        public static int NewItemId { get; set; }
    }
}
