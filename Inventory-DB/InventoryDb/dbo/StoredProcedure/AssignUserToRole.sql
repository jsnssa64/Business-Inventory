CREATE PROCEDURE [dbo].[AssignUserToRole]
	@Username VARCHAR(100),
    @RolePublicId VARCHAR(100)
AS
	SET NOCOUNT ON;
    SET XACT_ABORT ON;
    
    DECLARE @TransactionStarted BIT = 0;

    BEGIN TRY
        IF(@@TRANCOUNT = 0) 
        BEGIN
            SET @TransactionStarted = 1;
            BEGIN TRANSACTION
        END

        DECLARE @RoleId INT;
        DECLARE @UserId INT;

        SET @RoleId = (SELECT r1.Id FROM dbo.[Role] r1 WHERE r1.PublicId = @RolePublicId);
        SET @UserId = (SELECT u.Id FROM dbo.[User] u WHERE u.Username = @Username);

        /*  NULL Check, in case of accidental table column missing constraint double check */
        IF @RoleId IS NULL
            THROW 50001, 'Role not found.', 1;

        IF @UserId IS NULL
            THROW 50001, 'User not found.', 1;

        UPDATE dbo.UsersRole
        SET RoleId = @RoleId,
            UserId = @UserId
        WHERE UserId = @UserId;

        IF(@@ROWCOUNT = 0) 
        BEGIN
            INSERT INTO dbo.UsersRole(UserId, RoleId)
            VALUES(@UserId, @RoleId);
        END

        IF(@TransactionStarted = 1)
            COMMIT TRANSACTION;
        RETURN 0;  -- success
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 AND @TransactionStarted = 1 -- Only transaction when child started the transaction
            ROLLBACK TRANSACTION;

        -- Optional: log the error here
        DECLARE @errMessage VARCHAR(100) = 'Error: ' + ERROR_MESSAGE();
        THROW 50001, @errMessage, 1;
    END CATCH
RETURN 0
