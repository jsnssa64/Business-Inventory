CREATE PROCEDURE dbo.IsValidUsername
    @Username VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UserId INT;

    EXEC dbo.GetActiveUserIdByUsername @Username, @UserId OUTPUT;
    
    IF @UserId > 0
        RETURN 1
    ELSE 
        RETURN 0
END