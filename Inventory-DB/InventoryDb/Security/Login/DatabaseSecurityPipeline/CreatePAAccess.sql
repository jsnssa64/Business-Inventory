# Permissions for Pipeline Agent Access - Server and Application Permissions
USE [$(DATABASE_NAME)];

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = '$(USER_LOGIN)')
BEGIN
    THROW 50000, 'User Doesnt exist, make sure to create User first', 1;
END


IF NOT EXISTS (
    SELECT 1
    FROM sys.database_principals
    WHERE name = '$(USER_LOGIN)'
)
BEGIN
    CREATE USER [$(USER_LOGIN)] FOR LOGIN [$(USER_LOGIN)];
END

BEGIN TRANSACTION
    BEGIN TRY
        IF NOT EXISTS (
            SELECT 1
            FROM sys.server_role_members drm
            JOIN sys.server_principals r ON drm.role_principal_id = r.principal_id
            JOIN sys.server_principals m ON drm.member_principal_id = m.principal_id
            WHERE r.name = 'dbcreator'
              AND m.name = '$(USER_LOGIN)'
        )
        BEGIN
            ALTER SERVER ROLE [dbcreator] ADD MEMBER "$(USER_LOGIN)";
        END

        IF NOT EXISTS (
            SELECT 1
            FROM sys.database_role_members drm
            JOIN sys.database_principals r ON drm.role_principal_id = r.principal_id
            JOIN sys.database_principals m ON drm.member_principal_id = m.principal_id
            WHERE r.name = 'db_owner'
              AND m.name = '$(USER_LOGIN)'
        )
        BEGIN
            ALTER DATABASE ROLE [db_owner] ADD MEMBER "$(USER_LOGIN)";
        END

        IF NOT EXISTS (
            SELECT 1
            FROM sys.database_role_members drm
            JOIN sys.database_principals r ON drm.role_principal_id = r.principal_id
            JOIN sys.database_principals m ON drm.member_principal_id = m.principal_id
            WHERE r.name = 'db_backupoperator'
              AND m.name = '$(USER_LOGIN)'
        )
        BEGIN
            ALTER DATABASE ROLE [db_backupoperator] ADD MEMBER "$(USER_LOGIN)";
        END

        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH 
        ROLLBACK TRANSACTION
        PRINT 'Error assigning roles to $(USER_LOGIN): ' + ERROR_MESSAGE();
    END CATCH




