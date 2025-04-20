CREATE PROCEDURE [dbo].[UpdateProduct]
    @Username VARCHAR(100),
    @PublicProductId UNIQUEIDENTIFIER,
	@Name VARCHAR(100) = NULL,
	@Description VARCHAR(100) = NULL,
	@Quantity INT = NULL,
    @EnabledPrice BIT = NULL
AS
	SET NOCOUNT ON;
    SET XACT_ABORT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION
            DECLARE @UserId INT;
            DECLARE @ProductId INT;

            -- Get internal ids
            EXEC dbo.GetActiveUserIdByUsername @Username, @UserId OUTPUT;
            EXEC dbo.GetProductId @Username, @PublicProductId, @ProductId OUTPUT;

            -- Make sure the product exists for this user
            IF (@ProductId IS NULL OR @UserId IS NULL)
                THROW 50002, 'Product not found or does not belong to user.', 1;
            
            --  Declare Existing Values
            DECLARE @CurrentName VARCHAR(100), 
                @CurrentDesc VARCHAR(100), 
                @CurrentQty INT, 
                @CurrentPrice BIT,
                @CurrentCurrencyCode BIT,
                @CurrentEnabledPrice BIT;

            --  Populate Existing Values
            SELECT 
                @CurrentName = [Name],
                @CurrentDesc = [Description],
                @CurrentQty = Quantity,
                @CurrentEnabledPrice = EnabledPrice
            FROM dbo.Product p
            WHERE p.Id = @ProductId;

            SELECT 
                @CurrentPrice = pp.Price,
                @CurrentCurrencyCode = pp.CurrencyCode
            FROM dbo.ProductPrice pp
            WHERE pp.ProductId = @ProductId;

            --  Update Product
            UPDATE dbo.Product
            SET 
                [Name] = ISNULL(@Name, @CurrentName),
                [Description] = ISNULL(@Description, @CurrentDesc),
                Quantity = ISNULL(@Quantity, @CurrentQty),
                EnabledPrice = ISNULL(@EnabledPrice, @CurrentEnabledPrice)
            WHERE Id = @ProductId 
                AND UserId = @UserId
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
