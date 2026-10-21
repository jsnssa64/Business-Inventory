#!/bin/sh

print_usage() {
  echo "Usage: $0 [-e env] [-h]" >&2
}

env=dev
while getopts "e:h" opt; do
  case $opt in
    e) env=$OPTARG ;;
    h) print_usage; exit 0 ;;
    *) print_usage; exit 1 ;;
  esac
done

component="${env}-global-infisical"

# Deploy Secrets Chart
helm upgrade --install $component ./ \
  -f ./env/$env.values.yaml #\
  #--dry-run=client \
  #--debug