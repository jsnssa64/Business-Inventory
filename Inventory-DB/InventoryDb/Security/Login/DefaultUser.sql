IF NOT EXISTS 
    (SELECT name  
     FROM sys.database_principals
     WHERE name = 'defaultAdmin')
BEGIN
    CREATE USER defaultAdmin FOR LOGIN defaultAdmin;
END
GO

-- Add to role only if not already in it
IF NOT EXISTS (
    SELECT dp.name
    FROM sys.database_principals dp
    JOIN sys.database_role_members drm ON dp.principal_id = drm.member_principal_id
    JOIN sys.database_principals roles ON drm.role_principal_id = roles.principal_id
    WHERE dp.name = 'defaultAdmin' AND roles.name = 'db_datareader'
)
BEGIN
    ALTER ROLE db_datareader ADD MEMBER defaultAdmin;
END

GO