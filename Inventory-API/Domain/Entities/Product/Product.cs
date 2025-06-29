namespace Domain.Entities.Product
{
    public class Product
    {
        public ProductIdentity PublicProductId { get; set; }

        public ProductInfo ProductInfo { get; set; } = new ProductInfo();
        public bool Active { get; set; } = true;
        public Price? Price { get; set; } = null;

        //public void Map(dynamic? product, dynamic? price)
        //{
        //    try
        //    {
        //        if (product is null)
        //            throw new ArgumentException("Unable to find product");

        //        PublicProductId = product.PublicProductId;
        //        Name = product.Name;
        //        Description = product.Description;
        //        Version = product.Version;

        //        if (price is null)
        //            return;

        //        Price = new Price()
        //        {
        //            Amount = price.Price,
        //            Currency = price.CurrencyCode
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Unable to map Product ({exception})", ex);
        //    }
        //}
    }
}
