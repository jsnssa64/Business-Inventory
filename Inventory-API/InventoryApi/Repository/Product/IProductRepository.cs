using System.Data;
using Domain.Inventory;
using InventoryApi.Repository.Data;
using InventoryApi.Repository.Data.Product;

namespace InventoryApi.Repository.Inventory
{
    public interface IProductRepository
    {
        Task AddPriceToProduct(IDbConnection dbConnection, ProductIdentifierModel productIdentifierModel, PriceModel price, IDbTransaction? dbTransaction);
        Task<ProductIdModel> AddProduct(IDbConnection dbConnection, UserIdentifierModel userIdentifierModel, ProductDetailsModel productDetailsModel, IDbTransaction? dbTransaction);
        Task<Product> GetProductById(ProductIdentifierModel productIdentifierModel);
        Task<IEnumerable<ProductBase>> GetProducts(UserIdentifierModel username);
        Task RemoveProductById(ProductIdentifierModel productIdentifierModel);
        Task RemoveProductPrice(ProductIdentifierModel productIdentifierModel);
        Task UpdateProduct(ProductIdentifierModel productIdentifierModel, UpdateProductDetailsModel updateProductDetailsModel);
        Task UpdateProductPrice(ProductIdentifierModel productIdentifierModel, UpdatePriceModel updatePrice);
    }
}