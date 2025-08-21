variable "env_prefix" {
  description = "Prefiks środowiska (np. prod, lab, lab1)"
  type        = string
  default     = "lab"
}

variable "location" {
  description = "Region Azure dla zasobów"
  type        = string
  default     = "polandcentral"
}

variable "sql_admin_login" {
  description = "Login administratora SQL Server"
  type        = string
}

variable "sql_admin_password" {
  description = "Hasło administratora SQL Server"
  type        = string
  sensitive   = true
}

variable "tenant_id" {
  description = "Azure Active Directory Tenant ID"
  type        = string
  sensitive   = true
}

variable "azure_devops_pat" {
  description = "Personal Access Token for Azure DevOps (eg. pipelines)"
  type        = string
  sensitive   = true
}

variable "github_service_connection_id" {
  description = "ID connection GitHub in Azure DevOps"
  type        = string
  sensitive   = true
}

variable "github_pat" {
  description = "GitHub Personal Access Token"
  type        = string
  sensitive   = true
}

variable "api_key" {
  description = "API Key to external service(api)"
  type        = string
  sensitive   = true
}

variable "azure_sql_connection_string" {
  description = "Connection string to Azure SQL from Key Vault"
  type        = string
  sensitive   = true
}
