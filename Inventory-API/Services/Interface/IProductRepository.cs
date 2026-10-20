using System.Data;
using Domain.Entities.Product;

namespace Services.Interface.Product
{
    public interface IProductRepository
    {
        Task AddPriceToProduct(IDbConnection dbConnection, ProductIdentity productIdentifierModel, PriceModel price, IDbTransaction? dbTransaction);
        Task<ProductIdModel> AddProduct(IDbConnection dbConnection, UserIdentifierModel userIdentifierModel, ProductDetailsModel productDetailsModel, IDbTransaction? dbTransaction);
        Task<Product> GetProductById(ProductIdentity productIdentifierModel);
        Task<IEnumerable<Product>> GetProducts(UserIdentifierModel username);
        Task RemoveProductById(ProductIdentity productIdentifierModel);
        Task RemoveProductPrice(ProductIdentity productIdentifierModel);
        Task UpdateProduct(ProductIdentity productIdentifierModel, UpdateProductDetailsModel updateProductDetailsModel);
        Task UpdateProductPrice(ProductIdentity productIdentifierModel, UpdatePriceModel updatePrice);
    }
}