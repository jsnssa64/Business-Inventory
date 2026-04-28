#!/bin/bash

set -euxo pipefail

echo "Running CreateServerLogin.sh..."

show_usage() {
    cat <<EOF
Usage: $(basename "$0") [options]
Required options:
    -dbhost, -host, -dh <name>          Target SQL Server instance name
    -set-sysadmin-name, -ssn <name>          Name of the sysadmin login to create
Optional:
    -help, --help                       Show this help message
EOF
}

# Variables
CSL_SYSADMIN_NAME="defaultAdmin"
MASTER_DB_NAME="master"
# Might not be needed
SQL_FILEPATH="./"
SQL_FILENAME="CreateServerLogin.sql"

# Parse named options and support aliases
while [[ $# -gt 0 ]]; do
    case "$1" in
        -set-sysadmin-name|-ssn)
            CSL_SYSADMIN_NAME="$2"
            shift 2
            echo "Sysadmin login name override set to '$CSL_SYSADMIN_NAME'."
            ;;
        -dbhost|-host|-dh)
            DB_HOST="$2"
            shift 2
            echo "Target database host set to '$DB_HOST'."
            ;;
        -help|--help)
            show_usage
            exit 0
            ;;
        *)
            echo "Error: Unknown option '$1'"
            show_usage
            exit 1
            ;;
    esac
done

if [[ -z "$DB_HOST"]] then;
    echo "Error: Database host is required. Use -dbhost, -host, or -dh."
    show_usage
    exit 1
fi

#   Check required environment variables
echo "Checking required environment variables for SQL login creation..."
CURRENT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
envs=(  "MSSQL_SA_USER"
        "MSSQL_SA_PASSWORD" 
        "CSL_SYSADMIN_PASSWORD")
$CURRENT_DIR/CheckEnvVars.sh "${envs[@]}"

# Validate SQL Server connectivity before attempting login creation
echo "Warning: This script requires SQL Server to be running and accessible. Ensure that the SQL Server container is up and running before executing this script."
if ! sqlcmd -C -S "$DB_HOST" -U "$MSSQL_SA_USER" -P "$MSSQL_SA_PASSWORD" -d "$MASTER_DB_NAME" -Q "SELECT 1" > /dev/null 2>&1; then
    echo "Error: Unable to connect to SQL Server. Please ensure SQL Server is running and accessible."
    exit 1
fi

# Run login creation script
echo "Creating Administrator server-level login..."
# /opt/mssql-tools18/bin/sqlcmd \
sqlcmd \
-C \                                            # AUTO TRUST SERVER CERTIFICATE
    -S "$DB_HOST" \
    -U "$MSSQL_SA_USER" \                       # Default sysadmin login created by SQL Server image
    -P "$MSSQL_SA_PASSWORD" \                   # Password for the default sysadmin login
    -d "$MASTER_DB_NAME" \
    -i "$SQL_FILEPATH/$SQL_FILENAME" \          # Script To Be Run
    -v CSL_SYSADMIN_NAME="$CSL_SYSADMIN_NAME"
