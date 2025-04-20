CREATE PROCEDURE dbo.GetProductId
    @PublicProductId UNIQUEIDENTIFIER,
    @Username VARCHAR(100),
    @ProductId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT;
    DECLARE @ProductCount INT;

    EXEC dbo.GetActiveUserId @Username, @UserId OUTPUT;
    
    SELECT @ProductCount = COUNT(*) 
    FROM dbo.Product p
    WHERE p.UserId = @UserId
        AND p.PublicId = @PublicProductId
        AND p.[Disabled] = 0;


    IF @ProductCount > 1
        THROW 50000, 'Multiple Products found.', 1;
    ELSE IF @ProductCount = 0
        THROW 50000, 'Product not found' , 1;

    SELECT
        @ProductId = p.Id
    FROM dbo.Product p
    JOIN dbo.[User] u 
    ON p.UserId = u.Id
    WHERE p.PublicId = @PublicProductId
        AND p.[Disabled] = 0;

    IF @ProductId IS NULL
        THROW 50000, 'Product not found', 1;
END