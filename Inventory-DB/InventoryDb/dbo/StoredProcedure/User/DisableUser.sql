CREATE PROCEDURE [dbo].[DisableUser]
	@Username VARCHAR(100)
AS
	SET NOCOUNT ON;

    DECLARE @UserId INT;
    -- Get internal ids
    EXEC dbo.GetEnabledUserIdByUsername @Username, @UserId OUTPUT;

	UPDATE dbo.[User]
    SET 
        [Disabled] = 1,
        DisabledAt = GETDATE()
    WHERE Id = @UserId;

RETURN 0
