CREATE PROCEDURE [dbo].[TrackFileExists]
	@fileName VARCHAR(100)
AS
	SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 
	    FROM dbo._deployOnce AS d
	    WHERE d.[FileName] LIKE '%' + @fileName + '%'
    )
        RETURN 1;

    RETURN 0;

	