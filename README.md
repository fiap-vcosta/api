# TechChallenge - Aplicação de Gestão de Serviços

Aplicação desenvolvida em .NET 8 para gerenciamento de serviços, estoque, clientes e veículos. Utiliza Clean Architecture com camadas bem definidas: Domain, Application, Infrastructure e API.

## Tecnologias

- **.NET 8** - Framework principal
- **PostgreSQL** - Banco de dados
- **Entity Framework Core** - ORM
- **Docker** - Containerização
- **Swagger** - Documentação da API
- **SonarQube** - Análise de código
- **PgAdmin** - Gerenciamento do banco de dados

## Pré-requisitos

### Opção 1: Com Docker (Recomendado)
- Docker
- Docker Compose

### Opção 2: Localmente
- .NET SDK 8.0+
- PostgreSQL 16+
- Git

## Como Executar

### 1️⃣ Com Docker

A imagem da API roda como usuário **não-root** (`app`). Connection string e JWT vêm de variáveis de ambiente **obrigatórias** (sem defaults no compose — o deploy falha se faltarem). Em Kubernetes, esses valores devem vir de Secret/ConfigMap.

```bash
# Obrigatório: criar .env a partir do exemplo
cp .env.example .env

# Na raiz do projeto:
docker compose --profile app up -d --build
```

Isso vai iniciar:
- **PostgreSQL** na porta `5432`
- **SonarQube** na porta `9001`
- **PgAdmin** na porta `5050`
- **Aplicação** (profile `app`) na porta `8080`

Aguarde alguns segundos até a aplicação iniciar completamente.

**Acessar a aplicação:**
- API: http://localhost:8080
- Swagger: http://localhost:8080/swagger/index.html
- Health Check: http://localhost:8080/health

**Acessar serviços auxiliares:**
- PgAdmin: http://localhost:5050
  - Email: `admin@techchallenge.com`
  - Senha: `admin`
- SonarQube: http://localhost:9001
  - Usuário: `admin`
  - Senha: `admin`

Variáveis obrigatórias: ver [`.env.example`](.env.example) (`POSTGRES_*`, `JWT_*`, `ASPNETCORE_ENVIRONMENT`, `PGADMIN_*`).
### 2️⃣ Executar Localmente

#### 2.1. Preparar a infraestrutura

```bash
docker-compose up --build -d
```

#### 2.2. Restaurar dependências

```bash
dotnet restore
```

#### 2.3. Executar migrações

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

#### 2.4. Iniciar a aplicação

```bash
dotnet run
```

A aplicação estará disponível em `http://localhost:5000`

### 3️⃣ Executar Testes

#### Testes unitários simples:
```bash
dotnet test tests/UnitTests/UnitTests.csproj
```

#### Testes de integração (requer Docker):
```bash
dotnet test tests/IntegrationTests/IntegrationTests.csproj
```

#### Testes com cobertura de código:
```bash
./run-tests-with-coverage.sh
```

Após executar, visualizar o relatório em HTML:
```bash
# Abrir em seu navegador
open test-results/coverage-report/index.html  # macOS
xdg-open test-results/coverage-report/index.html  # Linux
start test-results/coverage-report/index.html  # Windows
```

### CI (GitHub Actions)

A cada `push` em qualquer branch, o workflow [`.github/workflows/ci.yml`](.github/workflows/ci.yml) roda em paralelo:

- **Lint** — build + InspectCode (Rider); **errors** e **warnings** falham o job
- **Unit tests** — testes unitários com gate de cobertura **≥ 80%** line e branch (por assembly)
- **Integration tests** — Testcontainers (Docker)

**Ver cobertura no CI:** abra o run em Actions → artifact **`coverage-report`** → baixe e abra `index.html`. O Job Summary do job *Unit tests* também mostra o resumo percentual.

## Estrutura do Projeto

```
├── src/
│   ├── Api/              # API REST (Controllers, Contracts, Validators)
│   ├── Application/      # Lógica de aplicação (Use Cases, DTOs)
│   ├── Domain/           # Entidades e interfaces de domínio
│   └── Infrastructure/   # Implementações (Database, Services)
├── tests/
│   ├── UnitTests/        # Testes unitários
│   └── IntegrationTests/ # Testes de integração (HTTP + Testcontainers)
├── .github/workflows/    # CI (GitHub Actions)
├── docs/
│   ├── api/requestly/    # Collections Requestly (exploratória + e2e)
│   └── …                 # Requisitos, histórias de domínio, ADRs
├── docker-compose.yml    # Orquestração de containers
├── Dockerfile            # Imagem da API (não-root, K8s-ready)
├── .dockerignore         # Contexto de build enxuto
└── .env.example          # Variáveis obrigatórias (copiar para .env)
```

## Collections Requestly (demo / e2e HTTP)

Importar no [Requestly API Client](https://requestly.com/) — detalhes em [docs/api/README.md](docs/api/README.md):

- Exploratória: [`docs/api/requestly/tech-challenge.requestly.json`](docs/api/requestly/tech-challenge.requestly.json)
- Suites e2e (Collection Runner): [`docs/api/requestly/tech-challenge-e2e-tests.requestly.json`](docs/api/requestly/tech-challenge-e2e-tests.requestly.json)
- Environments: [`docs/api/requestly/environments/`](docs/api/requestly/environments/) — **Docker** (`http://localhost:8080`) e **Local** (`http://localhost:5225`)

Login seed: `admin` / `admin`.

## Documentação Adicional

- [Requisitos do Projeto](docs/01_requisitos.md)
- [Requisitos e decisões — Fase 02](docs/04_requisitos-fase-02.md)
- [Collections / exploração da API](docs/api/README.md)
- [Linguagem Onipresente](docs/00_linguagem-onipresente.md)
- [ADR - Escolha do Banco de Dados](docs/adrs/001-escolha-banco-de-dados.md)
- [Guia para agentes de IA](AGENTS.md)
