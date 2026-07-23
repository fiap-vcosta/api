---
name: review-clean-arch
description: >-
  Revisa alterações quanto a Clean Architecture purista (camadas, Presenter,
  Gateway, Domain sem ORM). Use após refactors ou PRs de arquitetura.
---

# Review Clean Arch

## Checklist

- [ ] Domain sem referência a EF/ASP.NET; sem anotações ORM novas
- [ ] Application não referencia Infrastructure; só ports
- [ ] Controllers sem regra de negócio; Presenter monta a response HTTP
- [ ] Interfaces de persistência como Gateway (ou Repository justificado como Gateway)
- [ ] Fluent API / migrations só em Infrastructure
- [ ] Use case único ponto que muta agregado (adapters não gravam status direto)
- [ ] Dependências apontam para dentro
- [ ] Alinhado a `docs/01_requisitos.md` e casca https://github.com/proferickmuller/soat-cleanarch-csharp

## Saída

Listar ** Achados** (bloqueantes) e **Sugestões**. Citar arquivos/caminhos concretos. Não reescrever o PR inteiro — só o que viola o círculo.
