CREATE PROCEDURE dbo.GetInventoryItemByItemId
    @ItemId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id AS ItemId,
        [Name],
        [Description],
        Price,
        CurrencyCode,
        CreatedAt
    FROM dbo.InventoryItem
    WHERE Id = @ItemId;
END