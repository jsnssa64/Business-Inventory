namespace InventoryApi.DTOs.Product;

public record CreateProductRequest(
    string Name,
    string Description,
    decimal? Price,
    string? Currency
    // TODO: decide whether ItemQuantity / InventoryQuantity / EnabledPrice (from ProductDTO) belong here or are Stock concerns
);
