#!/bin/bash

# Start SQL Server in background
/opt/mssql/bin/sqlservr &

# Wait for SQL Server to be up
echo "Waiting for SQL Server to start..."
sleep 20

# Run login creation script
echo "Creating server-level login..."
/opt/mssql-tools18/bin/sqlcmd -C -S localhost -U SA -P "$MSSQL_SA_PASSWORD" -d master -i /tmp/CreateServerLogin.sql

# Deploy the DACPAC
echo "Deploying InventoryDb.dacpac..."
/opt/sqlpackage/sqlpackage \
    /Action:Publish \
    /SourceFile:/usr/src/app/InventoryDb.dacpac \
    /TargetServerName:localhost \
    /TargetDatabaseName:InventoryDb \
    /TargetUser:SA \
    /TargetTrustServerCertificate:True \
    /TargetPassword:$MSSQL_SA_PASSWORD \
    /v:AdminPassword=$DefaultAdmin_Password \
    /v:EnvironmentName=$EnvironmentName \


# Keep the container alive
wait
