DECLARE @result INT;
EXEC @result = dbo.TrackFile @fileName = 'SeedData';

IF('$(EnvironmentName)' = 'Development' AND @result = 1)
BEGIN
	DECLARE @ProductId INT;

	INSERT INTO dbo.Product(Name, Description, Price, CurrencyCode)
	VALUES('Test1', 'Test1', '233', 'GDP')

	SELECT @ProductId = SCOPE_IDENTITY()

	INSERT INTO dbo.Inventory(ProductId, Quantity)
	VALUES(@ProductId, 12)
	PRINT('Complete - FileName:SeedData - $(EnvironmentName)')
END
ELSE 
BEGIN
	PRINT('Skipped - FileName:SeedData - $(EnvironmentName) and TrackFile Status: ' + CAST(@result AS VARCHAR))
END