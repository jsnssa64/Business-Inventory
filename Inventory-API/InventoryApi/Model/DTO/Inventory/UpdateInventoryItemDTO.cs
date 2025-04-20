namespace InventoryApi.Model.DTO.Inventory
{
    public class UpdateInventoryItemDTO
    {
        public required Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
