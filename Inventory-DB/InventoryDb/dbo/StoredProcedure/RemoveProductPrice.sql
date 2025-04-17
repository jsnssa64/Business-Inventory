CREATE PROCEDURE [dbo].[RemoveProductPrice]
    @Username VARCHAR(100),
    @PublicProductId UNIQUEIDENTIFIER
AS
	SET NOCOUNT ON;
    SET XACT_ABORT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION
            DECLARE @UserId INT;
            DECLARE @ProductId INT;

            -- Get internal ids
            EXEC dbo.GetProductId @Username, @PublicProductId, @ProductId OUTPUT;

            -- Make sure the product exists for this user
            IF (@ProductId IS NULL OR @UserId IS NULL)
                THROW 50002, 'Product not found or does not belong to user.', 1;
            
            DELETE dbo.ProductPrice
            WHERE ProductId = @ProductId;
            
        COMMIT TRANSACTION;
        RETURN 0;  -- success
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 -- Only transaction when child started the transaction
            ROLLBACK TRANSACTION;

        -- Optional: log the error here
        DECLARE @errMessage VARCHAR(100) = 'Error: ' + ERROR_MESSAGE();
        THROW 50001, @errMessage, 1;
    END CATCH
RETURN 0
