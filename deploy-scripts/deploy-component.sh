#!/bin/sh

$serviceObj={
  inventory-api: {
    directoryname: inventory-api, # [directoryname]/Chart/env/[env].values.yaml
    component: api  # api/kurrentdb/rabbitmq/sqlserver
  },
  inventory-db: {
    directoryname: inventory-db,
    component: sqlserver
  },
  inventory-ui: {
    directoryname: inventory-ui,
    component: ui
  }
}

print_usage() {
  cat <<EOF
Usage: $0 -s service -e env -c component
Options:
  -s service   Service name (required)
  -e env       Environment name, default is dev
  -c component Component name (required)
  -d           Enable dev mode (dry-run and debug)
  -h           Show this help message
EOF
}

dev_mode=false

# Parse flags
while getopts "s:e:c:d:h" opt; do
  case $opt in
    s) service=$OPTARG ;;
    e) env=$OPTARG ;;
    c) component=$OPTARG ;;
    d) dev_mode=true ;;
    h) print_usage; exit 0 ;;
    *) print_usage >&2; exit 1 ;;
  esac
done

# Set defaults and check required
env=${env:-dev}
if [ -z "$service" || -z $serviceObj[$service] ]; then
  echo "Service is missing or invalid. Use -s service" >&2
  exit 1
fi
if [ -z "$component" ]; then
  echo "Component is required. Use -c component" >&2
  exit 1
fi

$obj = $serviceObj[$service]

$ChartValuePath=${obj.directoryname}/Chart/env
$componentPath=../Infrastructure/CD/k8s/Components/${obj.component}}

componentName=${env}-${service}-${component}

# Deploy the Chart using Helm
# Install if not existing
# create namespace if namespace does not exist
if $dev_mode; then
  helm upgrade --install $componentName $componentPath \
    -f ../$ChartValuePath/$env.values.yaml \
    --dry-run=client \
    --debug
else
  helm upgrade --install $componentName $componentPath \
    -f ../$ChartValuePath/$env.values.yaml
fi


