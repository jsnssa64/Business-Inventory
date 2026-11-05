using BuildingBlocks.Error;
using Domain.Entities.Product;
using Domain.Interface;
using Domain.ValueObjects.Product;
using Domain.ValueObjects.User;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Services.Service.Product.Dtos;
using Services.Service.Product.Inputs;
using Services.Service.User.Input;
using System.Data;

namespace Services.Service.Product
{
    public class ProductService(
            IProductRepository productRepository,
            IMediator mediator,
            IInventoryRepository inventoryRepository,
            IValidator<CreateProductInput> validatorCreateProductInput,
            IValidator<ProductIdInput> validatorProductIdInput,
            IValidator<UserIdInput> validatorUserIdInput,
            ILogger<ProductService> logger): IProductService
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<ProductService> _logger = logger;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IInventoryRepository _inventoryRepository = inventoryRepository;

        public async Task<Result<ProductDto>> CreateAsync(UserIdInput idInput, CreateProductInput input, CancellationToken cancellationToken)
        {
            // TODO: validate input with IValidator<CreateProductInput> (inject via constructor) and map failures to a validation ResultError
            // TODO: build domain objects (ProductInfo, optional Price) - Price.Create throws; catch-and-convert, or change Price to return Result?
            var validationResult = await validatorCreateProductInput.ValidateAsync(input, cancellationToken);
            if (!validationResult.IsValid)
            {
                // TODO: map validation errors to a validation ResultError
                throw new Exception("Invalid product input");
            }

            var userId = new UserId(idInput.UserId);
            var product = new ProductV1()
            {
                ProductInfo = new ProductInfo(input.Name, input.Description)
            };

            if (input.Price is not null && !string.IsNullOrEmpty(input.Currency))
            {
                product.SetPrice(Price.Create(input.Price.Value, input.Currency));
            }

            var productResult = await _productRepository.AddAsync(userId, product, cancellationToken);


            if (product.IsPriceEnabled())
            {
                var productId = new ProductId()
                {
                    Id = productResult.Id
                };

                await _productRepository.AddPriceToProductAsync(productId, product.Price!, cancellationToken);
            }
            
            return new ProductDto(product.ProductIdentity.PublicProductId.Id);
        }

        public async Task<ProductV1> GetByIdAsync(ProductIdInput idInput, CancellationToken cancellationToken)
        {
            var validationResult = await validatorProductIdInput.ValidateAsync(idInput, cancellationToken);
            if (!validationResult.IsValid)
            {
                // TODO: map validation errors to a validation ResultError
                throw new Exception("Invalid product input");
            }

            var productId = new ProductId(idInput.ProductId);
            var result = await _productRepository.GetByIdAsync(productId, cancellationToken);

            return result;
        }

        public async Task<IEnumerable<ProductV1>> GetProductsAsync(UserIdInput idInput, CancellationToken cancellationToken)
        {
            var validationResult = await validatorUserIdInput.ValidateAsync(idInput, cancellationToken);
            if (!validationResult.IsValid)
            {
                // TODO: map validation errors to a validation ResultError
                throw new Exception("Invalid user input");
            }

            var userId = new UserId(idInput.UserId);
            return await _productRepository.GetProductsAsync(userId, cancellationToken);
        }

        public async Task DeleteAsync(ProductIdInput idInput, CancellationToken cancellationToken)
        {
            var validationResult = await validatorProductIdInput.ValidateAsync(idInput, cancellationToken);
            if (!validationResult.IsValid)
            {
                // TODO: map validation errors to a validation ResultError
                throw new Exception("Invalid product input");
            }

            var productId = new ProductId(idInput.ProductId);
            await _productRepository.DeleteByIdAsync(productId, cancellationToken);
        }

        public async Task UpdateAsync(ProductIdInput idInput, ProductInput productInput, CancellationToken cancellationToken)
        {
            var validationResult = await validatorProductIdInput.ValidateAsync(idInput, cancellationToken);
            if (!validationResult.IsValid)
            {
                // TODO: map validation errors to a validation ResultError
                throw new Exception("Invalid product input");
            }

            var productId = new ProductId(idInput.ProductId);
            var productInfo = new ProductInfo(productInput.Name, productInput.Description);


            var price = (productInput.Price is null) ? null : Price.Create(productInput.Price, productInput.Currency);
            await _productRepository.UpdateAsync(productId, productInfo, cancellationToken);
        }
    }
}
