#!/bin/sh

services=("inventory-api" "inventory-db" "inventory-ui")

print_usage() {
  cat <<EOF
Usage: $0 -s service -e env -c component
Options:
  -s service   Service name (required)
  -e env       Environment name, default is dev
  -c component Component name (required)
  -h           Show this help message
EOF
}

# Parse flags
while getopts "s:e:c:h" opt; do
  case $opt in
    s) service=$OPTARG ;;
    e) env=$OPTARG ;;
    c) component=$OPTARG ;;
    h) print_usage; exit 0 ;;
    *) print_usage >&2; exit 1 ;;
  esac
done

# Set defaults and check required
env=${env:-dev}
if [[ -z "$service" || ! ${services[@]} =~ $service ]]; then
  echo "Service is missing or invalid. Use -s service" >&2
  exit 1
fi
if [ -z "$component" ]; then
  echo "Component is required. Use -c component" >&2
  exit 1
fi

# Base Directory is the parent of the current directory (deploy-scripts)
# Change if deploy-scripts is moved
cd .. 
current_dir=$(pwd)

ServiceChartPath=${current_dir}/${service}/Chart
infisicalSecretsPath=${current_dir}/Infrastructure/CD/Platform/secrets/infisical

if [ ! -d "$infisicalSecretsPath" ]; then
  echo "Infisical secrets path does not exist: $infisicalSecretsPath"
  exit 1
fi

if [ ! -d "$ServiceChartPath" ]; then
  echo "Service chart path does not exist: $ServiceChartPath"
  exit 1
fi

echo "Paths: ServiceChartPath=${ServiceChartPath}, infisicalSecretsPath=${infisicalSecretsPath}"
echo "Deploying secrets for service=${service}, env=${env}, component=${component}"

#$infisicalSecretsPath/deploy.sh -s $service -e $env -c $component -scp $ServiceChartPath
