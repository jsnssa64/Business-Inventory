DECLARE @result INT;
EXEC @result = dbo.TrackFile @fileName = 'SeedData';

IF('$(EnvironmentName)' = 'Development' AND @result = 1)
BEGIN
	DECLARE @InventoryitemId INT;

	INSERT INTO dbo.InventoryItem(Name, Description, Price, CurrencyCode)
	VALUES('Test1', 'Test1', '233', 'GDP')

	SELECT @InventoryitemId = SCOPE_IDENTITY()

	INSERT INTO dbo.Inventory(InventoryItemId, Quantity)
	VALUES(@InventoryitemId, 12)
	PRINT('Complete - FileName:SeedData - $(EnvironmentName)')
END
ELSE 
BEGIN
	PRINT('Skipped - FileName:SeedData - $(EnvironmentName) and TrackFile Status: ' + CAST(@result AS VARCHAR))
END