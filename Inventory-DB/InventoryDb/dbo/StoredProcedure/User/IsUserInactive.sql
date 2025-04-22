CREATE PROCEDURE dbo.IsUserInactive
    @Username VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UserCount INT;

    SELECT @UserCount = COUNT(*) 
    FROM dbo.[User] 
    WHERE Username = @Username 
        AND [Disabled] = 1;

    IF @UserCount > 1
        THROW 50000, 'Multiple Inactive Users found', 1;

    IF (@UserCount = 1)
        RETURN 1;
    ELSE
        RETURN 0;
END