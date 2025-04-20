using Domain.Inventory;
using InventoryApi.Repository.Data;
using InventoryApi.Repository.Data.Product;

namespace InventoryApi.Repository.Inventory
{
    public interface IProductRepository
    {
        Task AddPriceToProduct(ProductIdentifierModel productIdentifierModel, PriceModel price);
        Task<ProductIdModel> AddProduct(UserIdentifierModel productIdentifierModel, ProductDetailsModel productDetailsModel, PriceModel? priceModel);
        Task<Product> GetProductById(ProductIdentifierModel productIdentifierModel);
        Task<IEnumerable<Product>> GetProducts(UserIdentifierModel username);
        Task RemoveProductById(ProductIdentifierModel productIdentifierModel);
        Task RemoveProductPrice(ProductIdentifierModel productIdentifierModel);
        Task UpdatePrice(ProductIdentifierModel productIdentifierModel, UpdatePriceModel updatePrice);
        Task UpdateProduct(ProductIdentifierModel productIdentifierModel, UpdateProductDetailsModel updateProductDetailsModel);
        Task UpdateProductPrice(ProductIdentifierModel productIdentifierModel, UpdatePriceModel updatePrice);
    }
}