using Domain.Entities.Product;
using Domain.Events.Product;
using Domain.Events.Product.Enum;
using Domain.ValueObjects.Product;

namespace Domain.Aggregates
{
    public class ProductAggregate(ProductV1 Product) : Aggregate
    {
        public void CreateProduct(
            int Version,
            ProductIdentity ProductIdentity,
            Price Price)
        {
            if(Product != null) {
                throw new InvalidOperationException("Product already exists. Cannot create a new product.");
            }

            var productCreated = new ProductCreated(
                Version,
                ProductIdentity,
                Price,
                ProductStatus.Active);

            AddEvent(productCreated);
            Apply(productCreated);

            Commit();

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

            Commit();
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

            Commit();
        }

        public void DeactiveProduct(ProductDeactivated productDeactivated)
        {
            AddEvent(productDeactivated);

            Apply(productDeactivated);

            Commit();

        }

        public void Apply(ProductCreated productCreated)
        {
            if(Product != null)
            {
                throw new InvalidOperationException("Product is already set. Cannot apply ProductCreated event.");
            }

            Product = new ProductV1()
            {
                ProductInfo = new ProductInfo(
                    productCreated.ProductIdentity.PublicProductId,
                    productCreated.ProductIdentity.Username,
                    productCreated.ProductIdentity.Description),
                Active = true,
                Price = new Price(productCreated.Price.Amount, productCreated.Price.Currency)
            };
        }

        public void Apply(ProductInfo productInfo)
        {
            if(Product == null)
            {
                throw new InvalidOperationException("Product is null. Cannot apply product info.");
            }

            Product.ProductInfo = new ProductInfo(
                productInfo.PublicProductId,
                productInfo.Name,
                productInfo.Description);
        }

        public void Apply(Price price)
        {
            if(Product == null)
            {
                throw new InvalidOperationException("Product is null. Cannot apply price.");
            }

            Product.Price = price;
        }

        public void Apply(ProductDeactivated ProductDeactivated)
        {
            if(Product == null)
            {
                throw new InvalidOperationException("Product is null. Cannot apply product deactivation.");
            }

            Product.Active = false;
        }

        public void Apply(ProductActivated ProductActivated)
        {
            if(Product == null)
            {
                throw new InvalidOperationException("Product is null. Cannot apply product activation.");
            }

            Product.Active = true;
        }
    }
}
