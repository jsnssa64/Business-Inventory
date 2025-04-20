CREATE PROCEDURE dbo.GetProductById
    @Username VARCHAR(100),
    @PublicProductId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @UserId INT;

    EXEC dbo.GetActiveUserIdByUsername @Username, @UserId OUTPUT;

    SELECT
           p.[Name],
           p.[Description],
           p.EnabledPrice
      FROM dbo.Product p
      JOIN dbo.[User] u 
        ON u.Id = p.Id
     WHERE p.PublicId = @PublicProductId
           AND u.Id = @UserId
           AND p.[Disabled] = 0;
END