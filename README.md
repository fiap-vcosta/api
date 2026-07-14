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

```bash
# Na raiz do projeto, execute:
docker-compose --profile app up -d
```

Isso vai iniciar:
- **PostgreSQL** na porta `5432`
- **SonarQube** na porta `9001`
- **PgAdmin** na porta `5050`
- **Aplicação** será construída e disponibilizada

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
dotnet test
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

## Estrutura do Projeto

```
├── src/
│   ├── Api/              # API REST (Controllers, Contracts, Validators)
│   ├── Application/      # Lógica de aplicação (Use Cases, DTOs)
│   ├── Domain/           # Entidades e interfaces de domínio
│   └── Infrastructure/   # Implementações (Database, Services)
├── tests/
│   └── UnitTests/        # Testes unitários
├── docs/                 # Documentação (histórias de domínio, requisitos)
├── docker-compose.yml    # Orquestração de containers
└── Dockerfile            # Construção da imagem Docker
```

## Documentação Adicional

- [Requisitos do Projeto](docs/01_requisitos.md)
- [Requisitos e decisões — Fase 02](docs/04_requisitos-fase-02.md)
- [Linguagem Onipresente](docs/00_linguagem-onipresente.md)
- [ADR - Escolha do Banco de Dados](docs/adrs/001-escolha-banco-de-dados.md)
- [Guia para agentes de IA](AGENTS.md)
