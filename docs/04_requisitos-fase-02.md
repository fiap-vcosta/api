# Documento de Requisitos — Fase 02

> **Fase 2.** Evolui a aplicação da Fase 1 para qualidade, resiliência e escalabilidade.  
> Requisitos históricos da **Fase 1**: [`01_requisitos.md`](01_requisitos.md) (permanecem válidos salvo onde um RF/RNF abaixo **substitui** explicitamente).  
> Índice: [`docs/README.md`](README.md) · Vitrine: [`README.md`](../README.md).  
> Referência Clean Architecture: https://github.com/proferickmuller/soat-cleanarch-csharp

## Objetivos

* Reduzir riscos operacionais com infraestrutura escalável.
* Automatizar o provisionamento e o deploy do ambiente.
* Melhorar a qualidade e a organização do código (Clean Code + Clean Architecture).
* Preparar a aplicação para picos de demanda com escalabilidade dinâmica (HPA).

---

## 1. Requisitos Não-Funcionais (Técnicos e Globais)

* **RNF11 (Arquitetura):** Refatorar o código aplicando Clean Code e Clean Architecture, com separação de camadas e dependências apontando para dentro:
  * **RNF11.1:** Domain sem dependência de EF/ASP.NET (sem atributos de ORM no domínio).
  * **RNF11.2:** Application com Use Cases explícitos e ports (**Gateways**).
  * **RNF11.3:** Api com Controllers finos e **Presenters** montando ViewModels/Responses.
  * **RNF11.4:** Infrastructure implementando Gateways (EF Core, JWT, etc.).
* **RNF12 (Qualidade):** Manter testes automatizados (unitários e de integração) dos fluxos críticos, incluindo as APIs novas, com cobertura mínima de **80%** line e branch nos assemblies críticos. Ver [`docs/tests/README.md`](tests/README.md).
* **RNF13 (Containerização):** Garantir a aplicação containerizada com Dockerfile atualizado e `docker-compose` para desenvolvimento local (imagem não-root; variáveis sensíveis via env, sem defaults hardcoded de segredo).
* **RNF14 (Orquestração):** Disponibilizar manifestos Kubernetes em `/k8s`, contemplando:
  * **RNF14.1:** Deployments e Services.
  * **RNF14.2:** ConfigMaps e Secrets para variáveis sensíveis.
  * **RNF14.3:** Horizontal Pod Autoscaler (HPA) por consumo de CPU e memória.
* **RNF15 (Infraestrutura como Código):** Provisionar com Terraform em `/infra`:
  * **RNF15.1:** Cluster Kubernetes **local** (**kind**); cloud fica para fase posterior.
  * **RNF15.2:** Banco de dados (PostgreSQL no cluster).
  * **RNF15.3:** metrics-server (via Helm) para viabilizar o HPA.
  * **RNF15.4:** Documentar recursos criados e como aplicar ([`05_infraestrutura-kind-terraform.md`](05_infraestrutura-kind-terraform.md)).
* **RNF16 (CI/CD):** Pipeline no GitHub Actions que execute:
  * **RNF16.1 (CI):** Build da aplicação e execução dos testes automatizados (a cada push).
  * **RNF16.2 (CD):** Build da imagem Docker, deploy do banco (migrations no startup da API), aplicação dos manifestos no cluster kind via **self-hosted runner**.
* **RNF17 (Escalabilidade):** A API deve escalar horizontalmente no cluster conforme carga (HPA), demonstrável sob stress ([`06_kubernetes-api.md`](06_kubernetes-api.md)).
* **RNF18 (Documentação):** `README.md` atualizado com descrição/objetivos da fase, diagramas de arquitetura (C4 e infra/deploy), instruções de execução local, Terraform e Kubernetes, link da collection de APIs e link do vídeo demonstrativo.
* **RNF19 (Segurança operacional):** Credenciais e chaves (Postgres, JWT) via Secrets do Kubernetes / secrets do Actions — não embutidas na imagem.
* **RNF20 (Observabilidade operacional):** Manter endpoint de health para verificação da aplicação em runtime (alinhado ao RNF09 da Fase 1).

---

## 2. Requisitos Funcionais — Ordem de Serviço (evolução)

A máquina de estados e as regras de domínio da Fase 1 (**RF02–RF10**, **RF05–RF08**, etc. em [`01_requisitos.md`](01_requisitos.md)) permanecem, salvo os itens abaixo.

* **RF17 (Abertura de OS):** *(substitui parcialmente RF01.1)* O sistema deve permitir abrir uma OS recebendo **veículo**, **serviços** e **peças** (listas de serviços/peças podem ser ausentes ou `[]`), retornando a identificação única da OS. O cliente é obtido pelo vínculo do veículo.
  * **RF17.1:** Serviços/peças informados na criação são adicionados com a OS em `Recebida`; o fluxo de criação segue para `EmDiagnostico` sem finalizar o diagnóstico automaticamente.
* **RF18 (Adição de serviços e peças):** *(substitui RF01.3)* O domínio só permite adicionar itens de serviço/peças quando a OS estiver em **`Recebida`** ou **`EmDiagnostico`**. Qualquer outro status deve ser rejeitado por regra de negócio.
* **RF19 (Consulta de status):** O sistema deve expor consulta da OS por id informando a situação atual (status do domínio: Recebida, Em Diagnóstico, Aguardando Aprovação, Execução, Finalizada, Entregue, etc.).
  * Endpoint: `GET /api/OrdemServico/{id}` (JWT Admin).
* **RF20 (Listagem de OS):** O sistema deve listar ordens de serviço ativas com as regras:
  * **RF20.1 (Exclusão lógica da listagem):** Não incluir OS em `Finalizada`, `Entregue` ou `Descartada`.
  * **RF20.2 (Ordenação):** Prioridade evolutiva de status (maior prioridade primeiro), depois as mais antigas (`RecebidaEm`). Ordem de prioridade:
    1. `EmExecucao`
    2. `LiberadaParaExecucao`
    3. `AguardandoPeca`
    4. `ChecandoEstoque`
    5. `AguardandoAprovacao`
    6. `EmDiagnostico`
    7. `Recebida`  
    (Cumpre e estende a ordem mínima do enunciado: Em Execução > Aguardando Aprovação > Diagnóstico > Recebida.)
  * Endpoint: `GET /api/OrdemServico` (JWT Admin).
* **RF21 (Aprovação de orçamento — ator externo):** *(substitui o canal “e-mail/SMTP” do RF03 / enunciado)* O sistema deve receber aprovação ou recusa do orçamento via endpoint **público** (sem JWT), localizando a OS por **token opaco**.
  * **RF21.1:** Os endpoints públicos devem acionar os **mesmos** use cases de aprovar/rejeitar já usados pela API administrativa.
  * **RF21.2:** O token não deve ser exposto nas responses de criação/consulta da API.
  * **RF21.3:** Não há envio SMTP nem parser de e-mail nesta fase.
  * Endpoints: `POST /api/public/ordens-servico/aprovar?token=` e `POST /api/public/ordens-servico/rejeitar?token=`.
* **RF22 (Estoque insuficiente):** Em falta de peça, o domínio deve seguir o comportamento já existente (`AguardandoPeca` / `EstoqueEmFalta`). Não criar ItemEstoque com saldo 0 apenas para contornar a regra.
