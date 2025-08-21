output "resource_group_id" {
  description = "ID Resource Group w Azure"
  value       = azurerm_resource_group.rg.id
}

output "resource_group_name" {
  value = azurerm_resource_group.rg.name
}

output "webapp_name" {
  value = azurerm_windows_web_app.web_app.name
}

output "key_vault_uri" {
  value = azurerm_key_vault.keyvault.vault_uri
}