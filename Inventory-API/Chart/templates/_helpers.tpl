{{- define "inventory-api.appName" -}}
{{- default .Chart.Name .Chart.Component .Values.appNameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{- define "inventory-api.fullName" -}}
{{- if .Values.fullnameOverride -}}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- printf "%s-%s" (include "inventory-api.appName" .) .Release.Name | trunc 63 | trimSuffix "-" -}}
{{- end -}}
{{- end -}}

{{- define "inventory-api.fullChartName" -}}
{{- printf "%s-%s-%s" .Chart.Name .Chart.Component .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{- define "inventory-api.selectorLabels" -}}
    app.kubernetes.io/name: {{ include "inventory-api.appname" . }}
    app.kubernetes.io/instance: {{ .Release.Name }}
    app.kubernetes.io/type: {{ .Chart.type }}
    app.kubernetes.io/component: {{ .Chart.component }}
{{- end -}}
