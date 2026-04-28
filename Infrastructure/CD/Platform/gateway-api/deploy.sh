#!/usr/bin/env bash

set -euo pipefail

RELEASE_NAME="platform-gateway-api"
DEV_MODE=false
 
while [[ $# -gt 0 ]]; do 
    case $1 in 
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

# Deploy the Chart using Helm
# Install if not existing
# create namespace if namespace does not exist
if $DEV_MODE; then
    helm upgrade --install --debug --dry-run=client "$RELEASE_NAME" .
    # Want to see the generated manifests without applying them? Use the following command:
    #helm template --debug --dry-run=client "$RELEASE_NAME" .
else
    helm upgrade --install "$RELEASE_NAME" .
fi
