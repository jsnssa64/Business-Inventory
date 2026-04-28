#!/bin/bash

set -euo pipefail

show_usage() {
    cat <<EOF
Usage: $(basename "$0") [options]

Required options:
  -dacpacpath, -dpp <path>                      Path to the DACPAC file
  -dbname, -db <name>                           Target database name
  -dbhost, -h <host>                            Target SQL Server host
Optional:
  -username, -u <username>                      Database user with permissions to deploy the DACPAC
  -password, -p <password>                      Password for the database user
  -envov, -eo, -environment-override <name> ASPNETCORE_ENVIRONMENT value
  -help, --help                             Show this help message
EOF
}

echo "Running DeployDacPac.sh..."

DB_USER="${MSSQL_SYSADMIN_USER:-}"
DB_PASSWORD="${MSSQL_SYSADMIN_PASSWORD:-}"
ENVIRONMENT_NAME="${ASPNETCORE_ENVIRONMENT:-Development}"

# Parse named options and support aliases
while [[ $# -gt 0 ]]; do
    case "$1" in
        -dacpacpath|-dpp)
            DACPAC_PATH="$2"
            shift 2
            ;;
        -dbname|-db|-d)
            DB_NAME="$2"
            shift 2
            ;;
        -dbhost|-h)
            DB_HOST="$2"
            shift 2
            ;;
        -username|-u)
            DB_USER="$2"
            shift 2
            ;;
        -password|-p)
            DB_PASSWORD="$2"
            shift 2
            ;;
        -envov|-eo|-environment-override)
            ENVIRONMENT_NAME="$2"
            shift 2
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

#   Validate required options
missing=0
if [[ -z "$DB_USER" ]]; then
    echo "Error: No database user is undefined and is required. Use -username or -u or define environement variable MSSQL_SYSADMIN_USER."
    missing=1
fi
if [[ -z "$DB_PASSWORD" ]]; then
    echo "Error: Database password is undefined and is required. Use -password or -p or define environment variable MSSQL_SYSADMIN_PASSWORD."
    missing=1
fi
if [[ -z "$DACPAC_PATH" ]]; then
    echo "Error: DACPAC path is undefined and is required. Use -filepath or -fp."
    missing=1
fi
if [[ -z "$DB_NAME" ]]; then
    echo "Error: Database name is undefined and is required. Use -dbname or -db."
    missing=1
fi
if [[ -z "$DB_HOST" ]]; then
    echo "Error: Database host is undefined and is required. Use -dbhost or -h."
    missing=1
fi

if [[ $missing -ne 0 ]]; then
    echo
    show_usage
    exit 1
fi

# Deploy the DACPAC
echo "Deploying dacpac..."
#/opt/sqlpackage/sqlpackage \
if ! sqlpackage \
    /Action:Publish \
    /SourceFile:"$DACPAC_PATH" \
    /TargetServerName:"$DB_HOST" \
    /TargetDatabaseName:"$DB_NAME" \
    /TargetUser:"$DB_USER" \
    /TargetPassword:"$DB_PASSWORD" \
    /TargetTrustServerCertificate:True \
    /v:EnvironmentName="$ENVIRONMENT_NAME"; then
    echo "ERROR: DACPAC deployment failed."
    exit 1
fi

echo "Successfully deployed DACPAC to $DB_HOST for database $DB_NAME in $ENVIRONMENT_NAME environment."