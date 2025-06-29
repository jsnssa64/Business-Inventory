using Domain.Entities.Product;
using Services.DataModel.Product;
using Services.DataModel.User;

namespace InventoryApi.Service.InventoryService
{
    public interface IProductService
    {
        Task<ProductIdModel> AddProductAsync(UserIdentifierModel userIdentifierModel, ProductDetailsModel productDetailsModel, PriceModel? priceModel);
        Task<Product> GetProductByIdAsync(ProductIdentifierModel productIdentifierModel, CancellationToken cancellationToken);
        Task<IEnumerable<ProductBase>> GetProducts(UserIdentifierModel userIdentifierModel, CancellationToken cancellationToken);
        Task RemoveProduct(ProductIdentifierModel productIdentifierModel, CancellationToken cancellationToken);
        Task UpdateProduct(ProductIdentifierModel productIdentifierModel, UpdateProductDetailsModel updateProductDetailsModel, CancellationToken cancellationToken);
    }
}