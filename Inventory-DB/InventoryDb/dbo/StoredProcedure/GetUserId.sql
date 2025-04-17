CREATE PROCEDURE dbo.GetUserId
    @Username VARCHAR(100),
    @UserId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SET @UserId = (
    SELECT
           u.Id
      FROM dbo.[User] u
     WHERE u.Username = @Username 
           AND u.[Disabled] = 0
           AND u.Confirmed = 1);
END