CREATE PROCEDURE dbo.ActivateUser
    @Username VARCHAR(100)
AS
	SET NOCOUNT ON;
    SET XACT_ABORT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION
            
            DECLARE @UserId INT;
            -- Get internal id
            EXEC dbo.GetUnconfirmedUserId @Username, @UserId OUTPUT;
                        
            UPDATE dbo.[User]
            SET 
                Confirmed = 1,
                ConfirmedAt = GETDATE()
            WHERE Id = @UserId;

            IF(@@ROWCOUNT = 0)
            BEGIN
                ROLLBACK TRANSACTION;
                RETURN 1;  -- User not found
            END
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
