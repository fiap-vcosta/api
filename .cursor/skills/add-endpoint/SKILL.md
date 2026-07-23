---
name: add-endpoint
description: >-
  Adiciona endpoint HTTP na Api (Request, Validator, action no Controller, Presenter)
  mapeando para um use case existente. Use ao criar ou expor APIs REST.
---

# Add Endpoint

## Quando usar

Expor um use case via HTTP (CRUD, transição de OS, endpoint público de aprovação, listagem).

## Passos

1. Garantir que o **use case já existe** (skill `add-usecase` antes, se preciso).
2. Espelhar pasta de ação vizinha, ex.: `src/Api/Controllers/Cliente/CreateCliente/`.
3. Criar:
   - `*Request.cs`
   - `*RequestValidator.cs` (mesmo padrão de validação do projeto)
   - Action no `*Controller.cs` do recurso
   - **Presenter** + ViewModel/Response HTTP quando o alvo Clean Arch já tiver Presenters; se ainda não, mapear no controller de forma mínima e deixar nota para Presenter
4. Mapear Request → Command/Query; resultado → Presenter → `Ok` / `Created` / `NotFound` / Problem Details.
5. Autorização: Admin JWT por padrão; **exceção** — aprovação/rejeição externa **sem** `[Authorize]`, chamando os mesmos use cases.
6. Não colocar regra de negócio no controller.
7. Só então adicionar testes de integração do endpoint.

## Referência

- Controllers atuais em `src/Api/Controllers/`
- `docs/01_requisitos.md` para regras de OS
