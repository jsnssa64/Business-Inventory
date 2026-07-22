#!/bin/bash

set -euxo pipefail

GET_HOST_IP() {
    getent hosts "$(hostname)".local | awk '{print $1; exit}'
} 

export MSSQL_SA_USER="sa"
export MSSQL_SA_PASSWORD="Sampaio4240"
export CSL_SYSADMIN_PASSWORD=""

export MSSQL_SYSADMIN_USER="customAdmin"
export MSSQL_SYSADMIN_PASSWORD="Sampaio4240"
export ASPNETCORE_ENVIRONMENT="Development"

./deploy-dacpac/entrypoint.sh -dpp ../bin/Release/InventoryDb.dacpac -dh "$(GET_HOST_IP),1433" -db InventoryDb -csl