DECLARE @result INT;
EXEC @result = dbo.TrackFile @fileName = 'SeedData';

IF('$(EnvironmentName)' = 'Development' AND @result = 1)
BEGIN
	DECLARE @InventoryitemId INT;

	INSERT INTO dbo.InventoryItem(Name, Description)
	VALUES('Test1', 'Test1')

	SELECT @InventoryitemId = SCOPE_IDENTITY()

	INSERT INTO dbo.Inventory(InventoryItemId, Qty)
	VALUES(@InventoryitemId, 12)
END