{{- define "gateway-api.fullName" -}}
{{- printf "%s-%s" .Chart.Annotations.owner .Chart.Name | replace "+" "_" | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{- define "gateway-api.namespace" -}}
{{- printf "%s-%s" .Chart.Annotations.owner .Chart.Type | replace "+" "_" | trunc 63 | trimSuffix "-" -}}
{{- end -}}