namespace InventoryApi.Model.DTO
{
    public class AddToInventoryDTO : IEquatable<AddToInventoryDTO>
    {
        public string Name { get; set; }
        public int InventoryQuantity { get; set; }

        public bool Equals(AddToInventoryDTO? other)
        {
            return Name == other.Name;
        }
    }
}
