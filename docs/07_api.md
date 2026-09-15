# API — collections e exploração

Artefatos para exercitar a API fora da suite .NET (Swagger + Requestly).  
Camada **e2e HTTP**: ver também [`06_testes.md`](06_testes.md).  
Índice: [`docs/README.md`](README.md) · README: [`README.md`](../README.md).

## Requestly

Pasta: [`requestly/`](requestly/)

Collection **única** para API + auth (o repo [`auth`](https://github.com/fiap-vcosta/auth) aponta para cá).

| Arquivo | Uso |
|---------|-----|
| [`requestly/tech-challenge.requestly.json`](requestly/tech-challenge.requestly.json) | Collection **exploratória** (API + pasta `07-auth`) |
| [`requestly/tech-challenge-e2e-tests.requestly.json`](requestly/tech-challenge-e2e-tests.requestly.json) | Suites **automatizadas** (inclui `13-auth-emitir-jwt`) |
| [`requestly/environments/docker.requestly.json`](requestly/environments/docker.requestly.json) | Environment **Docker** → `http://localhost:8080` |
| [`requestly/environments/local.requestly.json`](requestly/environments/local.requestly.json) | Environment **Local** → `http://localhost:5225` |
| [`requestly/environments/gcp-gateway.requestly.json`](requestly/environments/gcp-gateway.requestly.json) | Environment **GCP-Gateway** → `https://vcosta-fiap.online` |
| [`requestly/environments/all.requestly.json`](requestly/environments/all.requestly.json) | Docker + Local + GCP-Gateway |

As collections já embutem os environments; ao importá-las, os ambientes entram juntos.

### Environments

| Nome | `baseUrl` / `authUrl` | Quando usar |
|------|----------------------|-------------|
| **Docker** | `http://localhost:8080` / `http://localhost:8081` | `docker compose --profile app up -d` + auth local |
| **Local** | `http://localhost:5225` / `http://localhost:8081` | `dotnet run` + auth local |
| **GCP-Gateway** | `https://vcosta-fiap.online` / `…/auth` | Entrada oficial (API Gateway + apex) |

Variáveis incluídas: `baseUrl`, `authUrl`, `token` (secret, preenchido no login), `tokenCliente` (JWT cliente via auth ou mint), `tokenAprovacao`, `serviceAuthKey` (secret do header `X-Service-Key`), `documentoCliente`, `jwtClienteIssuer`, e ids auxiliares (`ordemServicoId`, `clienteId`, …).

### Como importar

1. Suba a API (Docker ou Local)
2. Abra o [Requestly API Client](https://requestly.com/)
3. **Import → Requestly** (Collection & Environment)
4. Importe a collection desejada **ou** só `environments/all.requestly.json`
5. No seletor de environment (canto superior), escolha **Docker**, **Local** ou **GCP-Gateway**
6. Rode `00-auth / login` (ou `00-login` nas suites e2e) antes das rotas Admin

Credenciais seed: `admin` / `admin`.

Para o endpoint de sistema (RF23), use o valor de `SERVICE_AUTH_KEY` do `.env` em `serviceAuthKey` (já vem com o default local do `.env.example`).

### JWT cliente

Preferencial: Function/Cloud Run [`auth`](https://github.com/fiap-vcosta/auth) — no Gateway, `POST {{authUrl}}` com `{ "documento": "…" }` (suite `12-gateway-cliente-aprovar`).

Mint local (mesmo material `JwtCliente` do Compose), se o auth não estiver no ar:

```bash
./scripts/mint-cliente-jwt.sh 92561324354
```

Cole a saída em `tokenCliente`. O script lê `JWT_CLIENTE_*` do `.env`.

### Collection Runner (e2e)

Em cada pasta de fluxo (ex.: `01-criar-com-servicos-ate-entregue`): menu **⋯ → Run**.  
Cada pasta é autônoma (começa com login) e usa `rq.test` / `rq.expect`.

Caminho feliz **OS → auth → aprovar** (entrada oficial ou Compose): pasta Requestly `12-gateway-cliente-aprovar` com environment **GCP-Gateway** (ou Docker/`authUrl` local). Pode usar o CPF seed **`52998224725`** (João Silva) ou criar cliente/veículo com outro documento válido (ex.: `92561324354`).

O token opaco não vem na API (RF21.2). Após o passo que deixa a OS em `AguardandoAprovacao`, consulte o banco e preencha `tokenAprovacao` no environment antes de seguir o auth + aprovar:

```sql
SELECT "TokenAprovacao" FROM "OrdensServico" WHERE "Id" = <ordemServicoId>;
```

### Ator cliente (aprovação com JWT + token opaco)

Localiza a OS pelo token opaco na query e exige **JWT de cliente** (Bearer). Ownership: CPF do JWT deve ser o do dono da OS.

- `POST …/ordens-servico/aprovar?token=...` (+ `Authorization: Bearer <JWT cliente>`)
- `POST …/ordens-servico/rejeitar?token=...` (+ `Authorization: Bearer <JWT cliente>`)

Chamam os mesmos use cases de aprovar/rejeitar da API Admin. Sem JWT cliente → `401`. Token inválido ou CPF do JWT ≠ dono da OS → `404` (mesmo shape). O token opaco não é exposto nas responses de criação/consulta. Seeds de cliente (ex.: João `52998224725`, Maria `39053344705`) passam na validação do auth; o fluxo Gateway também aceita CPF criado na hora (ex.: `92561324354`).

No Requestly: pasta exploratória `05-public` (Bearer `{{tokenCliente}}`) e `07-auth` (emitir JWT); e2e `08-contratos-api` (sem JWT → 401; token inválido com JWT → 404), `11-jwt-cliente-vs-funcionario-publico`, `12-gateway-cliente-aprovar` e `13-auth-emitir-jwt`.

### Endpoint de serviço (RF23 — Function → API)

Consulta existência de cliente por documento (CPF ou CNPJ), autenticada por shared secret (não é rota anônima):

- `GET /api/system/clientes/por-documento/{documento}`
- Header: `X-Service-Key: {{serviceAuthKey}}`
- Existe → `200` (corpo vazio); não existe → `404`; documento inválido → `400`; sem key → `401`

No Requestly: pasta `06-system` (exploratória) e suite e2e `10-consultar-cliente-por-documento`.

### Alternativa

- Swagger Docker: http://localhost:8080/swagger/index.html  
- Swagger Local: http://localhost:5225/swagger/index.html
