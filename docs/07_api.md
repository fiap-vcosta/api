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
| [`requestly/environments/docker.requestly.json`](requestly/environments/docker.requestly.json) | Environment **Docker** → `http://localhost:8080` |
| [`requestly/environments/local.requestly.json`](requestly/environments/local.requestly.json) | Environment **Local** → `http://localhost:5225` |
| [`requestly/environments/all.requestly.json`](requestly/environments/all.requestly.json) | Docker + Local num único arquivo |

As collections já embutem os environments Docker e Local; ao importá-las, os dois ambientes entram juntos.

### Environments

| Nome | `baseUrl` | Quando usar |
|------|-----------|-------------|
| **Docker** | `http://localhost:8080` | `docker compose --profile app up -d` |
| **Local** | `http://localhost:5225` | `dotnet run --project src/Api --launch-profile http` |

Variáveis incluídas: `baseUrl`, `token` (secret, preenchido no login), `tokenCliente` (JWT cliente — mint local até a Function existir), `tokenAprovacao`, `serviceAuthKey` (secret do header `X-Service-Key`), `documentoCliente`, `jwtClienteIssuer`, e ids auxiliares (`ordemServicoId`, `clienteId`, …).

### Como importar

1. Suba a API (Docker ou Local)
2. Abra o [Requestly API Client](https://requestly.com/)
3. **Import → Requestly** (Collection & Environment)
4. Importe a collection desejada **ou** só `environments/all.requestly.json`
5. No seletor de environment (canto superior), escolha **Docker** ou **Local**
6. Rode `00-auth / login` (ou `00-login` nas suites e2e) antes das rotas Admin

Credenciais seed: `admin` / `admin`.

Para o endpoint de sistema (RF23), use o valor de `SERVICE_AUTH_KEY` do `.env` em `serviceAuthKey` (já vem com o default local do `.env.example`).

### JWT cliente local (antes da Function `auth`)

Até a §8, mint o Bearer cliente com o mesmo material `JwtCliente` do Compose:

```bash
./scripts/mint-cliente-jwt.sh 11144477735
```

Cole a saída em `tokenCliente` no environment Docker/Local. O script lê `JWT_CLIENTE_*` do `.env`.

### Collection Runner (e2e)

Em cada pasta de fluxo (ex.: `01-criar-com-servicos-ate-entregue`): menu **⋯ → Run**.  
Cada pasta é autônoma (começa com login) e usa `rq.test` / `rq.expect`.

### Ator cliente (aprovação com JWT + token opaco)

Localiza a OS pelo token opaco na query e exige **JWT de cliente** (Bearer). Ownership: CPF do JWT deve ser o do dono da OS.

- `POST …/ordens-servico/aprovar?token=...` (+ `Authorization: Bearer <JWT cliente>`)
- `POST …/ordens-servico/rejeitar?token=...` (+ `Authorization: Bearer <JWT cliente>`)

Chamam os mesmos use cases de aprovar/rejeitar da API Admin. O token opaco não é exposto nas responses de criação/consulta. JWT cliente será emitido pelo repo [`auth`](https://github.com/fiap-vcosta/auth); localmente use `scripts/mint-cliente-jwt.sh`. As rotas públicas ainda estão anônimas até o PR de ownership (RF21).

### Endpoint de serviço (RF23 — Function → API)

Consulta existência de cliente por documento (CPF ou CNPJ), autenticada por shared secret (não é rota anônima):

- `GET /api/system/clientes/por-documento/{documento}`
- Header: `X-Service-Key: {{serviceAuthKey}}`
- Existe → `200` (corpo vazio); não existe → `404`; documento inválido → `400`; sem key → `401`

No Requestly: pasta `06-system` (exploratória) e suite e2e `10-consultar-cliente-por-documento`.

### Alternativa

- Swagger Docker: http://localhost:8080/swagger/index.html  
- Swagger Local: http://localhost:5225/swagger/index.html
