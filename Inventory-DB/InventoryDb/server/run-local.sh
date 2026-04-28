#!/bin/bash

set -euxo pipefail

GET_HOST_IP() {
    getent hosts "$(hostname)".local | awk '{print $1; exit}'
}

./entrypoint.sh -dpp ../bin/Release/InventoryDb.dacpac -dh "$(GET_HOST_IP),1433" -db InventoryDb