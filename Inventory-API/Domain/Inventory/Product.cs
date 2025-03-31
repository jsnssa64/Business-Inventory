namespace Domain.Inventory
{
    public class Product : IEquatable<Product>
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }


        public bool Equals(Product? other)
        {
            throw new NotImplementedException();
        }
    }
}
