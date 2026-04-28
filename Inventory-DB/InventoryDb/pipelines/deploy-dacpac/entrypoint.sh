#!/bin/bash

set -euxo pipefail

show_usage() {
    cat <<EOF
Usage: $(basename "$0") [options]
Environment variables:
  MSSQL_SYSADMIN_USER: Database user with permissions to deploy the DACPAC (required if -username is not provided)
  MSSQL_SYSADMIN_PASSWORD: Password for the database user (required if -password is not provided)
  ASPNETCORE_ENVIRONMENT: Environment name for the deployment (optional, default: Development)

Required options:
  -dacpacpath, -dpp <path>                      Path to the DACPAC file
  -dbname, -db <name>                           Target database name
  -dbhost, -h <host>                            Target SQL Server host
Optional:
  -create-sysadmin-login, -csl              Flag to create a sysadmin login before deployment
  -dbuser, -du <name>                       Name of the sysadmin (required if MSSQL_SYSADMIN_USER is not set - default: defaultAdmin)
  -dbpassword, -dp <name>                   Password of the sysadmin (required if MSSQL_SYSADMIN_PASSWORD is not set)
  -env, -e <name>                           Environment (required if ASPNETCORE_ENVIRONMENT is not set)
  -help, --help                             Show this help message
EOF
}

# Variables - Development default
DB_HOST="localhost"
DB_NAME="master"

CREATE_SYSADMIN_LOGIN=false

# Environment variables:
echo "Environment variables:"
echo "MSSQL_SYSADMIN_USER: ${MSSQL_SYSADMIN_USER:-'NOT SET'}"
echo "MSSQL_SYSADMIN_PASSWORD: ${MSSQL_SYSADMIN_PASSWORD:-'NOT SET'}"
echo "ASPNETCORE_ENVIRONMENT: ${ASPNETCORE_ENVIRONMENT:-'NOT SET'}"

DB_USER="${MSSQL_SYSADMIN_USER:-defaultAdmin}"
DB_PASSWORD="${MSSQL_SYSADMIN_PASSWORD:-}"
ENVIRONMENT_NAME="${ASPNETCORE_ENVIRONMENT:-Development}"

#   Key/Value Variables
if [[ $# -eq 0 ]]; then
    echo "No arguments provided. Use -help or --help for usage information."
    exit 1
fi

while [[ $# -gt 0 ]]; do
    case "$1" in 
        -create-sysadmin-login|-csl)
            CREATE_SYSADMIN_LOGIN=true
            echo "Sysadmin login creation enabled."
            shift
            ;;
        -dacpacpath|-dpp)
            DACPAC_PATH="$2"
            shift 2
            ;;
        -dbname|-db)
            DB_NAME="$2"
            echo "Target database set to '$DB_NAME'."
            shift 2
            ;;
        -dbhost|-dh)
            DB_HOST="$2"
            echo "Target database host set to '$DB_HOST'."
            shift 2
            ;;
        -dbuser|-du)
            DB_USER="$2"
            echo "Sysadmin login name override set to '$DB_USER'."
            shift 2
            ;;
        -dbpassword|-dp)
            DB_PASSWORD="$2"
            echo "Target database password set to '$DB_PASSWORD'"
            shift 2
            ;;
        -env|-e)
            ENVIRONMENT_NAME="$2"
            echo "Environment is set to '$ENVIRONMENT_NAME'"
            shift 2
            ;;
        -help|--help)
            show_usage
            exit 0
            ;;
        *)
            show_usage
            exit 1
            ;;
    esac
done

# Validate required options
missing=0
if [[ -z "$DB_USER" ]]; then
    echo "Error: No database user is undefined and is required. Use -dbusername or -du or define environement variable MSSQL_SYSADMIN_USER."
    missing=1
fi
if [[ -z "$DB_PASSWORD" ]]; then
    echo "Error: No database password is undefined and is required. Use -dbpassword or -dp or define environement variable MSSQL_SYSADMIN_PASSWORD."
    missing=1
fi
if [[ -z "$ENVIRONMENT_NAME" ]]; then
    echo "Error: No environment is undefined and is required. Use -env or -e or define environement variable ASPNETCORE_ENVIRONMENT."
    missing=1
fi

if [[ $missing -ne 0 ]]; then
    echo
    show_usage
    exit 1
fi

CURRENT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

if [[ "$CREATE_SYSADMIN_LOGIN" == true ]]; then
    echo "Creating sysadmin login '$DB_USER'..."
    "$CURRENT_DIR"/scripts/CreateSysAdminLogin.sh -dbhost "$DB_HOST" -sn "$DB_USER" -sp "$DB_PASSWORD"
    echo "Sysadmin login '$DB_USER' created successfully."
fi

"$CURRENT_DIR"/scripts/DryRunDacPac.sh -dpp "$DACPAC_PATH" -db "$DB_NAME" -h "$DB_HOST" -u "$DB_USER" -p "$DB_PASSWORD" -e "$ENVIRONMENT_NAME"

"$CURRENT_DIR"/scripts/DeployDacPac.sh -dpp "$DACPAC_PATH" -db "$DB_NAME" -h "$DB_HOST" -u "$DB_USER" -p "$DB_PASSWORD" -e "$ENVIRONMENT_NAME"

# Keep the container alive
wait
