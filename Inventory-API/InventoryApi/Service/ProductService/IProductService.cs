using Domain.Inventory;
using Domain.User;
using InventoryApi.Repository.Data;
using InventoryApi.Repository.Data.Product;
using InventoryApi.Repository.Data.User;

namespace InventoryApi.Service.InventoryService
{
    public interface IProductService
    {
        Task<ProductIdModel> AddProductAsync(ProductIdentifierModel productIdentifierModel, ProductDetailsModel productDetailsModel, PriceModel? priceModel);
        Task<Product> GetProductByIdAsync(ProductIdentifierModel productIdentifierModel, CancellationToken cancellationToken);
        Task<IEnumerable<Product>> GetProducts(string userName, CancellationToken cancellationToken);
        Task RemoveProduct(ProductIdentifierModel productIdentifierModel, CancellationToken cancellationToken);
        Task<Product> UpdateProduct(ProductIdentifierModel productIdentifierModel, UpdateProductDetailsModel updateProductDetailsModel, CancellationToken cancellationToken);
    }
}