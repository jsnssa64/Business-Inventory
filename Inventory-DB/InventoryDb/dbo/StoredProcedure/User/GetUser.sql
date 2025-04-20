CREATE PROCEDURE dbo.GetUser
    @Username VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @UserId INT;

    EXEC dbo.GetActiveUserId @Username, @UserId OUTPUT;

    SELECT
           u.Username,
           u.Email,
           r.PublicId AS PublicRoleId,
           r.[Name] AS RoleName,
           p.PasswordHash
    FROM dbo.[User] u
    JOIN dbo.UsersRole ur
        ON u.Id =  ur.UserId
    JOIN dbo.[Role] r
        ON ur.RoleId = r.Id
    JOIN dbo.[Password] p
        ON u.Id = p.UserId
    WHERE u.Id = @UserId;
END