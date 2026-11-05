using Domain.ValueObjects.Product;

namespace Domain.Entities.Product
{
    public class ProductV1
    {
        public ProductId PublicProductId { get; set; }
        public ProductIdentity ProductIdentity { get; set; }
        public ProductInfo ProductInfo { get; set; }
        public bool Active { get; set; } = true;
        public Price? Price { get; set; }
        private bool PriceEnabled { get; set; } = false;

        public void SetPrice(Price price)
        {
            Price = price;
            PriceEnabled = true;
        }

        public void DisablePrice()
        {
            PriceEnabled = false;
            Price = null;
        }

        public bool IsPriceEnabled()
        {
            return PriceEnabled;
        }
    }
}
