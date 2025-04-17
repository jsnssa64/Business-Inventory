namespace Domain.Inventory
{
    public class Product
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public bool EnabledPrice { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;

        public void Map(dynamic product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            Id = product.Id;
            Name = product.Name;
            Description = product.Description;
            Quantity = product.Quantity;
            Price = product.Price;
            EnabledPrice = product.EnabledPrice;
            CurrencyCode = product.CurrencyCode;
        }
    }
}
