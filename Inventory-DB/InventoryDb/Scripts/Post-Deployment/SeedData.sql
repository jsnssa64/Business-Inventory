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
				@NewPublicRoleId UNIQUEIDENTIFIER,
				@NewUserId INT,
				@NewPublicProductId UNIQUEIDENTIFIER,
				@NewUsername VARCHAR(50) = 'Admin',
				@NewRoleName VARCHAR(50) = 'Admin';

			EXEC dbo.CreateRole
				@RoleName = @NewRoleName,
				@IsDefault = 0,
				@PublicRoleId = @NewPublicRoleId OUTPUT;
			PRINT('Create Role: ' + ISNULL(CAST(@NewPublicRoleId AS VARCHAR(100)), '0')  + ' - FileName:SeedData - $(EnvironmentName)')

			EXEC dbo.CreateUser
				@Username = @NewUsername,
				@Email = 'admin@org.com',
				@FirstName = 'admin',
				@LastName = 'admin',
				@Password = 'admin123',
				@UserId = @NewUserId OUTPUT;
			PRINT('Create User: ' + CAST(ISNULL(@NewUserId, 0) AS VARCHAR(100)) + ' - FileName:SeedData - $(EnvironmentName)')
			
			EXEC dbo.AssignUserToRole @NewUsername, @NewPublicRoleId;
			PRINT('Assign User to Role: Username: ' + CAST(ISNULL(@NewUsername, 0) AS VARCHAR(100)) + '- RoleId: ' + CAST(ISNULL(@NewPublicRoleId, 0) AS VARCHAR(100)) + ' - FileName:SeedData - $(EnvironmentName)')
			
			EXEC dbo.ActivateUser 
				@Username = @NewUsername;
			PRINT('Activate User: ' + CAST(ISNULL(@NewUsername, 0) AS VARCHAR(100)) + ' - FileName:SeedData - $(EnvironmentName)')
			
			DECLARE 
				@Username VARCHAR(100),
				@Email VARCHAR(100),
				@Confirmed BIT,
				@Disabled BIT,
				@Id INT;

			EXEC dbo.AddProduct
				@Username = @NewUsername,
				@ProductName = 'Test Product',
				@Description = 'Test Description',
				@Quantity = 1,
				@EnabledPrice = 1,
				@PublicProductId = @NewPublicProductId OUTPUT;
			PRINT('Add Product: ' + ISNULL(CAST(@NewPublicProductId AS VARCHAR(100)), '0') + ' - FileName:SeedData - $(EnvironmentName)')
			
			EXEC dbo.AddProductToPrice
				@Username = @NewUsername,
				@PublicProductId = @NewPublicProductId,
				@Price = 12.99,
				@CurrencyCode = 'USD'
			PRINT('Add price to product: ' + ISNULL(CAST(@NewPublicProductId AS VARCHAR(100)), '0') + ' - FileName:SeedData - $(EnvironmentName)')


			EXEC dbo.UpdateItemInInventory
				@Username = @NewUsername,
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