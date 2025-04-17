DECLARE @result INT;
EXEC @result = dbo.TrackFile @fileName = 'SeedData';

IF('$(EnvironmentName)' = 'Development' AND @result = 1)
BEGIN
	DECLARE @RoleId INT,
		@UserId INT,
		@ProductId INT,
		@InventoryId INT,
		@NewPublicProductId UNIQUEIDENTIFIER,
		@AdminUsername VARCHAR(100) = 'Admin',
		@Role VARCHAR(100) = 'Admin';

	EXEC dbo.CreateRole
		@RoleName = @Role,
		@NewRoleId = @RoleId OUTPUT;
	PRINT('Role: ' + CAST(@RoleId AS VARCHAR(10)) + ' - FileName:SeedData - $(EnvironmentName)')


	EXEC dbo.CreateUser
		@Username = @AdminUsername,
		@Email = 'admin@org.com',
		@FirstName = 'admin',
		@LastName = 'admin',
		@Password = 'admin123',
		@Role = @Role,
		@NewUserId = @UserId OUTPUT;
	PRINT('User: ' + CAST(@UserId AS VARCHAR(10)) + ' - FileName:SeedData - $(EnvironmentName)')

	EXEC dbo.AddProduct
		@Username = @AdminUsername,
		@ProductName = 'Test Product',
		@Description = 'Test Description',
		@Quantity = 1,
		@Price = 10.00,
		@CurrencyCode = 'USD',
		@EnabledPrice = 1,
		@NewPublicProductId = @ProductId OUTPUT;
	PRINT('Product: ' + CAST(@ProductId AS VARCHAR(10)) + ' - FileName:SeedData - $(EnvironmentName)')

	EXEC dbo.AddItemToInventoryByProductId
		@ProductId = @NewPublicProductId,
		@Quantity = 12,
		@NewInventoryId = @InventoryId OUTPUT;
	PRINT('Inventory: ' + CAST(@InventoryId AS VARCHAR(10)) + ' - FileName:SeedData - $(EnvironmentName)')
END
ELSE 
BEGIN
	PRINT('Skipped - FileName:SeedData - $(EnvironmentName) and TrackFile Status: ' + CAST(@result AS VARCHAR))
END