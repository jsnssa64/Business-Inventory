CREATE PROCEDURE dbo.GetProducts
    @Username VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @UserId INT;

    EXEC dbo.GetActiveUserIdByUsername @Username, @UserId OUTPUT;

    SELECT
        p.PublicId AS PublicProductId,
        p.[Name],
        p.[Description]
    FROM dbo.Product p
    JOIN dbo.[User] u 
    ON u.Id = p.UserId
    WHERE u.Id = @UserId
        AND p.[Disabled] = 0
    OPTION (RECOMPILE);
END