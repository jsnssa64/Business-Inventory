namespace Services.Service.Product.Inputs;

public record CreateProductInput(
    // TODO: user identity from claims - typed (UserIdInput) or primitive (string Username)? Rule 6 is unclear for IDs inside a record
    string Name,
    string Description,
    decimal? Price,
    string? Currency
);
