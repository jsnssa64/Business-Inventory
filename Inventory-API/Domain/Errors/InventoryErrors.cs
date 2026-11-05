using BuildingBlocks.Error;

namespace Domain.Errors;

public static class InventoryErrors
{
    public static ResultError NotFound(Guid itemId) 
        => new(KindErr.NotFound, "Inventory.NotFound", $"Inventory Item Not Found: {itemId}");

    public static ResultError QuantityIsAboveReserve(Guid itemId) 
        => new(KindErr.BusinessRule, "Inventory.QuantityIsAboveReserve", $"Quantity of item {itemId} cannot be above the amount in reserve.");
}