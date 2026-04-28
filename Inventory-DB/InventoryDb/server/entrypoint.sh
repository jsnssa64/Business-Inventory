#!/bin/bash

set -euxo pipefail

show_usage() {
    cat <<EOF
Usage: $(basename "$0") [options]

Required options:
  -dacpacpath, -dpp <path>                      Path to the DACPAC file
  -dbname, -db <name>                           Target database name
  -dbhost, -h <host>                            Target SQL Server host
Optional:
  -create-sysadmin-login, -csl              Flag to create a sysadmin login before deployment
  -sysadmin-name-override, -sn <name>       Name of the sysadmin login to create (default: defaultAdmin)
  -help, --help                             Show this help message
EOF
}

# Variables - Development default
DB_HOST="localhost"
DB_NAME="master"

CREATE_SYSADMIN_LOGIN=false
SYSADMIN_NAME="defaultAdmin"

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
        -sysadmin-name-override|-sn)
            SYSADMIN_NAME="$2"
            echo "Sysadmin login name override set to '$SYSADMIN_NAME'."
            shift 2
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

if [[ "$CREATE_SYSADMIN_LOGIN" == true ]]; then
    echo "Creating sysadmin login '$SYSADMIN_NAME'..."
    ./scripts/CreateSysAdminLogin.sh -dbhost "$DB_HOST" -sn "$SYSADMIN_NAME"
    echo "Sysadmin login '$SYSADMIN_NAME' created successfully."
fi

./scripts/DryRunDacPac.sh -dpp "$DACPAC_PATH" -db "$DB_NAME" -h "$DB_HOST"

./scripts/DeployDacPac.sh -dpp "$DACPAC_PATH" -db "$DB_NAME" -h "$DB_HOST"

# Keep the container alive
wait
