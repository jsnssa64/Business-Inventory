CREATE PROCEDURE dbo.GetDefaultRole
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
           r.[Name] AS RoleName,
           r.[Default] AS IsDefault,
           r.PublicId AS PublicRoleId
    FROM dbo.[Role] r
    WHERE r.[Default] = 1;
END