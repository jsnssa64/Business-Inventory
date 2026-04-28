IF NOT EXISTS 
    (SELECT 1
    FROM sys.server_principals
    WHERE name = '$(CSL_SYSADMIN_NAME)')
BEGIN 
    BEGIN TRY
         PRINT('Creating $(CSL_SYSADMIN_NAME) Server Login...');
        CREATE LOGIN [$(CSL_SYSADMIN_NAME)]  WITH PASSWORD = '$(CSL_SYSADMIN_PASSWORD)';
    END TRY 
    BEGIN CATCH
        PRINT 'Create server login for User step failed, continuing...' + ERROR_MESSAGE();
    END CATCH
END
ELSE 
BEGIN
    PRINT('Server Login $(CSL_SYSADMIN_NAME) already exists. Skipping creation.');
END

IF NOT EXISTS 
    (SELECT 1
    FROM sys.database_principals
    WHERE name = '$(CSL_SYSADMIN_NAME)')
BEGIN 
    BEGIN TRY
        CREATE USER [$(CSL_SYSADMIN_NAME)] FOR LOGIN [$(CSL_SYSADMIN_NAME)];
        ALTER SERVER ROLE [sysadmin] ADD MEMBER [$(CSL_SYSADMIN_NAME)];
        PRINT('Database User $(CSL_SYSADMIN_NAME) created and added to sysadmin role.');
    END TRY 
    BEGIN CATCH
        PRINT 'Create database User step failed, continuing...'  + ERROR_MESSAGE();
    END CATCH
END
ELSE 
BEGIN
    PRINT('Database User $(CSL_SYSADMIN_NAME) already exists. Skipping creation.');
END