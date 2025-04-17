CREATE PROCEDURE [dbo].[AddProduct]
    @Username VARCHAR(100),
	@ProductName VARCHAR(100),
	@Description VARCHAR(100),
	@Quantity INT,
    @EnabledPrice BIT,
    @PublicProductId UNIQUEIDENTIFIER OUTPUT
AS
	SET NOCOUNT ON;
    SET XACT_ABORT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION

        DECLARE @UserId INT;
        EXEC dbo.GetUserId @Username, @UserId OUTPUT;

		INSERT INTO dbo.Product([UserId], [Name],  [Description], Quantity, EnabledPrice)
		VALUES(@UserId, @ProductName, @Description, @Quantity, @EnabledPrice)

        -- Return the new ItemId
        DECLARE @NewProductId INT;
        SET @PublicProductId = SCOPE_IDENTITY();

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
