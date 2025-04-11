CREATE PROCEDURE dbo.GetProductById
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        [Name],
        [Description],
        Price,
        CurrencyCode,
        CreatedAt
    FROM dbo.Product
    WHERE Id = @ProductId;
END