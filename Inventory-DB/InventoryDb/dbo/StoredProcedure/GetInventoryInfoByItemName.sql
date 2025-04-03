CREATE PROCEDURE dbo.GetInventoryInfoByItemName
    @ItemName VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

  SELECT
         ii.[Name] AS ItemName,
         i.Quantity
    FROM dbo.Inventory i
    JOIN dbo.InventoryItem ii 
      ON i.InventoryItemId = ii.Id
    WHERE ii.[Name] = @ItemName;
END