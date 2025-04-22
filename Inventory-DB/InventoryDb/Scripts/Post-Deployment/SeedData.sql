IF('$(EnvironmentName)' = 'Development')
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION
			DECLARE @result INT;
			DECLARE @trackFileExists INT;
			DECLARE @fileName VARCHAR(100) = 'SeedData'

			EXEC @trackFileExists = dbo.TrackFileExists @fileName;

			IF(@trackFileExists = 1)
			BEGIN
				PRINT('Skipped - FileName:SeedData - $(EnvironmentName) and TrackFile Status: ' + CAST(@result AS VARCHAR));
				RETURN;
			END

			DECLARE 
				@NewRoleName VARCHAR(50) = 'Admin',
				@NewUserId INT,
				@NewPublicProductId UNIQUEIDENTIFIER,
				@NewAdminUsername VARCHAR(50) = 'Admin',
				@NewAdminRoleName VARCHAR(50) = 'Admin',
				@NewUserRoleName VARCHAR(50) = 'User';

			EXEC dbo.CreateUser
				@Username = @NewAdminUsername,
				@Email = 'admin@org.com',
				@FirstName = 'admin',
				@LastName = 'admin',
				@Password = '$2b$10$P9ATvoejRSnFgEcsqpqDFOO0FkiZpL85FZ4Jbm2/yo4mYqxZMW7RK', -- Admin123
				@UserId = @NewUserId OUTPUT;
			PRINT('Create User: ' + CAST(ISNULL(@NewUserId, 0) AS VARCHAR(100)) + ' - FileName:SeedData - $(EnvironmentName)')
			
			EXEC dbo.AssignUserToRole @NewAdminUsername, @NewRoleName;
			PRINT('Assign User to Role: Username: ' + CAST(ISNULL(@NewAdminUsername, 0) AS VARCHAR(100)) + '- RoleId: ' + ISNULL(CAST(@NewRoleName AS VARCHAR(100)), '0') + ' - FileName:SeedData - $(EnvironmentName)')
			
			EXEC dbo.ActivateUser 
				@Username = @NewAdminUsername;
			PRINT('Activate User: ' + CAST(ISNULL(@NewAdminUsername, 0) AS VARCHAR(100)) + ' - FileName:SeedData - $(EnvironmentName)')
			
			DECLARE 
				@Username VARCHAR(50),
				@Email VARCHAR(50),
				@Confirmed BIT,
				@Disabled BIT,
				@Id INT;

			EXEC dbo.AddProduct
				@Username = @NewAdminUsername,
				@ProductName = 'Test Product',
				@Description = 'Test Description',
				@Quantity = 1,
				@PublicProductId = @NewPublicProductId OUTPUT;
			PRINT('Add Product: ' + ISNULL(CAST(@NewPublicProductId AS VARCHAR(100)), '0') + ' - FileName:SeedData - $(EnvironmentName)')
			
			EXEC dbo.AddProductPrice
				@Username = @NewAdminUsername,
				@PublicProductId = @NewPublicProductId,
				@Price = 12.99,
				@CurrencyCode = 'USD'
			PRINT('Add price to product: ' + ISNULL(CAST(@NewPublicProductId AS VARCHAR(100)), '0') + ' - FileName:SeedData - $(EnvironmentName)')


			EXEC dbo.UpdateItemInInventory
				@Username = @NewAdminUsername,
				@PublicProductId = @NewPublicProductId,
				@Quantity = 12
			PRINT('Update Inventory: ' + ISNULL(CAST(@NewPublicProductId AS VARCHAR(100)), '0') + ' - FileName:SeedData - $(EnvironmentName)')
		
			EXEC dbo.TrackFile @fileName;
		
		COMMIT TRANSACTION
	END TRY 
	BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
		
		DECLARE @errMessage VARCHAR(1200) = 'Error: ' + CAST(ERROR_NUMBER() AS VARCHAR(100)) + ' at line ' + CAST(ERROR_LINE() AS VARCHAR(100)) + ' in ' + ISNULL(ERROR_PROCEDURE(), 'Ad-hoc') + ': ' + ERROR_MESSAGE();
        THROW 50001, @errMessage, 1;
	END CATCH
END