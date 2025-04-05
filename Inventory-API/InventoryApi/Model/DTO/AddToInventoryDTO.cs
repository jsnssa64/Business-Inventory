namespace InventoryApi.Model.DTO
{
    public class AddToInventoryDTO : IEquatable<AddToInventoryDTO?>
    {
        public required string Name { get; set; }
        public int InventoryQuantity { get; set; }

        public bool Equals(AddToInventoryDTO? other)
        {
            if (other is null) return false;
            return Name == other.Name;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as AddToInventoryDTO);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
