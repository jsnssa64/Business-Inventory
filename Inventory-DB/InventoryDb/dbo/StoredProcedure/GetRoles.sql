CREATE PROCEDURE dbo.GetRoles
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
           r.PublicId AS RolePublicId,
           r.[Name] AS RoleName
    FROM dbo.[Role] r;
END