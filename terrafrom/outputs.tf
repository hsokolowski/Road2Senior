output "resource_group_id" {
  description = "ID Resource Group w Azure"
  value       = azurerm_resource_group.rg.id
}

output "github_service_connection_id" {
  value = data.azuredevops_serviceendpoint_github.github.id
}
