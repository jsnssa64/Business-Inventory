CREATE PROCEDURE [dbo].[AddItemToInventoryByName]
	@Name VARCHAR(50),
    @Quantity INT
AS
	SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @ItemId INT;
        SET @ItemId = (SELECT TOP(1) Id FROM dbo.InventoryItem WHERE [Name] = @Name)

        IF @ItemId = 1 OR @ItemId IS NULL
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -1 AS StatusCode; -- Item doesn't exist
            RETURN;
        END

        DECLARE @result INT;
        EXEC @result = dbo.AddItemToInventory @ItemId, @Quantity

        IF @result <> 0
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -2 AS StatusCode;  -- Inventory wasnt updated/added
            RETURN;
        END

        COMMIT TRANSACTION;
        SELECT 0 AS StatusCode; 
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @errMessage VARCHAR(100) = 'Error: ' + ERROR_MESSAGE();
        THROW 50001, @errMessage, 1;
    END CATCH
RETURN 0
