using Domain.Entities.Product;
using Domain.Events.Product;
using Domain.ValueObjects.Product;

namespace Domain.Aggregates
{
    public class ProductAggregate : Aggregate
    {
        public Product? CurrentProduct { get; set; }

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
            if(CurrentProduct != null)
            {
                throw new InvalidOperationException("CurrentProduct is already set. Cannot apply ProductCreated event.");
            }

            CurrentProduct = new Product(
                productCreated.ProductIdentity,
                new ProductInfo(
                    productCreated.ProductIdentity.PublicProductId,
                    productCreated.ProductIdentity.Name,
                    productCreated.ProductIdentity.Description),
                true,
                productCreated.Price,
                productCreated.Version);
        }

        public void Apply(ProductInfo productInfo)
        {
            if(CurrentProduct == null)
            {
                throw new InvalidOperationException("CurrentProduct is null. Cannot apply product info.");
            }

            CurrentProduct.ProductInfo = new ProductInfo(
                productInfo.Name,
                productInfo.Description);
        }

        public void Apply(Price price)
        {
            if(CurrentProduct == null)
            {
                throw new InvalidOperationException("CurrentProduct is null. Cannot apply price.");
            }

            CurrentProduct.Price = price;
        }

        public void Apply(ProductDeactivated productDeactivated)
        {
            if(CurrentProduct == null)
            {
                throw new InvalidOperationException("CurrentProduct is null. Cannot apply product deactivation.");
            }

            CurrentProduct.Active = false;
        }

        public void Apply(ProductActivated productActivated)
        {
            if(CurrentProduct == null)
            {
                throw new InvalidOperationException("CurrentProduct is null. Cannot apply product activation.");
            }

            CurrentProduct.Active = true;
        }
    }
}
