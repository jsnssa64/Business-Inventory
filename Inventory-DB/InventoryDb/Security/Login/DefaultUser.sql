IF NOT EXISTS 
    (SELECT name  
     FROM sys.database_principals
     WHERE name = 'alice')
BEGIN
    CREATE USER [alice] FOR LOGIN defaultAdmin;
    ALTER ROLE db_datareader ADD MEMBER [alice];
END
GO