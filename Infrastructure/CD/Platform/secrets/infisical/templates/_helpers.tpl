{{- define "secrets.componentName"}}
{{- printf "%s-%s" .Values.secret.name .Values.secret.component | trunc 63 | trimSuffix "-" -}}
{{- end }}

{{- define "infisicalConfig.fullNamespace"}}
{{- printf "%s-%s" .Values.secret.infisicalConfig.environment .Values.secret.infisicalConfig.namespace | trunc 63 | trimSuffix "-" -}}
{{- end }}

{{- define "application.fullNamespace"}}
{{- printf "%s-%s" .Values.application.environment .Values.application.namespace | trunc 63 | trimSuffix "-" -}}
{{- end }}