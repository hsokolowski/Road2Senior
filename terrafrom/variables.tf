variable "resource_group_name" {
  description = "Nazwa resource group dla całego projektu"
  type        = string
  default     = "hus-dev"
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