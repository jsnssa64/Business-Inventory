CREATE PROCEDURE [dbo].[UpdateItemInInventory]
    @Username VARCHAR(100),
	@PublicProductId UNIQUEIDENTIFIER,
    @Quantity INT = 0
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
        
        DECLARE @ProductId INT;
        EXEC dbo.GetProductId @PublicProductId, @Username, @ProductId OUTPUT;

        UPDATE dbo.Inventory
           SET Quantity = Quantity + @Quantity
         WHERE ProductId = @ProductId

        IF(@@ROWCOUNT = 0)
        BEGIN
            IF(@Quantity <= 0)
                SET @Quantity = 0;

		    INSERT INTO dbo.Inventory(ProductId, Quantity)
		    VALUES(@ProductId, @Quantity)
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
