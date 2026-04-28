#!/bin/bash

set -euo pipefail

echo "Checking required environment variables..."

vars=("$@")
missing=0

for name in "${vars[@]}"; do
  echo "Checking $name..."
  if [[ -z "${!name:-}" ]]; then
    echo "Error: $name environment variable is not set."
    missing=1
  else
    echo "$name is set."
  fi
done

if [[ $missing -ne 0 ]]; then
  echo
  echo "Please set the required environment variables and try again."
  exit 1
fi