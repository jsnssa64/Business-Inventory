IF NOT EXISTS 
    (SELECT name  
     FROM sys.server_principals
     WHERE name = 'defaultAdmin')
BEGIN
    PRINT('Creating defaultAdmin login...');
    CREATE LOGIN defaultAdmin WITH PASSWORD = '$(DefaultAdmin_Password)';
    ALTER SERVER ROLE sysadmin ADD MEMBER defaultAdmin;
END
GO