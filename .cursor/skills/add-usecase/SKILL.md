---
name: add-usecase
description: >-
  Adiciona um novo use case (Command ou Query + Handler + Response) na Application
  espelhando a estrutura existente do Tech Challenge. Use ao criar casos de uso,
  handlers MediatR ou pastas em Application/UseCases.
---

# Add Use Case

## Quando usar

Novo fluxo de aplicação (criar/atualizar/consultar/transição de OS, etc.).

## Passos

1. Confirmar em `docs/04_requisitos-fase-02.md` que a feature é permitida **agora** (Clean Arch antes de APIs novas; não inventar requisito).
2. Espelhar um use case vizinho no mesmo bounded context (ex.: `Application/Administrativo/Cliente/Commands/CreateCliente/`).
3. Criar pasta dedicada com:
   - `*Command.cs` ou `*Query.cs` (`IRequest<TResponse>`)
   - `*Handler.cs`
   - `*Response.cs` quando a saída não for `Unit` / entity crua (preferir response dedicado)
4. Se a estrutura já tiver `UseCases/`, colocar lá; senão seguir Commands/Queries atuais e alinhar na refatoração.
5. Depender só de interfaces de Gateway/Repository + Domain — sem EF/HTTP.
6. Registrar nada extra se MediatR já faz assembly scanning; senão registrar no DI existente.
7. Rodar `dotnet build` e testes unitários do handler novo **somente** depois do use case existir.

## Não fazer

- Não colocar Presenter ou `IActionResult` na Application.
- Não retornar aggregate root para a Api se o fluxo vizinho já usa Response — manter consistência.
