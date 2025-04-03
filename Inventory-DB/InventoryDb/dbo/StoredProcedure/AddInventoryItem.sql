CREATE PROCEDURE [dbo].[AddInventoryItem]
	@Name VARCHAR(100),
	@Description VARCHAR(100),
	@Price DECIMAL(19, 4),
	@Quantity INT,
	@CurrencyCode CHAR(3),
    @NewItemId INT OUTPUT
AS
	SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

		INSERT INTO dbo.InventoryItem([Name], [Description], Price, Quantity, CurrencyCode)
		VALUES(@Name, @Description, @Price, @Quantity, @CurrencyCode)

        -- Return the new ItemId
        SET @NewItemId = SCOPE_IDENTITY();

        COMMIT TRANSACTION;
        RETURN 0;  -- success
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        -- Optional: log the error here

        RETURN ERROR_NUMBER();  -- return the SQL Server error number
    END CATCH
RETURN 0
