{{- define "application.namespace" -}}
{{printf "%s-%s" .Values.environment .Values.namespace.name | trunc 63 | trimSuffix "-" -}}
{{- end }}