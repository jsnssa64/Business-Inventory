namespace Services.Service.Product.Dtos;

public record ProductDto(
    string ProductId
    // TODO: decide whether create returns the full product or id only - if id only, reconsider the name (rule 7: <Noun>Dto)
);
