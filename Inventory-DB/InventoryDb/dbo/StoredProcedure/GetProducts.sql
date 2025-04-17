CREATE PROCEDURE dbo.GetProducts
    @Username VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
           p.PublicId AS ProductId,
           p.[Name],
           p.[Description],
           p.CreatedAt
      FROM dbo.Product p
      JOIN dbo.[User] u 
        ON u.Id = p.Id
     WHERE u.Username = @Username
           AND u.[Disabled] = 0
           AND u.Confirmed = 1
           AND p.[Disabled] = 0;
END