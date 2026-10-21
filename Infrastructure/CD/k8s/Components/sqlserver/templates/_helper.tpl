{{/* 
    Define Applications Name with Application based metadata
*/}}
{{- define "application.name" -}}
{{- .Values.application.name | trunc 63 | trimSuffix "-" -}}
{{- end }}

{{/* 
    Define Applications Full Name with Application metadata name and Chart Name
*/}}
{{- define "application.fullname" -}}
{{- printf "%s-%s" .Values.application.name .Chart.name | trunc 63 | trimSuffix "-" }}
{{- end}}

{{/* 
    Define Service Name with Application Full Name in Helper
*/}}
{{- define "application.servicename" -}}
{{- printf "%s-service" application.fullname | trunc 63 | trimSuffix "-" }}
{{- end}}

{{/* 
    Define Chart Full Name with Chart Name and Chart Version
*/}}
{{- define "application.fullchartname" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
    Common labels
    - Chart Full Name
    - App Version 
    - Release Service Name 
    - 
*/}}
{{- define "application.allLabels" }}
{{ include "application.selectorOnlyLabels" . }}
helm.sh/chart: {{ include "application.fullchartname" . }}
app.kubernetes.io/part-of: {{ .Values.application.platform | quote }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
app.kubernetes.io/component: {{ .Chart.component | quote }}
{{- if .Values.application.appVersion }}
app.kubernetes.io/version: {{ .Values.application.appVersion | quote }}
{{- end }}
{{- end}}

{{/*        
    Selector labels
    Key Helm Deployment Identifier
*/}}
{{- define "application.selectorOnlyLabels" -}}
app.kubernetes.io/name: {{ include "application.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}