# Gestão de Oficina Mecânica

API .NET 8 para ordens de serviço, clientes, veículos, catálogo de serviços e estoque. Clean Architecture, PostgreSQL, Docker Compose para dev local e GKE Autopilot na GCP. Cluster e banco são provisionados pelos repos [`infra-k8s`](https://github.com/fiap-vcosta/infra-k8s) e [`infra-db`](https://github.com/fiap-vcosta/infra-db); os manifests e o deploy moram aqui.

## Arquitetura

### C4 — System Context

```mermaid
C4Context
title Gestao de Oficina - Context

Person(admin, "Atendente / Admin", "Opera cadastros e o ciclo da OS (JWT).")
Person(cliente, "Cliente", "Aprova ou rejeita o orcamento (token opaco).")

System(oficina, "Gestao de Oficina", "Ordens de servico, cadastros, estoque e orcamento.")

Rel(admin, oficina, "HTTPS / JWT")
Rel(cliente, oficina, "HTTPS / token")
```

### C4 — Containers

```mermaid
C4Container
title Gestao de Oficina - Containers

Person(admin, "Atendente / Admin", "")
Person(cliente, "Cliente", "")

System_Boundary(oficina, "Gestao de Oficina") {
    Container(api, "API", ".NET 8 / ASP.NET Core", "REST, JWT e aprovacao publica de orcamento.")
    ContainerDb(db, "PostgreSQL", "Banco relacional", "Clientes, veiculos, OS e estoque.")
}

Rel(admin, api, "HTTPS / JWT")
Rel(cliente, api, "HTTPS / token")
Rel(api, db, "EF Core")
```

Fluxo interno da API: `Controller` → `Use Case` → `Gateway` / `Domain` → `Presenter` → HTTP.

### Infraestrutura

```mermaid
flowchart TB
  subgraph gcp["GCP"]
    ar[("Artifact Registry")]
    subgraph gke["GKE Autopilot"]
      lb["Service LoadBalancer"]
      subgraph pod["Pod"]
        api["API .NET"]
        proxy["Cloud SQL Auth Proxy"]
      end
      hpa["HPA"]
    end
    sql[("Cloud SQL PostgreSQL")]
    sm[("Secret Manager")]
  end

  lb --> api
  hpa -.->|"escala"| api
  api -->|"127.0.0.1:5432"| proxy
  proxy -->|"IP privado + IAM"| sql
  ar -.->|"imagem"| api
  sm -.->|"senha no deploy"| api
```

### CI

```mermaid
sequenceDiagram
  participant Dev
  participant CI as CI
  Dev->>CI: push / PR
  CI->>CI: lint, unit, integration
```

---

## Como executar

### Docker Compose

```bash
cp .env.example .env
docker compose --profile app up -d --build
```

- API / Swagger / Health: http://localhost:8080 · `/swagger` · `/health`
- Login seed: `admin` / `admin`
- Variáveis: [`.env.example`](.env.example)

### SDK .NET

```bash
docker compose up -d
dotnet restore
dotnet ef database update --project src/Infrastructure --startup-project src/Api
dotnet run --project src/Api --launch-profile http
```

API local: http://localhost:5225

---

## Deploy na GCP

Manifests em [`k8s/`](k8s/): namespace, service account, ConfigMap, Deployment, Service `LoadBalancer` e HPA. O Deployment roda o **Cloud SQL Auth Proxy como sidecar nativo**, que autentica na instância por IAM (Workload Identity) e escuta em `127.0.0.1:5432` — a API só conhece `localhost`.

Não há Job de migration: a aplicação roda `db.Database.Migrate()` no start, e o sidecar nativo garante que o túnel esteja pronto antes disso.

| Workflow | Quando | O que faz |
|----------|--------|-----------|
| [`build-push`](.github/workflows/build-push.yml) | Merge em `main` | Build da imagem e push no Artifact Registry (`:latest` + SHA curto) |
| [`deploy`](.github/workflows/deploy.yml) | Só manual | Aplica os manifests com a imagem `:latest` e devolve o IP público |

O `build-push` mantém o registry sempre com a imagem da `main`, e é independente de cluster e banco — roda fora da janela de demo. O `deploy` sempre usa `:latest`.

A cada deploy o workflow lê a senha do banco no Secret Manager, **gera** uma chave JWT nova e recria o Secret `api` do cluster. Nenhum dos dois valores existe no Git ou em tfstate. Em troca, a chave rotaciona: token de staff obtido antes de um redeploy deixa de valer, e basta logar de novo.

Pré-requisitos: cluster no ar (`infra-k8s` → `tf-apply`), banco no ar (`infra-db` → `tf-apply`) e as org vars `GCP_PROJECT_ID`, `GCP_REGION`, `GCP_AR_REPOSITORY`, `GCP_GKE_CLUSTER_NAME`, `GCP_DB_PASSWORD_SECRET`, `GCP_WORKLOAD_IDENTITY_PROVIDER` e `GCP_SERVICE_ACCOUNT_EMAIL`.

Evidência de HPA depois do deploy:

```bash
./scripts/stress-hpa.sh http://<ip-do-loadbalancer>
kubectl get hpa,pods -n tech-challenge
```

---

## APIs principais

| Capacidade | Endpoint | Auth |
|------------|----------|------|
| Abrir OS (veículo + serviços + peças) | `POST /api/ordens-servico` | JWT Admin |
| Consultar OS | `GET /api/ordens-servico/{id}` | JWT Admin |
| Listar OS ativas | `GET /api/ordens-servico` | JWT Admin |
| Aprovar / rejeitar orçamento | `POST /api/public/ordens-servico/aprovar?token=` · `.../rejeitar?token=` | Público |

Listagem exclui Finalizada, Entregue e Descartada; ordenação por status evolutivo e data. Aprovação pública reutiliza os mesmos use cases da API autenticada. Requisitos: [`docs/01_requisitos.md`](docs/01_requisitos.md).

---

## Testes e CI/CD

```bash
dotnet test tests/UnitTests/UnitTests.csproj
dotnet test tests/IntegrationTests/IntegrationTests.csproj   # requer Docker
./run-tests-with-coverage.sh                                 # meta ≥ 80%
```

- Estratégia: [`docs/06_testes.md`](docs/06_testes.md)
- Collections HTTP: [`docs/07_api.md`](docs/07_api.md) (Swagger + Requestly)
- CI: lint + unit + integration em paralelo ([`ci.yml`](.github/workflows/ci.yml))
- CD: `build-push` em merge na `main`, `deploy` manual (ver [Deploy na GCP](#deploy-na-gcp))

---

## Documentação

Índice: [`docs/README.md`](docs/README.md)
