-- Permissions for Application Access

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = '$(USER_LOGIN)')
BEGIN
    THROW 50000, 'User Doesnt exist, make sure to create User first', 1;
END

USE [$(DATABASE_NAME)] ;

IF NOT EXISTS (
    SELECT 1
    FROM sys.database_principals
    WHERE name = '$(USER_LOGIN)'
)
BEGIN
    CREATE USER [$(USER_LOGIN)] FOR LOGIN [$(USER_LOGIN)];
END

IF '$(READER_ACCESS)' = 'true' AND NOT EXISTS (
    SELECT 1
    FROM sys.database_role_members drm
    JOIN sys.database_principals r ON drm.role_principal_id = r.principal_id
    JOIN sys.database_principals m ON drm.member_principal_id = m.principal_id
    WHERE r.name = 'db_datareader'
        AND m.name = '$(USER_LOGIN)'
)
BEGIN 
    ALTER DATABASE ROLE [db_datareader] ADD MEMBER "$(USER_LOGIN)";
END

IF('$(WRITER_ACCESS)' = 'true' AND NOT EXISTS (
    SELECT 1
    FROM sys.database_role_members drm
    JOIN sys.database_principals r ON drm.role_principal_id = r.principal_id
    JOIN sys.database_principals m ON drm.member_principal_id = m.principal_id
    WHERE r.name = 'db_datawriter'
        AND m.name = '$(USER_LOGIN)'
))
BEGIN
    ALTER DATABASE ROLE [db_datawriter] ADD MEMBER "$(USER_LOGIN)";
END

IF ('$(ENVIRONMENT_NAME)' = 'Development' AND '$(ADMIN_ACCESS)' = 'true' AND NOT EXISTS (
    SELECT 1
    FROM sys.database_role_members drm
    JOIN sys.database_principals r ON drm.role_principal_id = r.principal_id
    JOIN sys.database_principals m ON drm.member_principal_id = m.principal_id
    WHERE r.name = 'db_owner'
      AND m.name = '$(USER_LOGIN)'
))
BEGIN
    ALTER DATABASE ROLE [db_owner] ADD MEMBER "$(USER_LOGIN)";
END




