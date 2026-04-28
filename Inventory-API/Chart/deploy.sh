#!/usr/bin/env bash

set -euo pipefail

# CUSTOM
NAMESPACE="inventory-api"
PRODUCTION_ENVIRONMENT="prod"
STAGING_ENVIRONMENT="staging"
DEVELOPMENT_ENVIRONMENT="dev"
# END CUSTOM

DEV_MODE=false

 
while [[ $# -gt 0 ]]; do 
    case $1 in 
        # OPTIONAL FLAGS
        --environment|--env)
            ENVIRONMENT="$2"
            shift 2 # Remove the flag and its value from the arguments
            ;;
        --release-name|-r)
            RELEASE_NAME="$2"
            shift 2 # Remove the flag and its value from the arguments
            ;;
        --namespace-override|-n)
            NAMESPACE="$2"
            shift 2 # Remove the flag and its value from the arguments
            ;;
        #   BOOLEAN FLAGS
        --dev-mode|-dev)
            DEV_MODE=true
            shift # Remove the flag from the arguments
            ;;
        *)
        shift # Remove other arguments
        ;;
    esac
done

case "$3" in 
    Production)
        ENVIRONMENT=PRODUCTION_ENVIRONMENT
        ;;
    Staging)
        ENVIRONMENT=STAGING_ENVIRONMENT
        ;;
    *)
        ENVIRONMENT=DEVELOPMENT_ENVIRONMENT
        ;;
esac

VALUES_FILE="env/$ENVIRONMENT.values.yaml"

if[[ ! -f "$VALUES_FILE" ]]; then
    echo "Values file for environment '$ENVIRONMENT' not found at path: $VALUES_FILE"
    exit 1
fi

# Deploy the Chart using Helm
# Install if not existing
# create namespace if namespace does not exist
if $DEV_MODE; then
    helm upgrade --install --dry-run "$RELEASE_NAME" . --namespace "$NAMESPACE" --create-namespace -f "$VALUES_FILE"
else
    helm upgrade --install "$RELEASE_NAME" . --namespace "$NAMESPACE" --create-namespace -f "$VALUES_FILE"
fi

