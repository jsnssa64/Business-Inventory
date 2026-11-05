namespace InventoryApi.DTOs.Product;

public record CreateProductResponse(
    string ProductId
    // TODO: decide what else the client needs back after create (id only, or the created product)
);
