CREATE PROCEDURE dbo.GetInventoryByProductName
    @ProductName VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

  SELECT
         p.[Name] AS ProductName,
         i.Quantity
    FROM dbo.Inventory i
    JOIN dbo.Product p 
      ON i.ProductId = p.Id
    WHERE p.[Name] = @ProductName;
END