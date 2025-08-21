# terraform {
#   backend "local" {}
# }

terraform {
  backend "azurerm" {
    resource_group_name  = "hus-dev-lab"
    storage_account_name = "tfstatehubertlab001"
    container_name       = "tfstate"
    key                  = "labs/default.tfstate" # realny klucz nadamy w terraform init
  }
}