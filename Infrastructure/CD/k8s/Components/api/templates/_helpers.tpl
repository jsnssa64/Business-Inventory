{{- define "component-api.appName" -}}
{{- default .Chart.Name .Chart.Component .Values.appName | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{- define "component-api.fullName" -}}
{{- if .Values.fullnameOverride -}}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- printf "%s-%s" (include "component-api.appName" .) .Release.Name | trunc 63 | trimSuffix "-" -}}
{{- end -}}
{{- end -}}

{{- define "component-api.fullChartName" -}}
{{- printf "%s-%s-%s" .Values.Name .Chart.Component .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{- define "component-api.selectorLabels" -}}
    app.kubernetes.io/name: {{ include "component-api.appname" . }}
    app.kubernetes.io/instance: {{ .Release.Name }}
    app.kubernetes.io/type: {{ .Chart.type }}
    app.kubernetes.io/component: {{ .Chart.component }}
{{- end -}}
