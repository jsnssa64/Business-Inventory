CREATE PROCEDURE [dbo].[ResetPassword]
    @Username VARCHAR(100),
    @Password VARBINARY(60) = NULL
AS
	SET NOCOUNT ON;
    SET XACT_ABORT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION
            
            DECLARE @UserId INT;
            -- Get internal ids
            EXEC dbo.GetActiveUserIdByUsername @Username, @UserId OUTPUT;

            --  Disable the users passwords
            UPDATE dbo.[Password]
            SET 
                [Disabled] = 1,
                DisabledAt = GETDATE()
            WHERE UserId = @UserId;

            --  Add Enabled Password
            INSERT INTO dbo.[Password](UserId, PasswordHash, [Disabled])
            VALUES (@UserId, @Password, 0);

        COMMIT TRANSACTION;
        RETURN 0;  -- success
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 -- Only transaction when child started the transaction
            ROLLBACK TRANSACTION;

        -- Optional: log the error here
        DECLARE @errMessage VARCHAR(1200) = 'Error: ' + CAST(ERROR_NUMBER() AS VARCHAR(100)) + ' at line ' + CAST(ERROR_LINE() AS VARCHAR(100)) + ' in ' + ISNULL(ERROR_PROCEDURE(), 'Ad-hoc') + ': ' + ERROR_MESSAGE();
        THROW 50001, @errMessage, 1;
    END CATCH
RETURN 0
