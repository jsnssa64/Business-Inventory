using Domain.Inventory;
using InventoryApi.Repository.Data;
using InventoryApi.Repository.Data.Product;
using InventoryApi.Repository.Data.User;

namespace InventoryApi.Repository.Inventory
{
    public interface IProductRepository
    {
        Task<ProductIdModel> AddProduct(ProductIdentifierModel productIdentifierModel, ProductDetailsModel productDetailsModel, PriceModel? priceModel);
        Task<Product> GetProductById(ProductIdentifierModel productIdentifierModel);
        Task<IEnumerable<Product>> GetProducts(string username);
        Task RemoveProductById(ProductIdentifierModel productIdentifierModel);
        Task<Product> UpdateProduct(ProductIdentifierModel productIdentifierModel, UpdateProductDetailsModel updateProductDetailsModel);
        Task UpdateProductPrice(ProductIdentifierModel productIdentifierModel, PriceModel updatePrice);
    }
}