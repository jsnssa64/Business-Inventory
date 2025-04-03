namespace InventoryApi.Model.DTO
{
    public class InventoryItemDTO : IEquatable<InventoryItemDTO>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public int ItemQuantity { get; set; }
        public int InventoryQuantity { get; set; }

        public bool Equals(InventoryItemDTO? other)
        {
            return Name == other.Name &&
                   Price == other.Price &&
                   Currency == other.Currency &&
                   ItemQuantity == other.ItemQuantity;
        }
    }
}
