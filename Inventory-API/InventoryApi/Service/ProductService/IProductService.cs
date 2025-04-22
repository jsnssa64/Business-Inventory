using Domain.Inventory;
using InventoryApi.Repository.Data;
using InventoryApi.Repository.Data.Product;

namespace InventoryApi.Service.InventoryService
{
    public interface IProductService
    {
        Task<ProductIdModel> AddProductAsync(UserIdentifierModel userIdentifierModel, ProductDetailsModel productDetailsModel, PriceModel? priceModel);
        Task<Product> GetProductByIdAsync(ProductIdentifierModel productIdentifierModel, CancellationToken cancellationToken);
        Task<IEnumerable<Product>> GetProducts(UserIdentifierModel userIdentifierModel, CancellationToken cancellationToken);
        Task RemoveProduct(ProductIdentifierModel productIdentifierModel, CancellationToken cancellationToken);
        Task UpdateProduct(ProductIdentifierModel productIdentifierModel, UpdateProductDetailsModel updateProductDetailsModel, CancellationToken cancellationToken);
    }
}