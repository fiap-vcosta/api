# Requisitos e decisões — Fase 02

Documento **versionado** (canon da equipe) para a Fase 02 do Tech Challenge. O checklist pessoal local (`FASE02-CHECKLIST.md`, gitignored) pode espelhar tarefas; decisões de produto/arquitetura vivem **aqui**.

Referência Clean Architecture (casca SOAT): https://github.com/proferickmuller/soat-cleanarch-csharp

Relacionados: [Linguagem onipresente](00_linguagem-onipresente.md), [Requisitos fase 1](01_requisitos.md).

---

## Objetivos da fase

Evoluir a aplicação da Fase 1 para qualidade, resiliência e escalabilidade:

- Reduzir riscos operacionais com infraestrutura escalável
- Automatizar provisionamento e deploy
- Melhorar organização do código (Clean Code + Clean Architecture purista)
- Preparar para picos de demanda (HPA / escalabilidade dinâmica)

---

## Decisões fechadas

| Tema | Decisão |
|------|---------|
| Cluster | Local (**kind**); cloud = fase 03 |
| CI/CD | GitHub Actions + **self-hosted runner** → kind; **CI (build/test) cedo**; CD completo depois |
| Status externo / “via email” | Endpoint **público** (sem JWT) que chama os **mesmos use cases** de aprovar/rejeitar; sem SMTP; adapter externo, não gravar status arbitrário |
| Status OS | Manter status do domínio; listagem exclui **Finalizada, Entregue e Descartada**; ordenação evolutiva + mais antigas primeiro |
| Criação OS | Veículo amarra o cliente; contrato: **veículo + serviços + peças** (listas podem ser `[]`) |
| Estoque insuficiente | Domínio já envia OS para `AguardandoPeca` / item `EstoqueEmFalta` — não criar ItemEstoque “sem saldo” como contorno |
| Guid | Fora do escopo desta fase |
| Clean Arch | Visão **purista antes das APIs novas**: Presenters, Gateways, UseCases explícitos, Domain limpo de ORM |

---

## Ordem de trabalho

Não pular etapas:

1. Documentação (este doc, alinhamentos)
2. Rede de segurança: testes só do comportamento **já implementado** (sem TDD de feature futura)
3. **CI cedo** (build + testes no GitHub Actions)
4. **Refatoração Clean Architecture purista**
5. APIs novas da fase 02
6. Testes das APIs novas
7. Docker revisado → Kubernetes → Terraform → CD (deploy)
8. Entregáveis (README, collection, vídeo, PDF, share `soat-architecture`)
9. Melhorias opcionais por último

---

## Listagem de OS — ordenação e exclusões

**Excluir da listagem:** `Finalizada`, `Entregue`, `Descartada`.

**Prioridade evolutiva** (maior primeiro), depois data mais antiga (`RecebidaEm`):

1. `EmExecucao`
2. `LiberadaParaExecucao`
3. `AguardandoPeca`
4. `ChecandoEstoque`
5. `AguardandoAprovacao`
6. `EmDiagnostico`
7. `Recebida`

---

## APIs obrigatórias

| API | Estado atual | Gap |
|-----|--------------|-----|
| Abertura de OS | `POST` com `IdVeiculo` | Incluir serviços e peças (podem ser `[]`); cliente via veículo |
| Consulta de status | `GET /api/OrdemServico/{id}` | Garantir status claro na response |
| Aprovação de orçamento (ator externo) | Aprovar/rejeitar com JWT Admin | Adapter **público** → mesmos use cases |
| Listagem de OS | Não existe | Ordenação + exclusões acima |
| Atualização “externa” de status | — | Coberto pelo endpoint público + demo Swagger/Postman |

Implementar **depois** da refatoração Clean Arch.

---

## Clean Architecture (alvo)

Fluxo esperado:

`Controller` (DTO entrada) → `Use Case` → `Gateway` → Domain → Presenter (ViewModel) → Controller (HTTP)

Solution .NET:

- **Domain** — Entities, VOs, eventos; sem EF/ASP.NET
- **Application** — UseCases (+ Commands/Queries/Handlers se mantidos) e ports/Gateways
- **Infrastructure** — implementação de Gateways, EF Core, serviços externos
- **Api** — Controllers, Presenters, validators, DI/host

---

## Infraestrutura (resumo)

- Docker: Dockerfile + docker-compose (já existem; revisar)
- `/k8s`: Deployments, Services, ConfigMaps, Secrets, HPA
- `/infra`: Terraform (kind + banco) documentado
- GitHub Actions: CI (build/test) cedo; depois imagem, DB, apply manifests no kind via self-hosted runner

---

## Fora de escopo

- Migrar IDs para Guid
- Cloud (EKS/AKS/GKE) — fase 03
- SMTP / parser de e-mail
- ItemEstoque com saldo 0 só para contornar falta de peça
- Testes de features ainda não implementadas (antes da implementação)
- Cursor hooks (não fazem parte da base de AI desta fase)
