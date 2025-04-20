CREATE PROCEDURE dbo.RemoveProductById
    @Username VARCHAR(100),
    @PublicProductId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UserId INT;

    EXEC dbo.GetActiveUserIdByUsername @Username, @UserId OUTPUT;

    UPDATE dbo.Product
    SET [Disabled] = 1,
        DisabledAt = GETDATE()
    FROM dbo.Product p
    JOIN dbo.[User] u 
    ON p.UserId = u.Id
    WHERE PublicId = @PublicProductId 
           AND u.Id = @UserId;
END