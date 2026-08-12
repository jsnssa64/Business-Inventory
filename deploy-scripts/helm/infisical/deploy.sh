#!/bin/sh

print_usage() {
  cat <<EOF
Usage: $0 -s service -e env -c component -p serviceChartPath
Options:
  -s service              Service name (required)
  -e env                  Environment name, default is dev
  -c component            Component name (required)
  -p serviceChartPath     Service chart path (required) e.g. ../${service}/Chart
  -h                      Show this help message
EOF
}

# Parse flags
while getopts "s:e:c:p:h" opt; do
  case $opt in
    s)  service=$OPTARG ;;
    e)  env=$OPTARG ;;
    c)  component=$OPTARG ;;
    p)  serviceChartPath=$OPTARG ;;
    h)  print_usage; exit 0 ;;
    *)  print_usage >&2; exit 1 ;;
  esac
done

# Set defaults and check required
env=${env:-dev}
if [ -z "$service" ]; then
  echo "Service is required. Use -s service" >&2
  exit 1
fi
if [ -z "$component" ]; then
  echo "Component is required. Use -c component" >&2
  exit 1
fi
if [ -z "$serviceChartPath" ]; then
  echo "Service chart path is required. Use -p serviceChartPath" >&2
  exit 1
fi

componentnamespace=${env}-${service}
componentname=${componentnamespace}-${component}

secretsChartDirectory=Infrastructure/CD/Platform/secrets/infisical/
infisicalValuesFile=env/${env}.values.yaml
serviceValuesFile=${serviceChartPath}/env/${env}.values.yaml

#   Install the Infisical Component Helm chart repository
helm repo add infisical-helm-charts 'https://dl.cloudsmith.io/public/infisical/helm-charts/helm/charts/'
# helm repo update

# Deploy Secrets Chart
helm upgrade --install $componentname $secretsChartDirectory \
  -f $serviceValuesFile -f $secretsChartDirectory/$infisicalValuesFile #\
  #--dry-run=client \
  #--debug