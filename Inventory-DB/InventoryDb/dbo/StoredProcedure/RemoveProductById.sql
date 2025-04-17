CREATE PROCEDURE dbo.RemoveProductById
    @PublicProductId UNIQUEIDENTIFIER,
    @Username VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Product
    SET [Disabled] = 1,
        DisabledAt = GETDATE()
    FROM dbo.Product p
    JOIN dbo.[User] u ON p.UserId = u.Id
    WHERE PublicId = @PublicProductId 
           AND u.Username = @Username
           AND u.[Disabled] = 0
           AND u.Confirmed = 1;
END