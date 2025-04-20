CREATE PROCEDURE dbo.GetInventoryItems
    @Username VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @UserId INT;

    EXEC dbo.GetActiveUserId @Username, @UserId OUTPUT;

    SELECT
           p.PublicId AS PublicProductId,
           p.[Name] AS ProductName,
           i.Quantity AS InventoryQuantity
    FROM dbo.Inventory i
    JOIN dbo.Product p 
    ON i.ProductId = p.Id
    JOIN dbo.[User] u 
    ON p.UserId = u.Id 
    WHERE u.Id = @UserId;
END