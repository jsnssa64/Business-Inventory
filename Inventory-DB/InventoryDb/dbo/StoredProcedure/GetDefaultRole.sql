CREATE PROCEDURE dbo.GetDefaultRole
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
           r.[Name] AS RoleName
    FROM dbo.[Role] r
    WHERE r.[Default] = 1;
END