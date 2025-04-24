CREATE PROCEDURE dbo.GetUserByEmail
    @Email VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @UserId INT;

    EXEC dbo.GetActiveUserIdByEmail @Email, @UserId OUTPUT;

    SELECT
           u.Username,
           u.Email,
           ur.[Role] AS RoleName,
           p.PasswordHash
    FROM dbo.[User] u
    JOIN dbo.UsersRole ur
        ON u.Id =  ur.UserId
    JOIN dbo.[Password] p
        ON u.Id = p.UserId
    WHERE u.Id = @UserId;
END