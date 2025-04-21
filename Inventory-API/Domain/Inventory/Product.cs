namespace Domain.Inventory
{
    public class Product
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public bool EnabledPrice { get; set; }
        public Price? Price { get; set; } = null;

        public void Map(dynamic product, dynamic? price)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            Id = product.Id;
            Name = product.Name;
            Description = product.Description;
            Quantity = product.Quantity;
            EnabledPrice = product.EnabledPrice;

            Price = null;

            if (price != null)
            {
                Price = new Price()
                {
                    Amount = product.Price,
                    Currency = product.CurrencyCode
                };
            }       
        }
    }
}
