# Documentação

Ponto de entrada da documentação versionada. A avaliação da Fase 02 começa pelo [`README.md`](../README.md) na raiz (objetivos, C4, infra, deploy, collections, vídeo).

## Domínio (Fase 1)

| Doc | Conteúdo |
|-----|----------|
| [00_linguagem-onipresente.md](00_linguagem-onipresente.md) | Vocabulário do domínio |
| [01_requisitos.md](01_requisitos.md) | Requisitos funcionais e não funcionais da **Fase 1** |
| `02_*.egn` | Histórias de domínio (EventStorming / tooling) |
| `03_01_event-storming-linha-do-tempo.jpg` | Linha do tempo |
| `03_02_event-storming-agregados.jpg` | Agregados |

## Fase 02 (deltas e decisões)

| Doc | Conteúdo |
|-----|----------|
| [04_requisitos-fase-02.md](04_requisitos-fase-02.md) | Objetivos, o que mudou vs Fase 1, APIs entregues, decisões (kind, CI/CD, token público, listagem) |

## Infraestrutura (how-to)

| Doc | Conteúdo |
|-----|----------|
| [05_infraestrutura-kind-terraform.md](05_infraestrutura-kind-terraform.md) | Terraform, kind, Postgres, metrics-server, self-hosted runner |
| [06_kubernetes-api.md](06_kubernetes-api.md) | Manifests `/k8s`, deploy da API, HPA / stress |

## Testes

| Doc | Conteúdo |
|-----|----------|
| [tests/README.md](tests/README.md) | Unitários, integração e e2e HTTP (visão geral) |

## API e collections

| Doc | Conteúdo |
|-----|----------|
| [api/README.md](api/README.md) | Swagger + Requestly (exploratória e e2e) |

## ADRs

| Doc | Conteúdo |
|-----|----------|
| [adrs/001-escolha-banco-de-dados.md](adrs/001-escolha-banco-de-dados.md) | Escolha do PostgreSQL |

## Referência Clean Architecture

https://github.com/proferickmuller/soat-cleanarch-csharp
