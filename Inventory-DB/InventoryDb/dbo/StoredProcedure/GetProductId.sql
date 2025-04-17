CREATE PROCEDURE dbo.GetProductId
    @PublicProductId UNIQUEIDENTIFIER,
    @Username VARCHAR(100),
    @ProductId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SET @ProductId = (
    SELECT
           p.Id
      FROM dbo.Product p
      JOIN dbo.[User] u 
      ON p.UserId = u.Id
     WHERE u.Username = @Username 
           AND u.[Disabled] = 0
           AND u.Confirmed = 1
           AND p.PublicId = @PublicProductId
           AND p.[Disabled] = 0);
END