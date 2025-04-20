CREATE PROCEDURE dbo.IsValidRole
    @PublicRoleId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.[Role] WHERE PublicId  = @PublicRoleId)
        RETURN 1;
    ELSE
        RETURN 0;
END