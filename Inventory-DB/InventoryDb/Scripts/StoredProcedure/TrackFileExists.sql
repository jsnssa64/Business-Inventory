CREATE PROCEDURE [dbo].[TrackFileExists]
	@fileName VARCHAR(100)
AS
	SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 
	    FROM dbo._deployOnce AS do 
	    WHERE do.[FileName] LIKE '%' + @fileName + '%'
    )
        RETURN 1;

    RETURN 0;

	