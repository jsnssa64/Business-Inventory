IF NOT EXISTS 
    (SELECT name  
     FROM sys.database_principals
     WHERE name = 'defaultAdmin')
BEGIN
    CREATE USER defaultAdmin FOR LOGIN defaultAdmin;
    ALTER ROLE db_datareader ADD MEMBER defaultAdmin;
END
GO