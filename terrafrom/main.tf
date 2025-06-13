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

# Konfiguracja dostawcy Azure
provider "azurerm" {
  features {}
}

# 1. Projekt DevOps (jeśli chcesz zarządzać nim przez TF) (konto hus nie ma uprawnien)
# resource "azuredevops_project" "project" {
#   name               = "hus"
#   description        = "Projekt do nauki Road2Senior"
#   visibility         = "private"
#   version_control    = "Git"
#   work_item_template = "Agile"
# }

# 2. Połączenie z GitHub (jako service endpoint)
data "azuredevops_serviceendpoint_github" "github" {
  project_id = data.azuredevops_project.project.id
  service_endpoint_name = "GitHubConnection"
}

# Resource group
resource "azurerm_resource_group" "rg" {
  name     = var.resource_group_name
  location = var.location
}

resource "azurerm_service_plan" "app_service_plan" {
  name                = "ASP-husdev-9c57"        # nazwa istniejącego planu z portala weź
  location            = "polandcentral"
  resource_group_name = azurerm_resource_group.rg.name
  os_type             = "Windows"
  sku_name            = "F1"                     # Free Tier z portala rowniez - wejsc do app-web i tam na dole po prawej stronie bedzie
}

#Web app
resource "azurerm_windows_web_app" "web_app" {
  name                = "apifootball-web"
  location            = azurerm_service_plan.app_service_plan.location
  resource_group_name = azurerm_resource_group.rg.name
  service_plan_id     = azurerm_service_plan.app_service_plan.id
  https_only          = true
  client_affinity_enabled = true
  ftp_publish_basic_authentication_enabled = false
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

# --- Szczegółowe objaśnienia poszczególnych ustawień ---

# name: Nazwa aplikacji widoczna w Azure
# location: Region Azure, w którym znajduje się Web App
# resource_group_name: Grupa zasobów, do której przypisana jest aplikacja
# service_plan_id: Identyfikator App Service Planu używanego przez aplikację
# https_only: Wymusza połączenie HTTPS (wzmacnia bezpieczeństwo)
# client_affinity_enabled: Użytkownik kierowany jest na ten sam serwer (istotne przy wielu instancjach)
# ftp_publish_basic_authentication_enabled: Włączenie autoryzacji FTP (domyślnie wyłączone)
# webdeploy_publish_basic_authentication_enabled: Włączenie autoryzacji WebDeploy (domyślnie wyłączone)

# app_settings:
# WEBSITE_RUN_FROM_PACKAGE: Uruchamianie aplikacji bezpośrednio z paczki ZIP
# WEBSITE_ENABLE_SYNC_UPDATE_SITE: Synchronizacja aktualizacji aplikacji

# site_config:
# always_on: Utrzymuje aplikację cały czas aktywną (dla Free Tier musi być false)
# ftps_state: Ustawienie FTPS (bardziej bezpieczna metoda FTP)

# application_stack:
# dotnet_version: Wersja .NET, na której uruchamiana jest aplikacja

# virtual_application:
# virtual_path: URL aplikacji, '/' oznacza główny katalog
# physical_path: Ścieżka fizyczna aplikacji na serwerze
# preload: Czy aplikacja jest wczytywana od razu po starcie

# ip_restriction_default_action: Domyślna akcja restrykcji IP (domyślnie Allow - brak restrykcji)
# scm_ip_restriction_default_action: Akcja restrykcji IP dla deploymentu SCM/Kudu

# identity:
# type: Managed Identity aplikacji (SystemAssigned – zarządzane przez Azure)

# logs:
# detailed_error_messages: Szczegółowe informacje o błędach (przydatne do debugowania)
# failed_request_tracing: Logowanie szczegółów nieudanych żądań HTTP

# http_logs:
# retention_in_days: Jak długo logi są przechowywane (w dniach)
# retention_in_mb: Maksymalny rozmiar przechowywanych logów (w MB)

resource "azurerm_key_vault" "keyvault" {
  name                      = "ApiFootballKeyVolt"
  location                  = "polandcentral"
  resource_group_name       = azurerm_resource_group.rg.name
  tenant_id                 = var.tenant_id # to CIklum
  sku_name                  = "standard"
  enable_rbac_authorization = true
  purge_protection_enabled  = false
}

resource "azurerm_mssql_server" "sql_server" {
  name                         = "apifootball-sqlserver"
  location                     = "polandcentral"
  resource_group_name          = azurerm_resource_group.rg.name
  version                      = "12.0"
  administrator_login          = var.sql_admin_login
  administrator_login_password = var.sql_admin_password
}

# SQL Database 
resource "azurerm_mssql_database" "main_db" {
  name                 = "apifootballdb"
  server_id            = azurerm_mssql_server.sql_server.id
  sku_name             = "Basic"
  max_size_gb          = 2
  collation            = "SQL_Latin1_General_CP1_CI_AS"
  zone_redundant       = false
  storage_account_type = "Local"
}