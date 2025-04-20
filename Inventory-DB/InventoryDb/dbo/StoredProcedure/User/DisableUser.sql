CREATE PROCEDURE [dbo].[DisableUser]
	@Username VARCHAR(100)
AS
	SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

            DECLARE @UserId INT;
            -- Get internal ids
            EXEC dbo.GetActiveUserId @Username, @UserId OUTPUT;

            -- Make sure the product exists for this user
            IF (@UserId IS NULL)
                THROW 50002, 'Product not found or does not belong to user.', 1;

		    UPDATE dbo.[User]
               SET [Disabled] = 1,
                   DisabledAt = GETDATE()
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
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        -- Optional: log the error here
        DECLARE @errMessage VARCHAR(1200) = 'Error: ' + CAST(ERROR_NUMBER() AS VARCHAR(100)) + ' at line ' + CAST(ERROR_LINE() AS VARCHAR(100)) + ' in ' + ISNULL(ERROR_PROCEDURE(), 'Ad-hoc') + ': ' + ERROR_MESSAGE();
        THROW 50001, @errMessage, 1;
    END CATCH
RETURN 0
