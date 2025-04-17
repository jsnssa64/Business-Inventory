CREATE PROCEDURE [dbo].[CreateRole]
    @RoleName VARCHAR(50),
    @IsDefault BIT = 0,
    @RolePublicId UNIQUEIDENTIFIER OUTPUT
AS
	SET NOCOUNT ON;
    SET XACT_ABORT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION

        INSERT INTO dbo.[Role] ([Name])
        VALUES (@RoleName);
        
        IF(@IsDefault = 1)
        BEGIN
            UPDATE dbo.[Role]
            SET [Default] = 0;
            
            UPDATE dbo.[Role]
            SET [Default] = 1
            WHERE [Name] = @RoleName;
        END

        DECLARE @RoleId INT;
        SET @RoleId = SCOPE_IDENTITY();

        SET @RolePublicId = (SELECT TOP 1 r.PublicId FROM dbo.[Role] r WHERE Id = @RoleId);

        COMMIT TRANSACTION;
        RETURN 0;  -- success
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 -- Only transaction when child started the transaction
            ROLLBACK TRANSACTION;

        -- Optional: log the error here
        DECLARE @errMessage VARCHAR(100) = 'Error: ' + ERROR_MESSAGE();
        THROW 50001, @errMessage, 1;
    END CATCH
RETURN 0
