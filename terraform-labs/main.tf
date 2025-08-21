terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }

    azuredevops = {
      source  = "microsoft/azuredevops"
      version = "~> 1.0"
    }
  }
}

provider "azuredevops" {
  org_service_url       = "https://dev.azure.com/hus-dev"
  personal_access_token = var.azure_devops_pat
}

provider "azurerm" {
  features {}
}

# Resource group (prefiksowana)
resource "azurerm_resource_group" "rg" {
  name     = "${var.env_prefix}-rg"
  location = var.location
}

# Service Plan
resource "azurerm_service_plan" "app_service_plan" {
  name                = "${var.env_prefix}-asp"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type             = "Windows"
  sku_name            = "F1"
}

# Web App
resource "azurerm_windows_web_app" "web_app" {
  name                    = "${var.env_prefix}-web"
  location                = azurerm_service_plan.app_service_plan.location
  resource_group_name     = azurerm_resource_group.rg.name
  service_plan_id         = azurerm_service_plan.app_service_plan.id
  https_only              = true
  client_affinity_enabled = true
  ftp_publish_basic_authentication_enabled     = false
  webdeploy_publish_basic_authentication_enabled = false

  app_settings = {
    "WEBSITE_RUN_FROM_PACKAGE"        = "1"
    "WEBSITE_ENABLE_SYNC_UPDATE_SITE" = "true"
  }

  site_config {
    always_on  = false
    ftps_state = "FtpsOnly"

    application_stack {
      dotnet_version = "v8.0"
    }

    virtual_application {
      virtual_path  = "/"
      physical_path = "site\\wwwroot"
      preload       = false
    }

    ip_restriction_default_action     = "Allow"
    scm_ip_restriction_default_action = "Allow"
  }

  identity {
    type = "SystemAssigned"
  }

  logs {
    detailed_error_messages = true
    failed_request_tracing  = false

    http_logs {
      file_system {
        retention_in_days = 2
        retention_in_mb   = 35
      }
    }
  }
}

# Key Vault
resource "azurerm_key_vault" "keyvault" {
  name                      = "${var.env_prefix}kv"
  location                  = var.location
  resource_group_name       = azurerm_resource_group.rg.name
  tenant_id                 = var.tenant_id
  sku_name                  = "standard"
  enable_rbac_authorization = true
  purge_protection_enabled  = false
}

# SQL Server
resource "azurerm_mssql_server" "sql_server" {
  name                         = "${var.env_prefix}-sql"
  location                     = var.location
  resource_group_name          = azurerm_resource_group.rg.name
  version                      = "12.0"
  administrator_login          = var.sql_admin_login
  administrator_login_password = var.sql_admin_password
}

# SQL Database
resource "azurerm_mssql_database" "main_db" {
  name                 = "${var.env_prefix}-db"
  server_id            = azurerm_mssql_server.sql_server.id
  sku_name             = "Basic"
  max_size_gb          = 2
  collation            = "SQL_Latin1_General_CP1_CI_AS"
  zone_redundant       = false
  storage_account_type = "Local"
}
