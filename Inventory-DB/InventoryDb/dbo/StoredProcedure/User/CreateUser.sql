CREATE PROCEDURE [dbo].[CreateUser]
	@Username VARCHAR(50),
	@Email VARCHAR(50),
	@FirstName VARCHAR(50),
	@LastName VARCHAR(50),
	@Password VARCHAR(150),
    @UserId INT OUTPUT
AS
	SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

		INSERT INTO dbo.[User](Username, Email)
		VALUES(@Username, @Email)

        SET @UserId = SCOPE_IDENTITY();

        INSERT INTO dbo.UserDetails(UserId, FirstName, LastName)
        VALUES(@UserId, @FirstName, @LastName);

        INSERT INTO dbo.[Password](UserId, PasswordHash)
        VALUES(@UserId, @Password);

        COMMIT TRANSACTION;
        RETURN 0;  -- success
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        DECLARE @errMessage VARCHAR(1200) = 'Error: ' + CAST(ERROR_NUMBER() AS VARCHAR(100)) + ' at line ' + CAST(ERROR_LINE() AS VARCHAR(100)) + ' in ' + ISNULL(ERROR_PROCEDURE(), 'Ad-hoc') + ': ' + ERROR_MESSAGE();
        THROW 50001, @errMessage, 1;
    END CATCH
RETURN 0
