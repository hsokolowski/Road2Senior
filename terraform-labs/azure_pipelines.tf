data "azuredevops_project" "project" {
  name = "hus"
}

resource "azuredevops_build_definition" "build_app" {
  name       = "azure-build-app-${var.env_prefix}"
  project_id = data.azuredevops_project.project.id
  path       = "\\"

  repository {
    repo_type             = "GitHub"
    repo_id               = "hsokolowski/Road2Senior"
    yml_path              = "azure-build-app.yml"
    branch_name           = "master"
    service_connection_id = var.github_service_connection_id
  }

  ci_trigger {
    use_yaml = true
  }
}

resource "azuredevops_build_definition" "deploy_app" {
  name       = "azure-deploy-app-${var.env_prefix}"
  project_id = data.azuredevops_project.project.id
  path       = "\\"

  repository {
    repo_type             = "GitHub"
    repo_id               = "hsokolowski/Road2Senior"
    yml_path              = "azure-deploy-app.yml"
    branch_name           = "master"
    service_connection_id = var.github_service_connection_id
  }
}

resource "azuredevops_build_definition" "infra" {
  name       = "azure-infra-${var.env_prefix}"
  project_id = data.azuredevops_project.project.id
  path       = "\\"

  repository {
    repo_type             = "GitHub"
    repo_id               = "hsokolowski/Road2Senior"
    yml_path              = "azure-infra.yml"
    branch_name           = "master"
    service_connection_id = var.github_service_connection_id
  }
}
