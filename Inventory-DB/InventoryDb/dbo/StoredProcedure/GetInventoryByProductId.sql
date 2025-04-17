CREATE PROCEDURE dbo.GetInventoryByProductId
    @ProductId VARCHAR(50),
    @Username VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
           p.PublicId AS ProductId,
           p.[Name] AS ProductName,
           i.Quantity AS InventoryQuantity
      FROM dbo.Inventory i
      JOIN dbo.Product p 
        ON i.ProductId = p.Id
      JOIN dbo.[User] u 
        ON p.UserId = u.Id 
     WHERE p.PublicId = @ProductId
       AND u.Username = @Username
       AND u.[Disabled] = 0
       AND u.Confirmed = 1
       AND i.Quantity > 0;
END