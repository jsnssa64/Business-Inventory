IF OBJECT_ID('dbo.[CheckTableExist]') IS NOT NULL
BEGIN
  DROP FUNCTION dbo.[CheckTableExist]
END
GO

CREATE FUNCTION [dbo].[CheckTableExist]
(
    @fileName VARCHAR(100)
)
RETURNS TABLE
AS
RETURN (
    SELECT 
        t.name AS TableName,
        s.name AS SchemaName
    FROM sys.tables as t
    JOIN sys.schemas s ON t.schema_id = s.schema_id
    WHERE t.name LIKE '%' + @fileName + '%'
);

GO