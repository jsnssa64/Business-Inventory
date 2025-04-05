IF NOT EXISTS 
    (SELECT 1 
    FROM sys.server_principals 
    WHERE name = 'defaultAdmin')
BEGIN
    PRINT('Creating defaultAdmin login...');
    CREATE LOGIN defaultAdmin WITH PASSWORD = '$(DEFAULT_ADMIN_PASSWORD)';
    ALTER SERVER ROLE sysadmin ADD MEMBER defaultAdmin;
    PRINT('Login defaultAdmin created and added to sysadmin role.');
END
ELSE 
BEGIN
    PRINT('Login defaultAdmin already exists. Skipping creation.');
END
GO