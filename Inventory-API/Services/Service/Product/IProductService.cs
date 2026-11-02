using Domain.Entities.Product;
using BuildingBlocks.Error;
using Services.Service.Product.Dtos;
using Services.Service.Product.Inputs;
using Services.Service.User.Input;

namespace Services.Service.Product
{
    public interface IProductService
    {
        Task<Result<ProductDto>> CreateAsync(UserIdInput idInput, CreateProductInput input, CancellationToken cancellationToken);
        Task<ProductV1> GetByIdAsync(ProductIdInput idInput, CancellationToken cancellationToken);
        Task<IEnumerable<ProductV1>> GetProductsAsync(UserIdInput idInput, CancellationToken cancellationToken);
        Task DeleteAsync(ProductIdInput idInput, CancellationToken cancellationToken);
        Task UpdateAsync(ProductIdInput idInput, ProductInput productInput, CancellationToken cancellationToken);
    }
}
