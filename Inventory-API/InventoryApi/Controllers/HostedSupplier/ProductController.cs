using InventoryApi.Authentication;
using InventoryApi.Controllers.Base;
using InventoryApi.DTOs.Product;
using Services.Service.InventoryService;
using Microsoft.AspNetCore.Mvc;
using Services.DataModel.Product;
using Services.DataModel.User;
using static Domain.User.Roles;
using Services.Service.ProductService;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route(V)]
    [MinimumRole(RoleLevel.User)]
    public class ProductController(IProductService productService, ILogger<ProductController> logger) : BaseController
    {
        [HttpGet("Product")]
        public async Task<IActionResult> GetProductById(string productid, CancellationToken cancellationToken)
        {
            if(!Guid.TryParse(productid, out var productGuid))
            {
                IfNullArgument();
            }

            var productIdentifierModel = new ProductIdentifierModel()
            {
                Username = GetUsernameFromClaim(),
                PublicProductId = productGuid
            };

            var products = await productService.GetAsync(productIdentifierModel, cancellationToken);

            return Ok(products);
        }

        [HttpPost("Product")]
        public async Task<IActionResult> CreateProduct(ProductDTO productDTO, CancellationToken cancellationToken)
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
                Username = GetUsernameFromClaim()
            };

            //  Do an OR check to error out if only one is null
            var priceModel = (productDTO.Price is not null && productDTO.Currency is not null) ? new PriceModel()
            {
                Price = (decimal)productDTO.Price,
                CurrencyCode = productDTO.Currency
            } : null;

            var productId = await productService.AddAsync(userIdentifierModel, productDetailsModel, priceModel, cancellationToken);
            return Ok(productId);
        }

        [HttpGet("Products")]
        public async Task<IActionResult> GetProducts(CancellationToken cancellationToken)
        {
            var userIdentifierModel = new UserIdentifierModel() { 
                Username = GetUsernameFromClaim() 
            };

            //  TODO: Trying to figure out how to handle errors correctly following rest principles and handling different exceptions
            var productEntities = await productService.GetByUserAsync(userIdentifierModel, cancellationToken);
            return Ok(productEntities);
        }


        [HttpDelete("Product/{productId}")]
        public async Task<IActionResult> DeleteProuct(string productId, CancellationToken cancellationToken)
        {
            if(!Guid.TryParse(productId, out var productGuid))
            {
                IfNullArgument();
            }

            var productIdentifierModel = new ProductIdentifierModel
            {
                Username = GetUsernameFromClaim(),
                PublicProductId = productGuid
            };

            //  TODO: Trying to figure out how to handle errors correctly following rest principles and handling different exceptions
            var result = await productService.DeleteAsync(productIdentifierModel, cancellationToken);
            
            //  Handle Exception Corrrctly??
            if(result.status == failed)
                return BadRequest();
            
            return Ok();
        }

        [HttpPut("Product/{ProductId}")]
        public async Task<IActionResult> UpdateProduct(string ProductId, UpdateProductDTO updateProductDTO, CancellationToken cancellationToken)
        {
            var usersProduct = new UpdateProductDetailsModel()
            {
                Name =          updateProductDTO.ProductName,
                Description =   updateProductDTO.Description,
                Quantity =      updateProductDTO.Quantity
            };

            var productIdentifier = new ProductIdentifierModel()
            {
                Username = GetUsernameFromClaim(),
                PublicProductId = ProductId
            };

            if (updateProductDTO.Price != null) {
                usersProduct.Price = new UpdatePriceModel()
                {
                    CurrencyCode = updateProductDTO.Price.CurrencyCode,
                    Price = updateProductDTO.Price.Price
                };
            }

            //  TODO: Trying to figure out how to handle errors correctly following rest principles and handling different exceptions
            await productService.UpdateAsync(productIdentifier, usersProduct, cancellationToken);
            return Ok();
        }
    }
}
