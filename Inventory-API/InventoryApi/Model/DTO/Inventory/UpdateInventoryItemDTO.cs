namespace InventoryApi.Model.DTO.Inventory
{
    public class UpdateInventoryItemDTO
    {
        public required string Id { get; set; }
        public int Quantity { get; set; }
    }
}
