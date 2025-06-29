CREATE PROCEDURE dbo.GetPPIDByInternalId -- Public Product Id (PPID)
    @ProductId INT,
    @Username VARCHAR(50),
    @PublicProductId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @UserId INT;

    EXEC dbo.GetActiveUserIdByUsername @Username, @UserId OUTPUT;

    SELECT
        @PublicProductId = p.PublicId
    FROM dbo.Product p
    JOIN dbo.[User] u 
    ON p.UserId = u.Id
    WHERE u.Id = @UserId
        AND p.Id = @ProductId
        AND p.[Disabled] = 0;

    IF @PublicProductId IS NULL 
        THROW 50001, 'Product not found', 1;
END