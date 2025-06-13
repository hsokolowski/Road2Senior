346d2f81-de9f-40aa-ae50-ebb3bc20a09b
# 📘 Road2Senior – Terraform Infrastructure Setup for Azure + DevOps

Ten plik README to **kompletny dziennik wiedzy i instrukcji** związanych z infrastrukturą Road2Senior zarządzaną przy pomocy **Terraform**.  
Zawiera:
- Opis wszystkich plików konfiguracyjnych
- Instrukcje krok po kroku jak skonfigurować środowisko
- Wiedzę o połączeniach z GitHub, PAT, Azure DevOps
- Tipy i wskazówki „co zrobić w przyszłości”

---

## 📁 Struktura plików

| Plik | Opis |
|------|------|
| `main.tf` | Główny plik: tworzy zasoby Azure (Web App, SQL Server, Key Vault, RG), DevOps Project i GitHub Connection |
| `azure_pipelines.tf` | Definicje 3 pipeline'ów (build, deploy, infra) z użyciem repo z GitHub |
| `backend.tf` | Konfiguracja backendu remote state (Azure Blob Storage) |
| `variables.tf` | Deklaracja zmiennych z domyślnymi wartościami lub typami |
| `secret.tfvars` | Plik zawierający hasła, PAT-y, ID-ki – nie commituj do repo! |
| `outputs.tf` | Wyświetlenie wartości jak ID resource group, ID GitHub connection |
| `storage.tf` | Tworzy Storage Account + Container na plik stanu Terraform |
| `terraform.tfstate`, `terraform.tfstate.backup` | Pliki z lokalnym stanem – po migracji do backenda mogą zniknąć |
| `.terraform.lock.hcl` | Lockfile providera – nie edytuj ręcznie |
| `readme.md` | 📘 Ty właśnie tu jesteś! |

---

## ⚙️ Co zostało zrobione

✅ Stworzono **pełną infrastrukturę w Azure**:
- Resource Group, Web App (.NET 8), App Service Plan, SQL Server, Baza Danych
- Key Vault z RBAC
- Remote backend w Azure Blob Storage

✅ Skonfigurowano **Azure DevOps** z poziomu Terraform:
- Projekt `hus`
- Service Endpoint do GitHub (`GitHubConnection`)
- 3 pipeline’y:
    - `azure-build-app`: buduje projekt .NET
    - `azure-deploy-app`: deploy do App Service
    - `azure-infra`: deploy infrastruktury przez Terraform

---

## 🧪 Jak to działa krok po kroku

1. Stwórz plik `secret.tfvars`:

```hcl
sql_admin_login       = "TwojLogin"
sql_admin_password    = "TwojeHaslo"
tenant_id             = "ID Tenanta (np. z Azure AD)"
azure_devops_pat      = "PAT do Azure DevOps (z uprawnieniami Project & Pipeline)"
github_pat            = "PAT do GitHub (z uprawnieniami: repo, admin:repo_hook, user)"
github_service_connection_id = "" # Zostanie nadpisane automatycznie
```

2. W terminalu wykonaj (1 raz):
```bash
terraform init -reconfigure   -backend-config="resource_group_name=hus-dev"   -backend-config="storage_account_name=tfstatehubert001"   -backend-config="container_name=tfstate"   -backend-config="key=terraform.tfstate"
```

3. Następnie:
```bash
terraform plan -var-file="secret.tfvars"
terraform apply -var-file="secret.tfvars"
```

4. Pipeline’y powinny pojawić się w Azure DevOps i działać z YAML-i w repozytorium.

---

## 🛠️ Jak utworzyć PAT (Personal Access Token)

### Azure DevOps
1. Wejdź na: `https://dev.azure.com/{TwojaOrg}/_usersSettings/tokens`
2. Nadaj mu nazwę (np. "terraform-pat")
3. Wybierz zakresy:
    - `Read & manage` → **Project & Team**
    - `Read & execute` → **Build & Release**
    - `Full Access` → **Service Connections**

### GitHub
1. Wejdź: `https://github.com/settings/tokens`
2. Wybierz **classic** lub **fine-grained** (classic jest prostszy)
3. Zaznacz:
    - `repo`, `user`, `admin:repo_hook`

---

## 🚀 Co robić w przyszłości

| Czynność | Co zrobić |
|---------|-----------|
| Zmienić nazwę Web App | Zmień `azurerm_windows_web_app.name` w `main.tf` |
| Zmienić nazwę pipeline | Zmień nazwę w `azure_pipelines.tf` oraz YAML |
| Zmienić nazwę pliku YAML | Zmień `yml_path` i zatwierdź nowy plik do GitHub |
| Zmienić login/hasło SQL | Zmień wartości w `secret.tfvars` |
| Zmienić połączenie GitHub | Zmień `azuredevops_serviceendpoint_github` lub zaimportuj nowe |
| Przenieść backend | Edytuj `backend.tf` i wykonaj `terraform init -reconfigure` |

---

## ❗ Tips i pułapki

- **Service Endpoint GitHub** najlepiej stworzyć ręcznie i zaimportować (import → Terraform)
- Unikaj nadmiarowego `terraform apply`, jeśli DevOps nie ma uprawnień do usunięcia roli – pojawi się `AuthorizationFailed`
- `terraform.tfstate` trzymaj w storage, nie commituj lokalnego!
- `terraform destroy` może mieć problem z rolami jeśli nie masz pełnych RBAC

---

## 📂 Import istniejącego zasobu (np. GitHub service connection)

```bash
terraform import azuredevops_serviceendpoint_github.github hus/3ce06609-d9b5-478a-84b9-93071a5143fd
```

Następnie upewnij się, że jego definicja znajduje się w `main.tf`.

---

## 🧠 TL;DR – Co mamy

- ✅ Azure Web App + Baza SQL
- ✅ Key Vault z tożsamością
- ✅ Pełna integracja GitHub + Azure DevOps
- ✅ CI/CD: build, deploy, infra
- ✅ Terraform state w Azure Blob Storage
- ✅ Wiedza jak to odtworzyć/zmodyfikować

---

## 👨‍💻 Autor
Road2Senior Infrastructure by [Hubert Sokołowski](https://github.com/hsokolowski)

---

## 🔧 Troubleshooting (Rozwiązywanie problemów)

### Problem: `terraform apply` kończy się błędem 401 Unauthorized przy imporcie serviceendpoint
**Rozwiązanie:**
- Sprawdź czy `azure_devops_pat` w `secret.tfvars` jest aktualny.
- Zweryfikuj czy masz uprawnienia do projektu DevOps (co najmniej `Project Administrator`).

### Problem: Nie można usunąć `azuredevops_serviceendpoint_github` — error z rolą na subskrypcji
**Rozwiązanie:**
- Ręcznie usuń service connection w Azure DevOps (Project Settings > Service Connections).
- Usuń przypisaną rolę SP w Azure Portal (IAM > Role assignments).

### Problem: `Build pipeline already exists`
**Rozwiązanie:**
- Wykonaj `terraform import` dla odpowiedniego pipeline:
  ```bash
  terraform import azuredevops_build_definition.azure_build_app [ID]
  ```

---

## 🧭 Diagram (Opis architektury)

```mermaid
graph TD
    A[Terraform CLI] --> B[Azure DevOps]
    A --> C[Azure Resources]
    B --> D[GitHub (YAML)]
    C --> E[App Service, SQL, KeyVault]
    C --> F[Storage (tfstate)]
    B --> G[Build/Deploy Pipelines]
```

---

## 🔗 Przydatne linki

- [Terraform Azure DevOps Provider](https://registry.terraform.io/providers/microsoft/azuredevops/latest)
- [Terraform AzureRM Provider](https://registry.terraform.io/providers/hashicorp/azurerm/latest)
- [Azure DevOps PAT Generator](https://dev.azure.com/{ORG}/_usersSettings/tokens)
- [Azure Portal - Service Connections](https://dev.azure.com/{ORG}/{PROJECT}/_settings/adminservices)

## 🔧 Terraform Backend – kiedy i jak go tworzyć?

### 📌 Kiedy tworzyć backend?
Tworzymy backend **na początku projektu Terraform**, **zanim zaczniemy tworzyć zasoby** – jeśli chcemy, żeby stan (`terraform.tfstate`) był **zdalny** (np. w Azure Storage), a nie lokalny.

Jest to **konieczne**, jeśli:
- pracujesz w zespole i chcesz dzielić stan między developerami
- używasz CI/CD (np. Azure Pipelines) i chcesz, żeby pipeline miał dostęp do aktualnego stanu
- chcesz zachować **bezpieczeństwo, locki i historię zmian** stanu infrastruktury

---

### 🛠️ Jak go tworzymy?

1. **Dodajesz plik `backend.tf` z konfiguracją zdalnego backendu:**

```hcl
terraform {
  backend "azurerm" {
    resource_group_name  = "hus-dev"
    storage_account_name = "tfstatehubert001"
    container_name       = "tfstate"
    key                  = "terraform.tfstate"
  }
}
```

2. **Tworzysz Storage Account oraz kontener przez Terraform (storage.tf):**

```
resource "azurerm_storage_account" "tf_backend_sa" { ... }
resource "azurerm_storage_container" "tf_backend_container" { ... }
```

3. **Po ich utworzeniu (np. lokalnie):**

```
terraform init
```
To zainicjuje backend – czyli zapisze stan z lokalnego pliku do Azure.

## 🚀 Kolejność prac w projekcie Terraform

1. **Utwórz backend (zdalny stan)** – *na samym początku!*
    - Zapisz konfigurację do `backend.tf`
    - Przygotuj `storage.tf` (Storage Account + Container)
    - Wykonaj lokalnie:
      ```bash
      terraform init
      terraform apply -target=azurerm_storage_account.tf_backend_sa
      terraform apply -target=azurerm_storage_container.tf_backend_container
      terraform init  # ponownie, teraz z podłączeniem do backendu
      ```

2. **Dodaj pliki logiczne infrastruktury:**
    - `main.tf` – zasoby: Web App, SQL, App Plan itp.
    - `variables.tf` – zmienne
    - `secret.tfvars` – hasła i wrażliwe dane (lokalny, nie commituj)
    - `outputs.tf` – ID zasobów i inne przydatne dane

3. **Zrób pełny `terraform plan` i `apply`:**
    - Sprawdzasz co zostanie utworzone
    - Wdrażasz wszystko mając stan w Azure (remote backend)

---

### 🔁 I dalej już tylko:
- Modyfikujesz .tf i robisz `terraform plan`, `apply`
- Nie martwisz się lokalnym stanem
- W pipeline możesz użyć tego samego backendu

---
