# Testes

A suite protege o comportamento da API e do domínio. Há três camadas: **unitários**, **integração** (HTTP in-process) e **e2e HTTP** (Requestly contra API real).

Visão no README raiz: seção **Testes** e **Collection das APIs**.

---

## Unitários

| | |
|--|--|
| Projeto | [`tests/UnitTests`](../../tests/UnitTests) |
| Escopo | Espelha `src/`: Domain, Application, Api, Infrastructure |
| Estilo | **AAA** (`Arrange` / `Act` / `Assert`) — um tipo sob teste por arquivo |
| Meta | ≥ **80%** line e branch por assembly (`Api`, `Application`, `Infrastructure`, `Domain`) |

```bash
dotnet test tests/UnitTests/UnitTests.csproj
./run-tests-with-coverage.sh
```

Relatório HTML: `test-results/coverage-report/index.html`. No CI, o job **Unit tests** publica o artifact `coverage-report` e o resumo no Job Summary.

Não escrevemos testes de feature ainda não implementada (ordem da Fase 02).

---

## Integração

| | |
|--|--|
| Projeto | [`tests/IntegrationTests`](../../tests/IntegrationTests) |
| Stack | `WebApplicationFactory` + **Testcontainers** (PostgreSQL) |
| Requisito | Docker disponível na máquina / no runner |

Cobre fluxos HTTP reais contra a API em memória com banco efêmero, por exemplo:

- Cadastros e autenticação
- Criação de OS com serviços/peças (incluindo listas vazias)
- Listagem (ordenação + exclusões)
- Aprovação/rejeição **sem** JWT (token público)
- Fluxos ponta a ponta do ciclo da OS

```bash
dotnet test tests/IntegrationTests/IntegrationTests.csproj
```

No CI: job **Integration tests** em paralelo com unitários e lint.

---

## E2E HTTP (Requestly)

Exercitam a API **já em execução** (Docker, kind ou `dotnet run`), fora da suite .NET.

| | |
|--|--|
| Guia | [`docs/api/README.md`](../api/README.md) |
| Exploratória | [`requestly/tech-challenge.requestly.json`](../api/requestly/tech-challenge.requestly.json) |
| Suites Runner | [`requestly/tech-challenge-e2e-tests.requestly.json`](../api/requestly/tech-challenge-e2e-tests.requestly.json) |
| Environments | Docker `http://localhost:8080` · Local `http://localhost:5225` |

Uso típico na demo: importar no [Requestly](https://requestly.com/), escolher o environment, rodar `00-auth / login` (seed `admin` / `admin`) e as pastas de fluxo no Collection Runner. Alternativa visual: Swagger.

Aprovação como **ator externo**: endpoints públicos `POST /api/public/ordens-servico/aprovar|rejeitar?token=` (sem JWT).

---

## CI

Workflow [`.github/workflows/ci.yml`](../../.github/workflows/ci.yml): Lint + Unit (com gate de cobertura) + Integration em paralelo a cada `push`.
