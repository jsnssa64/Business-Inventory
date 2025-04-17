using InventoryApi.Controllers.CustomController;
using InventoryApi.Model.DTO.Product;
using InventoryApi.Repository.Data;
using InventoryApi.Repository.Data.Product;
using InventoryApi.Repository.Data.User;
using InventoryApi.Service.InventoryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace InventoryApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet("GetProductById")]
        public async Task<IActionResult> GetProductById(string productid, CancellationToken cancellationToken)
        {
            try
            {
                if (productid.IsNullOrEmpty())
                {
                    throw new ArgumentNullException(nameof(productid));
                }

                var productIdentifierModel = new ProductIdentifierModel()
                {
                    Username = GetUsername(),
                    PublicProductId = productid
                };

                var products = await _productService.GetProductByIdAsync(productIdentifierModel, cancellationToken);

                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding inventory");
                return StatusCode(500);
            }
        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct(ProductDTO productDTO, CancellationToken cancellationToken)
        {
            try
            {
                var productDetailsModel = new ProductDetailsModel
                {
                    Name = productDTO.Name,
                    Description = productDTO.Description,
                    Quantity = productDTO.ItemQuantity,
                    EnabledPrice = productDTO.EnabledPrice,
                    InventoryQuantity = productDTO.InventoryQuantity,
                };

                var productIdentifier = new ProductIdentifierModel() {
                    PublicProductId = "",
                    Username = GetUsername()
                };

                var priceModel = new PriceModel()
                {
                    Price = productDTO.Price,
                    CurrencyCode = productDTO.Currency
                };

                var productId = await _productService.AddProductAsync(productIdentifier, productDetailsModel, priceModel);
                return Ok(productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding inventory");
                return StatusCode(500);
            }
        }

        [HttpPost("GetProducts")]
        public async Task<IActionResult> GetProducts(CancellationToken cancellationToken)
        {
            try
            {

                var productId = await _productService.GetProducts(GetUsername(), cancellationToken);
                return Ok(productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding inventory");
                return StatusCode(500);
            }
        }


        [HttpPost("RemoveProduct")]
        public async Task<IActionResult> RemoveProduct(ProductIdDTO productDTO, CancellationToken cancellationToken)
        {
            try
            {
                var productIdentifierModel = new ProductIdentifierModel
                {
                    Username = GetUsername(),
                    PublicProductId = productDTO.ProductId
                };

                await _productService.RemoveProduct(productIdentifierModel, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding inventory");
                return StatusCode(500);
            }
        }

        [HttpPost("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(UpdateProductDTO updateProductDTO, CancellationToken cancellationToken)
        {
            try
            {
                var usersProduct = new UpdateProductDetailsModel()
                {
                    Name =          updateProductDTO.ProductName,
                    Description =   updateProductDTO.Description,
                    Quantity =      updateProductDTO.Quantity
                };

                var productIdentifier = new ProductIdentifierModel()
                {
                    Username = GetUsername(),
                    PublicProductId = updateProductDTO.ProductId
                };

                if (updateProductDTO.Price != null) {
                    usersProduct.Price = new PriceModel()
                    {
                        CurrencyCode = updateProductDTO.Price.CurrencyCode,
                        Price = updateProductDTO.Price.Price
                    };
                }

                var productId = await _productService.UpdateProduct(productIdentifier, usersProduct, cancellationToken);
                return Ok(productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding inventory");
                return StatusCode(500);
            }
        }
    }
}
