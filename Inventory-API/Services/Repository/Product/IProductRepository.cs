using System.Data;
using Domain.Entities.Product;
using Services.DataModel.Product;
using Services.DataModel.User;

namespace InventoryApi.Repository.Inventory
{
    public interface IProductRepository
    {
        Task AddPriceToProduct(IDbConnection dbConnection, ProductIdentifierModel productIdentifierModel, PriceModel price, IDbTransaction? dbTransaction);
        Task<ProductIdModel> AddProduct(IDbConnection dbConnection, UserIdentifierModel userIdentifierModel, ProductDetailsModel productDetailsModel, IDbTransaction? dbTransaction);
        Task<Product> GetProductById(ProductIdentifierModel productIdentifierModel);
        Task<IEnumerable<Product>> GetProducts(UserIdentifierModel username);
        Task RemoveProductById(ProductIdentifierModel productIdentifierModel);
        Task RemoveProductPrice(ProductIdentifierModel productIdentifierModel);
        Task UpdateProduct(ProductIdentifierModel productIdentifierModel, UpdateProductDetailsModel updateProductDetailsModel);
        Task UpdateProductPrice(ProductIdentifierModel productIdentifierModel, UpdatePriceModel updatePrice);
    }
}