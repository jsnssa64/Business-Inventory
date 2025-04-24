CREATE PROCEDURE dbo.GetAllUsers
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
           u.Username,
           u.Email,
           ur.[Role] AS RoleName
    FROM dbo.[User] u
    JOIN dbo.UsersRole ur
        ON u.Id =  ur.UserId;
END