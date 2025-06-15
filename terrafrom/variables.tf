variable "resource_group_name" {
  description = "Nazwa resource group dla całego projektu"
  type        = string
  default     = "hus-dev2"
}

variable "location" {
  description = "Region Azure dla zasobów"
  type        = string
  default     = "westeurope"
}

variable "sql_admin_login" {
  description = "Login administratora SQL Server"
  type        = string
}

variable "sql_admin_password" {
  description = "Hasło administratora SQL Server"
  type        = string
  sensitive   = true  # oznaczenie jako dane wrażliwe
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