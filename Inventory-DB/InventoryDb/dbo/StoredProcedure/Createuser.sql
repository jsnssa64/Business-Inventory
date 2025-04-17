CREATE PROCEDURE [dbo].[CreateUser]
	@Username VARCHAR(50),
	@Email VARCHAR(50),
	@FirstName VARCHAR(50),
	@LastName VARCHAR(50),
	@Password VARCHAR(150),
	@RolePublicId UNIQUEIDENTIFIER,
    @UserId INT OUTPUT
AS
	SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

		INSERT INTO dbo.[User](UserName, Email)
		VALUES(@Username, @Email)

        SET @UserId = SCOPE_IDENTITY();

        INSERT INTO dbo.UserDetails(UserId, FirstName, LastName)
        VALUES(@UserId, @FirstName, @LastName);

        EXEC dbo.AssignUserToRole @Username, @RolePublicId;

        INSERT INTO dbo.[Password](UserId, PasswordHash)
        VALUES(@UserId, @Password);

        COMMIT TRANSACTION;
        RETURN 0;  -- success
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        -- Optional: log the error here
        DECLARE @errMessage VARCHAR(100) = 'Error: ' + ERROR_MESSAGE();
        THROW 50001, @errMessage, 1;
    END CATCH
RETURN 0
