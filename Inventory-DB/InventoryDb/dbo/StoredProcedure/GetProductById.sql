CREATE PROCEDURE dbo.GetProductById
    @PublicProductId UNIQUEIDENTIFIER,
    @Username VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
           p.[Name],
           p.[Description],
           p.EnabledPrice
      FROM dbo.Product p
      JOIN dbo.[User] u 
        ON u.Id = p.Id
     WHERE p.PublicId = @PublicProductId
           AND u.Username = @Username
           AND u.[Disabled] = 0
           AND u.Confirmed = 1
           AND p.[Disabled] = 0;
END