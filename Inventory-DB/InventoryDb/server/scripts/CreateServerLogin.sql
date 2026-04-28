IF NOT EXISTS 
    (SELECT 1 
    FROM sys.server_principals 
    WHERE name = '$(CSL_SYSADMIN_NAME)'
    )
BEGIN
    PRINT('Creating $(CSL_SYSADMIN_NAME) login...');
    CREATE LOGIN [$(CSL_SYSADMIN_NAME)]  WITH PASSWORD = '$(CSL_SYSADMIN_PASSWORD)';
    ALTER SERVER ROLE [$(CSL_SYSADMIN_NAME)] ADD MEMBER [$(CSL_SYSADMIN_NAME)];
    PRINT('Login $(CSL_SYSADMIN_NAME) created and added to sysadmin role.');
END
ELSE 
BEGIN
    PRINT('Login $(CSL_SYSADMIN_NAME) already exists. Skipping creation.');
END
GO