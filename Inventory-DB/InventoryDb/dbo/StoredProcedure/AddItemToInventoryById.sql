CREATE PROCEDURE [dbo].[AddItemToInventoryById]
	@ItemId INT,
    @Quantity INT
AS
	SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF(@@TRANCOUNT = 0) 
    BEGIN
        DECLARE @TransactionStarted BIT = 1;
        BEGIN TRANSACTION
    END

    BEGIN TRY

        IF EXISTS(SELECT 1 FROM dbo.Inventory WHERE InventoryItemId = @ItemId) 
        BEGIN
            UPDATE dbo.Inventory
            SET Quantity = Quantity + @Quantity
            WHERE InventoryItemId = @ItemId
        END
        ELSE 
        BEGIN
		    INSERT INTO dbo.Inventory(InventoryItemId, Quantity)
		    VALUES(@ItemId, @Quantity)
        END

        IF @@ROWCOUNT = 0
        BEGIN
            IF @TransactionStarted = 1
                ROLLBACK TRANSACTION;
            RETURN -1;  -- No rows affected
        END

        IF(@TransactionStarted = 1)
            COMMIT TRANSACTION;
        RETURN 0;  -- success
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 AND @TransactionStarted = 1 -- Only transaction when child started the transaction
            ROLLBACK TRANSACTION;

        -- Optional: log the error here
        DECLARE @errMessage VARCHAR(100) = 'Error: ' + ERROR_MESSAGE();
        THROW 50001, @errMessage, 1;
    END CATCH
RETURN 0
