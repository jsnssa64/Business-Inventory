namespace Domain.Inventory
{
    public class ProductBase
    {
        public Guid PublicProductId { get; set; } = Guid.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;


        public void Map(dynamic? product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            PublicProductId = product.PublicProductId;
            Name = product.Name;
            Description = product.Description;
        }
    }

    public class Product: ProductBase
    {
        public bool EnabledPrice { get; set; }
        public Price? Price { get; set; } = null;

        public void Map(dynamic product, dynamic? price)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            PublicProductId = product.PublicProductId;
            Name = product.Name;
            Description = product.Description;
            EnabledPrice = product.EnabledPrice;

            Price = null;

            if (price != null)
            {
                Price = new Price()
                {
                    Amount = price.Price,
                    Currency = price.CurrencyCode
                };
            }       
        }
    }
}
