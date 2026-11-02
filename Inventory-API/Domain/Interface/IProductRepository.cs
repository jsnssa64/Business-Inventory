using Domain.Entities.Product;
using Domain.ValueObjects.Product;
using Domain.ValueObjects.User;

namespace Domain.Interface
{
    public interface IProductRepository
    {
        Task AddPriceToProductAsync(ProductId productId, Price price, CancellationToken cancellationToken);
        Task<ProductId> AddAsync(UserId userId, ProductV1 product, CancellationToken cancellationToken);
        Task<ProductV1> GetByIdAsync(ProductId productId, CancellationToken cancellationToken);
        Task<IEnumerable<ProductV1>> GetProductsAsync(UserId userId, CancellationToken cancellationToken);
        Task DeleteByIdAsync(ProductId productId, CancellationToken cancellationToken);
        Task RemovePriceAsync(ProductId productId, CancellationToken cancellationToken);
        Task UpdateAsync(ProductId productId, ProductV1 product, CancellationToken cancellationToken);
        Task UpdatePriceAsync(ProductId productId, Price price, CancellationToken cancellationToken);
    }
}