CREATE PROCEDURE dbo.IsValidUserEmail
    @Email VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UserId INT;

    EXEC dbo.GetActiveUserIdByEmail @Email, @UserId OUTPUT;
    
    IF @UserId > 0
        RETURN 1
    ELSE 
        RETURN 0
END