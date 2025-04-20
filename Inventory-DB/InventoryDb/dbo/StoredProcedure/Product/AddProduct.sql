CREATE PROCEDURE [dbo].[AddProduct]
    @Username VARCHAR(100),
	@ProductName VARCHAR(100),
	@Description VARCHAR(100),
	@Quantity INT,
    @PublicProductId UNIQUEIDENTIFIER OUTPUT
AS
	SET NOCOUNT ON;
    SET XACT_ABORT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION

        DECLARE @UserId INT;

        EXEC dbo.GetActiveUserId @Username, @UserId OUTPUT;

		INSERT INTO dbo.Product([UserId], [Name],  [Description], Quantity)
		VALUES(@UserId, @ProductName, @Description, @Quantity)

        -- Return the new ItemId
        DECLARE @NewProductId INT;
        SET @NewProductId = SCOPE_IDENTITY();

        PRINT 'ProductId' + CAST(ISNULL(@NewProductId, 0) AS VARCHAR(100))
        PRINT 'Username' + CAST(ISNULL(@Username, 0) AS VARCHAR(100))

        EXEC dbo.GetProductByInternalId @NewProductId, @Username, @PublicProductId OUTPUT;
        
        PRINT 'ProductId' + ISNULL(CAST(@PublicProductId AS VARCHAR(100)), '0')

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
