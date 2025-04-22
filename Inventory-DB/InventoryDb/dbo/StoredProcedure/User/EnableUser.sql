CREATE PROCEDURE [dbo].[EnableUser]
    @Username VARCHAR(50)
AS
	SET NOCOUNT ON;

    DECLARE @UserId INT;
    -- Get internal ids
    EXEC dbo.GetDisabledUserIdByUsername @Username, @UserId OUTPUT;

    UPDATE dbo.[User]
    SET 
        [Disabled] = 0,
        DisabledAt = NULL
    WHERE Id = @UserId;
RETURN 0
