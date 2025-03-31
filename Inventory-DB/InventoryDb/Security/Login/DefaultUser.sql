IF NOT EXISTS 
    (SELECT name  
     FROM master.sys.server_principals
     WHERE name = 'defaultAdmin')
BEGIN
    CREATE LOGIN defaultAdmin WITH PASSWORD = '$(AdminPassword)';
END
GO