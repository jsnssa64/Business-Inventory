using InventoryApi.Authentication;
using InventoryApi.Controllers.CustomController;
using InventoryApi.DTOs.Product;
using Microsoft.AspNetCore.Mvc;
using Services.DataModel.Product;
using Services.DataModel.User;
using Services.Service.InventoryService;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [MinimumRole(Shared.Constants.Roles.RoleLevel.User)]
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
        public async Task<IActionResult> GetProductById(Guid productid, CancellationToken cancellationToken)
        {
            try
            {
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
                    ProductName = productDTO.Name,
                    Description = productDTO.Description,
                    Quantity = productDTO.ItemQuantity,
                    EnabledPrice = productDTO.EnabledPrice,
                    InventoryQuantity = productDTO.InventoryQuantity,
                };

                var userIdentifierModel = new UserIdentifierModel() {
                    Username = GetUsername()
                };

                //  Do an OR check to error out if only one is null
                var priceModel = (productDTO.Price is not null && productDTO.Currency is not null) ? new PriceModel()
                {
                    Price = (decimal)productDTO.Price,
                    CurrencyCode = productDTO.Currency
                } : null;

                var productId = await _productService.AddProductAsync(userIdentifierModel, productDetailsModel, priceModel);
                return Ok(productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding inventory");
                return StatusCode(500);
            }
        }

        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProducts(CancellationToken cancellationToken)
        {
            try
            {
                var userIdentifierModel = new UserIdentifierModel() { 
                    Username = GetUsername() 
                };

                var productId = await _productService.GetProducts(userIdentifierModel, cancellationToken);
                return Ok(productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding inventory");
                return StatusCode(500);
            }
        }


        [HttpGet("RemoveProduct")]
        public async Task<IActionResult> RemoveProduct(Guid productId, CancellationToken cancellationToken)
        {
            try
            {
                var productIdentifierModel = new ProductIdentifierModel
                {
                    Username = GetUsername(),
                    PublicProductId = productId
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
                    usersProduct.Price = new UpdatePriceModel()
                    {
                        CurrencyCode = updateProductDTO.Price.CurrencyCode,
                        Price = updateProductDTO.Price.Price
                    };
                }

                await _productService.UpdateProduct(productIdentifier, usersProduct, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding inventory");
                return StatusCode(500);
            }
        }
    }
}
