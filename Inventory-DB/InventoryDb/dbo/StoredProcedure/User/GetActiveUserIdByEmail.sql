CREATE PROCEDURE dbo.GetActiveUserIdByEmail
    @Email VARCHAR(100),
    @UserId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UserCount INT;

    SELECT @UserCount = COUNT(*) 
    FROM dbo.[User] 
    WHERE Email = @Email 
        AND [Disabled] = 0 AND Confirmed = 1

    IF @UserCount > 1
        THROW 50000, 'Multiple users found with that username.', 1;
    ELSE IF @UserCount = 0
        THROW 50000, 'User not found', 1;

    SELECT
            @UserId = u.Id
        FROM dbo.[User] u
        WHERE u.Email = @Email 
            AND u.[Disabled] = 0
            AND u.Confirmed = 1;

    IF @UserId IS NULL 
        THROW 50000, 'User not found', 1;
END