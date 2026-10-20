using Domain.Entities.Product;
using Services.DataModel.Product;
using Services.DataModel.User;

namespace Services.Service.ProductService
{
    public interface IProductService
    {
        Task<ProductIdModel> AddAsync(UserIdentifierModel userIdentifierModel, ProductDetailsModel productDetailsModel, PriceModel? priceModel, CancellationToken cancellationToken);
        Task<ProductEntity> GetAsync(ProductIdentifierModel productIdentifierModel, CancellationToken cancellationToken);
        Task<IEnumerable<ProductEntity>> GetByUserAsync(UserIdentifierModel userIdentifierModel, CancellationToken cancellationToken);
        Task DeleteAsync(ProductIdentifierModel productIdentifierModel, CancellationToken cancellationToken);
        Task UpdateAsync(ProductIdentifierModel productIdentifierModel, UpdateProductDetailsModel updateProductDetailsModel, CancellationToken cancellationToken);
    }
}