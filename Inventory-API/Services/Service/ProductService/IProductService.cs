using Domain.Entities.Product;
using Services.DataModel.Product;
using Services.DataModel.User;

namespace Services.Service.InventoryService
{
    public interface IProductService
    {
        Task<ProductIdModel> AddProductAsync(UserIdentifierModel userIdentifierModel, ProductDetailsModel productDetailsModel, PriceModel? priceModel);
        Task<Product> GetProductByIdAsync(ProductIdentity productIdentifierModel, CancellationToken cancellationToken);
        Task<IEnumerable<Product>> GetProducts(UserIdentifierModel userIdentifierModel, CancellationToken cancellationToken);
        Task RemoveProduct(ProductIdentity productIdentifierModel, CancellationToken cancellationToken);
        Task UpdateProduct(ProductIdentity productIdentifierModel, UpdateProductDetailsModel updateProductDetailsModel, CancellationToken cancellationToken);
    }
}