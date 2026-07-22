using Domain.Entities.Product;
using Domain.Events.Product;

namespace InventoryApi.Model.Action
{
    public class ProductAggregate : Aggregate
    {
        public Product CurrentProduct { get; set; } = new Product();

        public void CreateProduct(
            int Version,
            ProductIdentity ProductIdentity)
        {
            if(CurrentProduct != null) {
                throw new InvalidOperationException("Product already exists. Cannot create a new product.");
            }

            var productCreated = new ProductCreated(
                Version,
                ProductIdentity);

            AddEvent(productCreated);
            Apply(productCreated);

            commit();

        }

        public void UpdateProductDetails(
            int Version,
            ProductInfo productInfo,
            ProductIdentity ProductIdentity)
        {
            var productUpdated = new ProductDetailsUpdated(
                Version,
                ProductIdentity,
                productInfo);

            AddEvent(productUpdated);

            Apply(productInfo);

            commit();

        }
        public void EnableProductPrice(
            int Version,
            ProductIdentity ProductIdentity,
            Price price)
        {

            var priceEnabled = new ProductPriceEnabled(
                Version,
                ProductIdentity,
                price);

            AddEvent(priceEnabled);
            Apply(price);

            commit();

        }

        public void DeactiveProduct(ProductDeactivated productDeactivated)
        {
            AddEvent(productDeactivated);

            Apply(productDeactivated);

            commit();

        }

        public void Apply(ProductCreated productCreated)
        {
            //currentProduct.publicProductId = productCreated.ProductIdentity.PublicProductId,
            //currentProduct.name = productCreated.ProductIdentity.Name,
            //currentProduct.description = productCreated.ProductIdentity.Description,
            //currentProduct.version = productCreated.Version,
            //currentProduct.price = price ?? new Price()
            //{
            //    Amount = productCreated.Price?.Amount ?? 0,
            //    Currency = productCreated.Price?.Currency ?? "USD"
            //},
            //currentProduct.metaData = productCreated.MetaData
        }

        public void Apply(ProductInfo productInfo)
        {
            CurrentProduct.ProductInfo = new ProductInfo()
            {
                Name = productInfo.Name ?? CurrentProduct.ProductInfo.Name,
                Description = productInfo.Description ?? CurrentProduct.ProductInfo.Description,
                MetaData = productInfo.MetaData ?? CurrentProduct.ProductInfo.MetaData
            };
        }

        public void Apply(Price price)
        {
            CurrentProduct.Price = price;
        }

        public void Apply(ProductDeactivated productDeactivated)
        {
            CurrentProduct.Active = false;
        }

        public void Apply(ProductActivated productDeactivated)
        {
            CurrentProduct.Active = true;
        }
    }
}
