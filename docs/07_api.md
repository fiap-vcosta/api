# API — collections e exploração

Artefatos para exercitar a API fora da suite .NET (Swagger + Requestly).  
Camada **e2e HTTP**: ver também [`06_testes.md`](06_testes.md).  
Índice: [`docs/README.md`](README.md) · README: [`README.md`](../README.md).

## Requestly

Pasta: [`requestly/`](requestly/)

| Arquivo | Uso |
|---------|-----|
| [`requestly/tech-challenge.requestly.json`](requestly/tech-challenge.requestly.json) | Collection **exploratória** (todos os endpoints) |
| [`requestly/tech-challenge-e2e-tests.requestly.json`](requestly/tech-challenge-e2e-tests.requestly.json) | Suites **automatizadas** (Collection Runner) |
| [`requestly/environments/docker.requestly.json`](requestly/environments/docker.requestly.json) | Environment **Docker / kind** → `http://localhost:8080` |
| [`requestly/environments/local.requestly.json`](requestly/environments/local.requestly.json) | Environment **Local** → `http://localhost:5225` |
| [`requestly/environments/all.requestly.json`](requestly/environments/all.requestly.json) | Docker + Local num único arquivo |

As collections já embutem os environments Docker e Local; ao importá-las, os dois ambientes entram juntos.

### Environments

| Nome | `baseUrl` | Quando usar |
|------|-----------|-------------|
| **Docker** | `http://localhost:8080` | `docker compose --profile app up -d` ou API no **kind** (`./scripts/up.sh`) |
| **Local** | `http://localhost:5225` | `dotnet run --project src/Api --launch-profile http` |

Variáveis incluídas: `baseUrl`, `token` (secret, preenchido no login), `tokenAprovacao`, e ids auxiliares (`ordemServicoId`, `clienteId`, …).

### Como importar

1. Suba a API (Docker, kind **ou** Local)
2. Abra o [Requestly API Client](https://requestly.com/)
3. **Import → Requestly** (Collection & Environment)
4. Importe a collection desejada **ou** só `environments/all.requestly.json`
5. No seletor de environment (canto superior), escolha **Docker** ou **Local**
6. Rode `00-auth / login` (ou `00-login` nas suites e2e) antes das rotas Admin

Credenciais seed: `admin` / `admin`.

### Collection Runner (e2e)

Em cada pasta de fluxo (ex.: `01-criar-com-servicos-ate-entregue`): menu **⋯ → Run**.  
Cada pasta é autônoma (começa com login) e usa `rq.test` / `rq.expect`.

### Ator externo (aprovação sem JWT)

Endpoints públicos (token opaco na query):

- `POST /api/public/ordens-servico/aprovar?token=...`
- `POST /api/public/ordens-servico/rejeitar?token=...`

Chamam os mesmos use cases de aprovar/rejeitar da API Admin. O token não é exposto nas responses de criação/consulta — use o valor obtido no fluxo de teste/seed da collection.

### Alternativa

- Swagger Docker / kind: http://localhost:8080/swagger/index.html  
- Swagger Local: http://localhost:5225/swagger/index.html
