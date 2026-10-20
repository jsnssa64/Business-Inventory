using System.Data;
using Domain.Entities.Product;
using Services.DataModel.Product;
using Services.DataModel.User;

namespace Services.Repository.Product
{
    public interface IProductRepository
    {
        Task AddPriceToProduct(IDbConnection dbConnection, ProductIdentifierModel productIdentifierModel, PriceModel price, IDbTransaction? dbTransaction, CancellationToken cancellationToken);
        Task<ProductIdModel> AddProduct(IDbConnection dbConnection, UserIdentifierModel userIdentifierModel, ProductDetailsModel productDetailsModel, IDbTransaction? dbTransaction, CancellationToken cancellationToken);
        Task<ProductEntity> GetProductById(ProductIdentifierModel productIdentifierModel, CancellationToken cancellationToken);
        Task<IEnumerable<ProductEntity>> GetProducts(UserIdentifierModel username, CancellationToken cancellationToken);
        Task RemoveProductById(ProductIdentifierModel productIdentifierModel, CancellationToken cancellationToken);
        Task RemoveProductPrice(ProductIdentifierModel productIdentifierModel, CancellationToken cancellationToken);
        Task UpdateProduct(ProductIdentifierModel productIdentifierModel, UpdateProductDetailsModel updateProductDetailsModel, CancellationToken cancellationToken);
        Task UpdateProductPrice(ProductIdentifierModel productIdentifierModel, UpdatePriceModel updatePrice, CancellationToken cancellationToken);
    }
}