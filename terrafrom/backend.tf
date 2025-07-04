# terraform {
#   backend "azurerm" {
#     resource_group_name  = "hus-dev"
#     storage_account_name = "tfstatehubert001"
#     container_name       = "tfstate"
#     key                  = "terraform.tfstate"
#   }
# }

#local
terraform {
  backend "local" {}
}