# 📦 Dokumentacja pipeline'ów Road2Senior

## 🛠️ 1. `azure-build-app.yml` – Pipeline budujący aplikację .NET

### ✅ Co robi:
- Uruchamia się na `push` do `master`
- Buduje projekt .NET 8 (`dotnet build`)
- Uruchamia testy (`dotnet test`)
- Publikuje ZIP (`dotnet publish` + artifact)

### 🕘 Kiedy uruchamiać:
- Zmiany w kodzie źródłowym aplikacji (.cs)
- Nowe testy, refaktoryzacje
- Manualnie w Azure DevOps, np. dla testu artefaktu

### 💡 Co dodać:
- Wersjonowanie builda (np. `1.0.$(Build.BuildId)`)
- Testy jednostkowe/integracyjne
- Analiza jakości kodu (SonarQube, Coverlet)


## 🚀 2. `azure-deploy-app.yml` – Pipeline wdrażający aplikację do Azure Web App

### ✅ Co robi:
- Pobiera ZIP z artifactu z build pipeline
- Deployuje do `apifootball-web` w Azure App Service
- Może ustawiać App Settings, restartować appkę

### 🕘 Kiedy uruchamiać:
- Po zakończonym buildzie (`dependsOn`)
- Manualnie do rollbacku/stagingu
- Po zmianach typu hotfix w branchu

### 💡 Co dodać:
- Warunki deployu (tylko jeśli build succeeded)
- Powiadomienia (Slack, e-mail)
- Automatyczna zmiana wersji w AppSettings


## ☁️ 3. `azure-infra.yml` – Pipeline do infrastruktury (Terraform)

### ✅ Co robi:
- `terraform init`, `plan`, `apply`
- Provisioning zasobów: RG, SQL, Key Vault, App Service, Storage, Pipelines

### 🕘 Kiedy uruchamiać:
- Po zmianach w plikach `.tf`
- Tworzenie nowych środowisk (np. dev/staging)
- Dodanie nowych zasobów (np. baza, nowy App)

### 💡 Co dodać:
- `terraform validate` i `fmt`
- Osobny krok `plan` z approvalem
- Praca na remote backend + secrets z Key Vault

Dokładnie! Właśnie po to masz azure-infra.yml – żeby nie klikać w portalu, tylko dodawać nowe zasoby przez .tf, a potem:

✨ Jednym kliknięciem w DevOps zaktualizować całą infrastrukturę.

## 🔐 Obsługa zmiennych wrażliwych (Terraform + Azure DevOps)

### 🎯 Cel

Utrzymujemy wrażliwe dane (`sql_admin_password`, `tenant_id`, `azure_devops_pat`, `github_pat`) w dwóch miejscach:
- **Lokalnie** — używamy `secret.tfvars` przy pracy z `terraform apply`
- **W pipeline** — konfigurujemy zmienne jako Variable Group w Azure DevOps

---

### 🖥️ Użycie lokalne (Terraform CLI)

1. Przechowujemy dane w pliku `secret.tfvars` (nigdy nie wrzucamy go do repo!):
    ```hcl
    sql_admin_login       = "admin"
    sql_admin_password    = "supersecret123"
    tenant_id             = "xxxx-xxxx-xxxx"
    azure_devops_pat      = "xxx"
    github_pat            = "xxx"
    ```

2. Uruchamiamy Terraform lokalnie z tym plikiem:
    ```bash
    terraform plan -var-file="secret.tfvars"
    terraform apply -var-file="secret.tfvars"
    ```

---

### 🔧 Konfiguracja w Azure DevOps (dla pipeline `azure-infra`)

1. Przejdź do:
   `Azure DevOps → Pipelines → Library → + Variable Group`

2. Nazwij grupę np. `infra-secrets`

3. Dodaj zmienne (zaznacz `Keep this value secret` tam, gdzie potrzebne):
    - `sql_admin_login`
    - `sql_admin_password` ✅
    - `tenant_id` ✅
    - `azure_devops_pat` ✅
    - `github_pat` ✅

4. W YAML pipeline (`azure-infra.yml`) dodaj sekcję:
    ```yaml
    variables:
      - group: infra-secrets
    ```

5. Upewnij się, że zadanie `terraform apply` wygląda np. tak:
    ```yaml
    - script: |
        terraform init
        terraform apply -auto-approve \
          -var "sql_admin_login=$(sql_admin_login)" \
          -var "sql_admin_password=$(sql_admin_password)" \
          -var "tenant_id=$(tenant_id)" \
          -var "azure_devops_pat=$(azure_devops_pat)" \
          -var "github_pat=$(github_pat)"
      displayName: Terraform Apply
    ```

---

### 🔒 Bezpieczeństwo

- `secret.tfvars` dodajemy do `.gitignore` – nie może trafić do repo!
- W Azure DevOps **nie używamy zmiennych tekstowych w YAML**, tylko Variable Group z ukrytymi wartościami

---

### ✅ Rezultat

Dzięki temu:
- Pracujesz lokalnie wygodnie z `secret.tfvars`
- Twój pipeline działa w pełni automatycznie i bezpiecznie z DevOps Variable Group

## 🔄 Relacja między pipeline'ami

```
[ azure-build-app ]
       |
       v
[ Artifact ZIP ] → [ azure-deploy-app ]
                            ^
                            |
                    Ręcznie lub trigger
                    po sukcesie builda

[ azure-infra ] → osobny pipeline TF
```


## 📋 Przykładowe scenariusze

| Zmiana                         | Build | Deploy | Infra |
|-------------------------------|-------|--------|-------|
| Nowy feature w C#             | ✅     | ✅      | ❌     |
| Zmiana YAML pipeline          | ❌     | ✅      | ❌     |
| Zmiana AppSettings (Terraform)| ❌     | ✅      | ✅     |
| Dodanie Key Vault / SQL       | ❌     | ❌      | ✅     |
| Tworzenie środowiska staging  | ❌     | ❌      | ✅     |
| Modyfikacja pliku `*.tf`      | ❌     | ❌      | ✅     |