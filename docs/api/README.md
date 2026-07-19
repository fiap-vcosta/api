# API — collections e exploração

Artefatos para exercitar a API fora da suite .NET (Swagger + Requestly).

## Requestly

Pasta: [`requestly/`](requestly/)

| Arquivo | Uso |
|---------|-----|
| [`requestly/tech-challenge.requestly.json`](requestly/tech-challenge.requestly.json) | Collection **exploratória** (todos os endpoints) |
| [`requestly/tech-challenge-e2e-tests.requestly.json`](requestly/tech-challenge-e2e-tests.requestly.json) | Suites **automatizadas** (Collection Runner) |
| [`requestly/environments/docker.requestly.json`](requestly/environments/docker.requestly.json) | Environment **Docker** → `http://localhost:8080` |
| [`requestly/environments/local.requestly.json`](requestly/environments/local.requestly.json) | Environment **Local** → `http://localhost:5225` |
| [`requestly/environments/all.requestly.json`](requestly/environments/all.requestly.json) | Docker + Local num único arquivo |

As collections já embutem os environments Docker e Local; ao importá-las, os dois ambientes entram juntos.

### Environments

| Nome | `baseUrl` | Quando usar |
|------|-----------|-------------|
| **Docker** | `http://localhost:8080` | `docker-compose --profile app up -d` |
| **Local** | `http://localhost:5225` | `dotnet run` em `src/Api` (perfil `http` do `launchSettings.json`) |

Variáveis incluídas: `baseUrl`, `token` (secret, preenchido no login), `tokenAprovacao`, e ids auxiliares (`ordemServicoId`, `clienteId`, …).

### Como importar

1. Suba a API (Docker **ou** Local)
2. Abra o [Requestly API Client](https://requestly.com/)
3. **Import → Requestly** (Collection & Environment)
4. Importe a collection desejada **ou** só `environments/all.requestly.json`
5. No seletor de environment (canto superior), escolha **Docker** ou **Local**
6. Rode `00-auth / login` (ou `00-login` nas suites e2e) antes das rotas Admin

Credenciais seed: `admin` / `admin`.

### Collection Runner (e2e)

Em cada pasta de fluxo (ex.: `01-criar-com-servicos-ate-entregue`): menu **⋯ → Run**.  
Cada pasta é autônoma (começa com login) e usa `rq.test` / `rq.expect`.

### Alternativa

- Swagger Docker: http://localhost:8080/swagger/index.html  
- Swagger Local: http://localhost:5225/swagger/index.html
