CREATE PROCEDURE dbo.GetInventoryItems
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
     WHERE u.Username = @Username
       AND u.[Disabled] = 0
       AND u.Confirmed = 1
       AND i.Quantity > 0;
END