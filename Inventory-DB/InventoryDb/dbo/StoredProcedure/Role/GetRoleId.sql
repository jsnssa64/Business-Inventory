CREATE PROCEDURE dbo.GetRoleId
    @PublicRoleId UNIQUEIDENTIFIER,
    @RoleId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @RoleCount INT;

    SELECT @RoleCount = COUNT(*) 
    FROM dbo.[Role] r
    WHERE r.PublicId = @PublicRoleId

    IF @RoleCount > 1
        THROW 50000, 'Multiple Roles found.', 1;
    ELSE IF @RoleCount = 0
        THROW 50000, 'Role not found', 1;

    SELECT
        @RoleId = r.Id
    FROM dbo.[Role] r
    WHERE r.PublicId = @PublicRoleId;

    
    IF @RoleId IS NULL
        THROW 50000, 'Role not found', 1;
END