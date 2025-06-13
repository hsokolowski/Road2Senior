data "azuredevops_project" "project" {
  name = "hus"
}

resource "azuredevops_build_definition" "build_app" {
  name       = "azure-build-app"
  project_id = data.azuredevops_project.project.id
  path       = "\\"

  repository {
    repo_type             = "GitHub"
    repo_id               = "hsokolowski/Road2Senior"
    yml_path              = "azure-build-app.yml"
    branch_name           = "master"
    service_connection_id = data.azuredevops_serviceendpoint_github.github.id # Service connection brany z DevOps Azure/Settings
  }
}

resource "azuredevops_build_definition" "deploy_app" {
  name       = "azure-deploy-app"
  project_id = data.azuredevops_project.project.id
  path       = "\\"

  repository {
    repo_type             = "GitHub"
    repo_id               = "hsokolowski/Road2Senior"
    yml_path              = "azure-deploy-app.yml"
    branch_name           = "master"
    service_connection_id = data.azuredevops_serviceendpoint_github.github.id
  }
}

resource "azuredevops_build_definition" "infra" {
  name       = "azure-infra"
  project_id = data.azuredevops_project.project.id
  path       = "\\"

  repository {
    repo_type             = "GitHub"
    repo_id               = "hsokolowski/Road2Senior"
    yml_path              = "azure-infra.yml"
    branch_name           = "master"
    service_connection_id = data.azuredevops_serviceendpoint_github.github.id
  }
}
