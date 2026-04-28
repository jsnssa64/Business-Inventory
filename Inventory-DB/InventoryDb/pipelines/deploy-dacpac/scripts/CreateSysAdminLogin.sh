#!/bin/bash

set -euxo pipefail

echo "Running CreateServerLogin.sh..."

show_usage() {
    cat <<EOF
Usage: $(basename "$0") [options]
Environment Variables:
    MSSQL_SA_USER                           System Administration login Details (Required)
    MSSQL_SA_PASSWORD                       System Administration login Details (Required)

Required options:
    -dbhost, -host, -dh <name>                      Target SQL Server instance name
    -sysadmin-name, -sysname, -sn <name>            Create System Administrator with this username
    -sysadmin-password, -syspassword, -sp <name>    Create System Administrator with this password
Optional:
    -help, --help                           Show this help message
EOF
}

# Variables
SYSADMIN_NAME=""
SYSADMIN_PASSWORD=""
MASTER_DB_NAME="master"
DB_HOST=""
# Might not be needed
SQL_FILENAME="CreateServerLogin.sql"

# Parse named options and support aliases
while [[ $# -gt 0 ]]; do
    case "$1" in
        -sysadmin-name|-sysname|-sn)
            SYSADMIN_NAME="$2"
            shift 2
            echo "Sysadmin login name override set to '$SYSADMIN_NAME'."
            ;;
        -sysadmin-password|-syspassword|-sp)
            SYSADMIN_PASSWORD="$2"
            shift 2
            echo "Sysadmin login password override set to '$SYSADMIN_PASSWORD'."
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

if [[ -z "$SYSADMIN_NAME" ]]; then
    echo "Error: System Administrator username creation is missing. Use -sysadmin-name, -sysname  or -sn"
    show_usage
    exit 1
fi
if [[ -z "$SYSADMIN_PASSWORD" ]]; then
    echo "Error: System Administrator password creation is missing. Use -sysadmin-password, -syspassword  or -sp"
    show_usage
    exit 1
fi
if [[ -z "$DB_HOST" ]]; then
    echo "Error: Database host is required. Use -dbhost, -host, or -dh."
    show_usage
    exit 1
fi

export CSL_SYSADMIN_PASSWORD=$SYSADMIN_PASSWORD

#   Check required environment variables
echo "Checking required environment variables for SQL login creation..."
CURRENT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
envs=(  "MSSQL_SA_USER"
        "MSSQL_SA_PASSWORD"
        "CSL_SYSADMIN_PASSWORD")
$CURRENT_DIR/bash/CheckEnvVars.sh "${envs[@]}"

# Validate SQL Server connectivity before attempting login creation
echo "Warning: This script requires SQL Server to be running and accessible. Ensure that the SQL Server container is up and running before executing this script."
if ! sqlcmd -C -S "$DB_HOST" -U "$MSSQL_SA_USER" -P "$MSSQL_SA_PASSWORD" -d "$MASTER_DB_NAME" -Q "SELECT 1" > /dev/null 2>&1; then
    echo "Error: Unable to connect to SQL Server. Please ensure SQL Server is running and accessible."
    exit 1
fi

# Run login creation script
echo "Creating Administrator server-level login..."
sqlcmd \
-C \
-S "$DB_HOST" \
-U "$MSSQL_SA_USER" \
-P "$MSSQL_SA_PASSWORD" \
-d "$MASTER_DB_NAME" \
-i "$CURRENT_DIR/SQL/$SQL_FILENAME" \
-v CSL_SYSADMIN_NAME="$SYSADMIN_NAME"
