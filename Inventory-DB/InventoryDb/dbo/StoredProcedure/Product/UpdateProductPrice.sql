CREATE PROCEDURE [dbo].[UpdateProductPrice]
    @Username VARCHAR(100),
    @PublicProductId UNIQUEIDENTIFIER,
	@Price DECIMAL(19, 4) = NULL,
    @CurrencyCode CHAR(3) = NULL
AS
	SET NOCOUNT ON;
    SET XACT_ABORT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION
            DECLARE @ProductId INT;
            EXEC dbo.GetProductId @Username, @PublicProductId, @ProductId OUTPUT;

            --  Declare Existing Values
            DECLARE  
                @CurrentPrice BIT,
                @CurrentCurrencyCode BIT;

            SELECT 
                @CurrentPrice = pp.Price,
                @CurrentCurrencyCode = pp.CurrencyCode
            FROM dbo.ProductPrice pp
            WHERE pp.ProductId = @ProductId;
                        
            --  Update Product price
            UPDATE dbo.ProductPrice
            SET
                Price =         ISNULL(@Price, @CurrentPrice),
                CurrencyCode =  ISNULL(@CurrencyCode, @CurrentCurrencyCode)
            WHERE ProductId = @ProductId;
            
            --  Price not updated
            IF(@@ROWCOUNT = 0)
                THROW 50001, 'Unable to update product price', 1;
            
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
