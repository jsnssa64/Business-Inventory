-- Permissions for sysadmin access

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = '$(USER_LOGIN)')
BEGIN
    THROW 50000, 'User Doesnt exist, make sure to create User first', 1;
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.server_role_members drm
    JOIN sys.server_principals r ON drm.role_principal_id = r.principal_id
    JOIN sys.server_principals m ON drm.member_principal_id = m.principal_id
    WHERE r.name = 'sysadmin'
        AND m.name = '$(USER_LOGIN)'
)
BEGIN 
    ALTER SERVER ROLE [sysadmin] ADD MEMBER "$(USER_LOGIN)";
END