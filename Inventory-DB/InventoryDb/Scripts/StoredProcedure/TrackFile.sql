CREATE PROCEDURE [dbo].[TrackFile]
	@fileName VARCHAR(100),
	@newVersion INT = 1
AS
BEGIN TRY
	IF NOT EXISTS (SELECT * FROM dbo._deployOnce as do WHERE do.[FileName] LIKE '%' + @fileName + '%')
	BEGIN 
		INSERT INTO dbo._deployOnce (Id, [FileName])
		VALUES(@newVersion, @fileName)
	END
	ELSE
	BEGIN
		UPDATE dbo._deployOnce
		SET Id = @newVersion
		WHERE Id < @newVersion;

		IF (@@ROWCOUNT = 0)
		BEGIN
			DECLARE @oldVersion INT;

			SELECT TOP 1 @oldVersion = do.Id	
			FROM dbo._deployOnce do	
			WHERE do.[FileName] LIKE '%' + @fileName + '%';
			
			PRINT('Skipped - FileName:' + @fileName + ', Current Version:' + CAST(@oldVersion AS VARCHAR) + ', New Version: ' + CAST(@newVersion AS VARCHAR)) 
			RETURN 0;
		END

		PRINT('Updated - FileName:' + @fileName + ', New Version: ' + CAST(@newVersion AS VARCHAR))
	END
	RETURN 1;
END TRY
BEGIN CATCH
	PRINT('FAILED: Unable to track file, FileName:' + @fileName + ', New Version: ' + CAST(@newVersion AS VARCHAR)) 
	RETURN 0
END CATCH
