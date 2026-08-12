#!/bin/sh

networks="api-shared-network data-shared-network backend-shared-network"
existing_networks=$(docker network ls --format "{{.Name}}")

for network in $networks; do
  if ! echo "$existing_networks" | grep -qx "$network"; then
    docker network create "$network"
  fi
done

cd "$(dirname "$0")"

docker-compose \
  -f compose.network.yml \
  -f compose.db.yml \
  -f compose.kurrentdb.yml \
  -f compose.rabbitmq.yml \
  -f compose.api.yml \
  -f compose.ui.yml \
  up -d
