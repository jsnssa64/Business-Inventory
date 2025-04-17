CREATE PROCEDURE dbo.IsValidRole
    @RolePublicId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.[Role] WHERE PublicId  = @RolePublicId)
        RETURN 1;
    ELSE
        RETURN 0;
END