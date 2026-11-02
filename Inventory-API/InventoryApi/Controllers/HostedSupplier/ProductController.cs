using Domain.ValueObjects.Product;
using Domain.ValueObjects.User;
using Domain.ValueObjects.User.Role;
using InventoryApi.Authorization;
using InventoryApi.Controllers.Base;
using InventoryApi.DTOs.Product;
using Microsoft.AspNetCore.Mvc;
using Services.Service.Product;
using Services.Service.Product.Inputs;
using Services.Service.User.Input;

namespace InventoryApi.Controllers.HostedSupplier
{
    [ApiController]
    [Route(V)]
    [MinimumRole(UserRoleLevel.Supplier)]
    public class ProductController(IProductService productService, ILogger<ProductController> logger) : BaseController
    {
        [HttpGet("Product/{productid}")]
        public async Task<IActionResult> GetProductById(string productid, CancellationToken cancellationToken)
        {
            var productIdInput = new ProductIdInput(productid);

            var products = await productService.GetByIdAsync(productIdInput, cancellationToken);

            return Ok(products);
        }

        [HttpPost("Product")]
        public async Task<IActionResult> CreateProduct(CreateProductRequest request, CancellationToken cancellationToken)
        {
            var createProductRequest = new CreateProductInput(request.Name, request.Description, request.Price, request.Currency);
            var productId = await productService.CreateAsync(createProductRequest, cancellationToken);
            return Ok(productId);
        }

        [HttpGet("Products/{productId}")]
        public async Task<IActionResult> GetProducts(string productId, CancellationToken cancellationToken)
        {
            var userIdentity = new UserIdInput(GetUserIdFromClaim());

            //  TODO: Trying to figure out how to handle errors correctly following rest principles and handling different exceptions
            var productEntities = await productService.GetProductsAsync(userIdentity, cancellationToken);
            return Ok(productEntities);
        }


        [HttpDelete("Product/{productId}")]
        public async Task<IActionResult> DeleteProuct(string productId, CancellationToken cancellationToken)
        {
            var productIdInput = new ProductIdInput(productId);

            //  TODO: Trying to figure out how to handle errors correctly following rest principles and handling different exceptions
            await productService.DeleteAsync(productIdInput, cancellationToken);
            
            return Ok();
        }

        [HttpPut("Product/{ProductId}")]
        public async Task<IActionResult> UpdateProduct(string ProductId, UpdateProductRequest updateProductDTO, CancellationToken cancellationToken)
        {
            var productIdInput = new ProductIdInput(ProductId);
            var productInput = new ProductInput(updateProductDTO?.Name, updateProductDTO?.Description, updateProductDTO?.Price, updateProductDTO?.Currency);

            //  TODO: Trying to figure out how to handle errors correctly following rest principles and handling different exceptions
            await productService.UpdateAsync(productIdInput, productInput, cancellationToken);
            return Ok();
        }
    }
}
