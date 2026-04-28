# Create Server Login Access

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = '$(USER_LOGIN)')
BEGIN
    CREATE LOGIN "$(USER_LOGIN)" 
    WITH PASSWORD = '$(SECRET_PASSWORD)';
END
ELSE
BEGIN
    PRINT 'Login $(USER_LOGIN) already exists. Skipping creation.';
END