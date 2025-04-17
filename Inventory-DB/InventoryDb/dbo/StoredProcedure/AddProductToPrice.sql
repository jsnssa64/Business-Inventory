CREATE PROCEDURE [dbo].[AddProductToPrice]
    @Username VARCHAR(100),
    @PublicProductId UNIQUEIDENTIFIER,
	@Price DECIMAL(19, 4),
	@CurrencyCode CHAR(3)
AS
	SET NOCOUNT ON;
    SET XACT_ABORT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION

        -- Return the new ItemId
        DECLARE @ProductId INT;
        EXEC dbo.GetProductId @PublicProductId, @Username, @ProductId OUTPUT;

        IF NOT EXISTS (SELECT 1 FROM dbo.Product p WHERE p.Id = @ProductId AND p.EnabledPrice = 1)
        BEGIN
            UPDATE dbo.Product
            SET EnabledPrice = 1
            WHERE Id = @ProductId;

            INSERT INTO dbo.ProductPrice (ProductId, CurrencyCode, Price)
            VALUES(@ProductId, @CurrencyCode, @Price)
        END
        ELSE 
        BEGIN
            UPDATE dbo.ProductPrice
            SET 
                CurrencyCode = @CurrencyCode,
                Price = @Price
            WHERE ProductId = @ProductId;
        END

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
