CREATE PROCEDURE [dbo].[AddProductPrice]
    @Username VARCHAR(100),
    @PublicProductId UNIQUEIDENTIFIER,
	@Price DECIMAL(19, 4),
    @CurrencyCode CHAR(3)
AS
	SET NOCOUNT ON;
    SET XACT_ABORT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION
            DECLARE @ProductId INT;
            EXEC dbo.GetProductId @PublicProductId, @Username, @ProductId OUTPUT;

            IF NOT EXISTS(SELECT 1 FROM dbo.Product p WHERE p.Id = @ProductId AND p.EnabledPrice = 1)
            BEGIN
                UPDATE dbo.Product
                SET 
                    EnabledPrice = 1
                WHERE Id = @ProductId
            END

            INSERT INTO dbo.ProductPrice(ProductId, Price, CurrencyCode)
            VALUES(@ProductId, @Price, @CurrencyCode);            
        COMMIT TRANSACTION;
        RETURN 0;  -- success
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 -- Only transaction when child started the transaction
            ROLLBACK TRANSACTION;

        -- Optional: log the error here
        DECLARE @errMessage VARCHAR(1200) = 'Error: ' + CAST(ERROR_NUMBER() AS VARCHAR(100)) + ' at line ' + CAST(ERROR_LINE() AS VARCHAR(100)) + ' in ' + ISNULL(ERROR_PROCEDURE(), 'Ad-hoc') + ': ' + ERROR_MESSAGE();
        THROW 50001, @errMessage, 1;
    END CATCH
RETURN 0
