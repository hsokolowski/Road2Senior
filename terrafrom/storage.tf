# Tworzy Storage Account na plik terraform.tfstate w RG "hus-dev"
resource "azurerm_storage_account" "tf_backend_sa" {
  name                     = "tfstatehubert001"              # musi być unikalna w całym Azure
  resource_group_name      = azurerm_resource_group.rg.name   # czyli hus-dev
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  #allow_blob_public_access = false #nie działa
}

resource "azurerm_storage_container" "tf_backend_container" {
  name                  = "tfstate"
  storage_account_name  = azurerm_storage_account.tf_backend_sa.name
  container_access_type = "private"
}
