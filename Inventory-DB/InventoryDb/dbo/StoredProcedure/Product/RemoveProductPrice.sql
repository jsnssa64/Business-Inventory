CREATE PROCEDURE [dbo].[RemoveProductPrice]
    @Username VARCHAR(100),
    @PublicProductId UNIQUEIDENTIFIER
AS
	SET NOCOUNT ON;
    SET XACT_ABORT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION
            DECLARE @ProductId INT;

            -- Get internal ids
            EXEC dbo.GetProductId @PublicProductId, @Username, @ProductId OUTPUT;

            DELETE dbo.ProductPrice
            WHERE ProductId = @ProductId;
            
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
