# Tech Challenge — Guia para agentes

Aplicação .NET 8 de gestão de oficina (clientes, veículos, serviços, estoque, ordens de serviço). Clean Architecture + DDD/CQRS (MediatR). Banco PostgreSQL + EF Core.

## Antes de mudar código

1. Ler [docs/04_requisitos-fase-02.md](docs/04_requisitos-fase-02.md) (canon da Fase 02)
2. Ler [docs/01_requisitos.md](docs/01_requisitos.md) e [docs/00_linguagem-onipresente.md](docs/00_linguagem-onipresente.md) quando o domínio for tocado
3. Seguir a ordem de trabalho em `docs/04` (testes existentes → CI → Clean Arch → APIs novas)
4. Espelhar padrões das pastas vizinhas; não inventar estrutura paralela
5. Não escrever testes de feature ainda não implementada; não inventar requisitos fora de `docs/04`

## Layout da solution

| Projeto | Responsabilidade |
|---------|------------------|
| `src/Domain` | Entities, VOs, eventos de domínio e exceções;
| `src/Application` | `UseCases/` (Commands/Queries/Handlers/Responses) + ports em `Abstractions/Gateways` e `Abstractions/Services` |
| `src/Infrastructure` | EF Core, implementações de Gateway, JWT/SMTP stubs |
| `src/Api` | Controllers, Requests/Validators, **Presenters**, ViewModels e filtro de Problem Details |
| `tests/UnitTests` | Testes unitários do comportamento existente |
| `tests/IntegrationTests` | Testes de integração HTTP (`WebApplicationFactory` + Testcontainers) |

Dependências apontam para dentro: Api → Application → Domain; Infrastructure implementa ports.

Não adicionar comentários narrativos no código. Comentários só são aceitáveis quando explicam intenção, restrição ou trade-off que o código não comunica; marcadores AAA nos testes continuam obrigatórios.

Responses/ViewModels devem ser reutilizados quando o shape e a semântica forem iguais entre endpoints; não criar DTO por verbo HTTP por simetria de pasta.

Referência SOAT: https://github.com/proferickmuller/soat-cleanarch-csharp

## Testes

- Sempre **AAA** (`Arrange` / `Act` / `Assert`) — detalhes em [`.cursor/rules/tests.mdc`](.cursor/rules/tests.mdc).
- Testes unitários novos seguem o mesmo caminho relativo do arquivo de produção em `src/` e cobrem um tipo por arquivo.
- Meta de cobertura unitária: **≥ 80% line e branch** por assembly (`Api`, `Application`, `Infrastructure`, `Domain`).
- Só cobrir comportamento já implementado (não TDD de feature futura da Fase 02).

## Comandos

```bash
dotnet build TechChallenge.sln
dotnet test TechChallenge.sln
dotnet restore
# Cobertura + HTML: ./run-tests-with-coverage.sh
# Integração precisa de Docker (Testcontainers Postgres)
# Docker app (ver README): docker-compose --profile app up -d
# CI: .github/workflows/ci.yml (push → lint + unit + integration em paralelo)
```

Swagger local (Docker): `http://localhost:8080/swagger/index.html`

## Fase 02 — resumo operacional

- CI cedo no GitHub Actions (build + testes) antes de volume grande de refatoração
- Clean Architecture purista **antes** de criar APIs novas
- Endpoint público de aprovação/rejeição chama os **mesmos** use cases (sem JWT)
- Listagem de OS: excluir Finalizada, Entregue, Descartada; ordem evolutiva documentada em `docs/04`
- Checklist pessoal local: `FASE02-CHECKLIST.md` (gitignored) — canon público é `docs/04`
