CREATE PROCEDURE dbo.GetProductPriceById
    @Username VARCHAR(50),
    @PublicProductId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @UserId INT;

    EXEC dbo.GetActiveUserIdByUsername @Username, @UserId OUTPUT;

    SELECT
           pp.Price,
           pp.CurrencyCode
    FROM dbo.Product p
    JOIN dbo.[User] u 
        ON u.Id = p.Id
    JOIN dbo.ProductPrice pp
        ON p.Id = pp.ProductId
     WHERE p.PublicId = @PublicProductId
           AND u.Id = @UserId
           AND p.[Disabled] = 0;
END