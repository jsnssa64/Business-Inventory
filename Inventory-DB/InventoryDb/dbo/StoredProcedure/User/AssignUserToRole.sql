CREATE PROCEDURE [dbo].[AssignUserToRole]
	@Username VARCHAR(100),
    @PublicRoleId UNIQUEIDENTIFIER
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

        EXEC dbo.GetRoleId @PublicRoleId, @RoleId OUTPUT;
        EXEC dbo.GetValidUserId @Username, @UserId OUTPUT;

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
        DECLARE @errMessage VARCHAR(1200) = 'Error: ' + CAST(ERROR_NUMBER() AS VARCHAR(100)) + ' at line ' + CAST(ERROR_LINE() AS VARCHAR(100)) + ' in ' + ISNULL(ERROR_PROCEDURE(), 'Ad-hoc') + ': ' + ERROR_MESSAGE();
        THROW 50001, @errMessage, 1;
    END CATCH
RETURN 0
