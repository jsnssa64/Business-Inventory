CREATE PROCEDURE dbo.GetProductById
    @Username VARCHAR(50),
    @PublicProductId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @UserId INT;

    EXEC dbo.GetActiveUserIdByUsername @Username, @UserId OUTPUT;

    SELECT
           p.PublicId AS PublicProductId,
           p.[Name],
           p.[Description],
           p.Quantity,
           p.[Version],
           p.EnabledPrice
      FROM dbo.Product p
      JOIN dbo.[User] u 
        ON u.Id = p.UserId
     WHERE p.PublicId = @PublicProductId
           AND u.Id = @UserId
           AND p.[Disabled] = 0;
END