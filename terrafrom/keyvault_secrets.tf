resource "azurerm_key_vault_secret" "api_key" {
  name         = "ApiKey"
  value        = var.api_key
  key_vault_id = azurerm_key_vault.keyvault.id
}

resource "azurerm_key_vault_secret" "azure_sql_connection_string" {
  name         = "AzureSql"
  value        = var.azure_sql_connection_string
  key_vault_id = azurerm_key_vault.keyvault.id
}

resource "azurerm_key_vault_secret" "sql_admin_password" {
  name         = "SqlAdminPassword"
  value        = var.sql_admin_password
  key_vault_id = azurerm_key_vault.keyvault.id
}

resource "azurerm_key_vault_secret" "sql_admin_login" {
  name         = "SqlAdminLogin"
  value        = var.sql_admin_login
  key_vault_id = azurerm_key_vault.keyvault.id
}
