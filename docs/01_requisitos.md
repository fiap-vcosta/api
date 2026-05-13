# Documento de Requisitos - Sistema de Gestão de Oficina Mecânica

## 1. Requisitos Não-Funcionais (Técnicos e Globais)

* **RNF01 (Segurança):** Implementação de autenticação JWT para proteger as APIs administrativas.
* **RNF02 (Segurança/Domínio):** Validação obrigatória e rigorosa de dados sensíveis:
  * **RNF02.1:** Validação de formato e integridade de CPF.
  * **RNF02.2:** Validação de formato e integridade de CNPJ.
  * **RNF02.3:** Validação do formato da placa do veículo (suporte a padrão Mercosul e antigo).
* **RNF03 (Documentação):** As APIs devem seguir o padrão RESTful e ser documentadas via Swagger.
* **RNF04 (Documentação):** Configuração explicada no `README.md` com instruções para para execução local.
* **RNF05 (Infraestrutura):** Orquestração do ambiente completo utilizando `docker-compose.yml`.
* **RNF06 (Qualidade):** Implementação de testes unitários e de integração para os principais fluxos, garantindo uma cobertura mínima de 80% nos domínios críticos.
* **RNF07 (Padronização de API):** A API deve retornar erros no formato padrão RFC 7807 (Problem Details for HTTP APIs), garantindo uma experiência de integração previsível.
* **RNF08 (Performance e Usabilidade):** Todas as rotas de listagem (Clientes, Veículos, OS) devem implementar paginação (limit/offset) e filtros básicos.
* **RNF09 (Resiliência):** Implementar endpoint `/health/` para verificar a saúde da aplicação.
* **RNF10 (Integridade de Dados):** Exclusões de clientes e veículos devem utilizar *Soft Delete* (marcá-los como inativos via `deleted_at`).
---

## 2. Requisitos Funcionais - Domínio Core (Ordem de Serviço)

* **RF01 (Criação e Diagnóstico da OS):** O sistema deve permitir a evolução da OS em etapas:
  * **RF01.1:** Criação gerando a OS no status Recebida, exigindo Cliente e Veículo.
  * **RF01.2:** Transição explícita para Em Diagnóstico, habilitando a adição de serviços e seus itens necessários (peças/insumos).
  * **RF01.3:** O sistema só permite adicionar itens quando a OS estiver estritamente Em Diagnóstico.
* **RF02 (Finalização do Diagnóstico):** O fechamento do diagnóstico deve rotear a OS automaticamente:
  * **RF02.1:** Se houver serviços Sugeridos, a OS transita para Aguardando Aprovação.
  * **RF02.2:** Se não houver serviços Sugeridos, mas houver serviços previamente Aprovados, a OS transita para Checando Estoque.
  * **RF02.3:** Se todos os serviços já estiverem processados sem nada a fazer (ex:** todos rejeitados), a OS é transitada diretamente para Entregue (encerrada).
* **RF03 (Orçamento e Comunicação):** O sistema deve gerar o orçamento totalizando o valor cobrado dos serviços não-rejeitados e enviá-lo ao cliente para aprovação.
* **RF04 (Fluxo de Aprovação e Rejeição):** Gestão da resposta do cliente:
  * **RF04.1 (Aprovação Total):** Ao aprovar todos os serviços, o status dos itens muda para Aprovado e a OS transita para Checando Estoque.
  * **RF04.2 (Rejeição Total):** Ao rejeitar os serviços sugeridos, seus status mudam para Rejeitado e a OS retrocede automaticamente para o status Em Diagnóstico.
  * **RF04.3 (Aprovação Parcial):** O sistema deve aceitar a aprovação de apenas uma lista específica de serviços. Se restarem itens Sugeridos não aprovados, o sistema os marcará como Rejeitados e recuará a OS para Em Diagnóstico. Caso contrário, avança para Checando Estoque.
* **RF05 (Máquina de Estados da OS):** A entidade deve travar transições inválidas, obedecendo o seguinte fluxo principal:
    Recebida ➔ Em Diagnóstico ou Descartada.
    Em Diagnóstico ➔ Aguardando Aprovação, Checando Estoque, Entregue ou Descartada.
    Aguardando Aprovação ➔ Em Diagnóstico (rejeição) ou Checando Estoque (aprovação).
    Checando Estoque / Aguardando Peça ➔ Liberada para Execução ou Aguardando Peça.
    Liberada para Execução / Em Execução ➔ Finalizada.
    Finalizada ➔ Entregue.
* **RF06 (Máquina de Estados dos Serviços):** O ciclo interno do serviço obedece ao fluxo:** Sugerido ➔ Aprovado ou Rejeitado. O Aprovado transita para Concluído mediante confirmação de execução.
* **RF07 (Checagem e Liberação de Estoque):** O sistema deve processar a viabilidade de execução com base no estoque:
  * **RF07.1:** O sistema deve avaliar cada item necessário dos serviços aprovados comparando sua quantidade exigida com um dicionário de saldos disponíveis injetado no domínio.
  * **RF07.2:** Cada item receberá o status Estoque Disponível ou Estoque Em Falta. O saldo iterado na memória deve ser deduzido durante a checagem na mesma OS para evitar falsos positivos de múltiplos itens exigindo a mesma peça.
  * **RF07.3:** A OS transitará para Liberada Para Execução apenas se todos os itens apontarem Estoque Disponível. Caso contrário, transitará para Aguardando Peça.
* **RF08 (Execução e Conclusão):** O processamento do trabalho mecânico:
  * **RF08.1:** A confirmação de execução requer o envio das datas de início e término de cada serviço específico.
  * **RF08.2:** O sistema marcará os serviços indicados como Concluído e os seus itens necessários consumidos receberão o status Utilizado.
  * **RF08.3:** A OS só assume o status Finalizada quando todos os serviços assumirem o status Concluído.
* **RF09 (Métricas):** O sistema deve calcular o valor total da OS somando dinamicamente os valores dos serviços que não estão no status Rejeitado.
* **RF10 (Descarte):** A OS só pode ser Descartada enquanto estiver nos estados de origem (Recebida ou Em Diagnóstico).

---

## 3. Requisitos Funcionais - Domínio Administrativo (Cadastros Básicos)

* **RF11 (Gestão de Clientes):** O sistema deve possuir CRUD completo de clientes.
* **RF12 (Gestão de Veículos):** O sistema deve possuir CRUD completo de veículos.
* **RF13 (Gestão de Serviços):** O sistema deve possuir CRUD completo de serviços base (catálogo de mão de obra).

## 4. Requisitos Funcionais - Domínio de Estoque

* **RF14 (Gestão de Estoque Físico):** Gerenciamento do catálogo:
  * **RF14.1:** CRUD de itens de estoque (peças e insumos).
  * **RF14.2:** Manter o controle do saldo disponível.
* **RF15 (Trava e Utilização de Peças):** A garantia do inventário na OS:
  * **RF15.1:** Quando uma OS é liberada, a aplicação deve invocar a Trava de Estoque nos itens necessários, alterando seu status interno para Estoque Travado.
  * **RF15.2:** No momento em que a conclusão do serviço é confirmada (RF08.2), o sistema deve converter o status dos itens da trava para Utilizado.

---

## 5. Requisitos Funcionais - Domínio de Pagamento
* ** RF16 (Fluxo de Pagamento e Entrega):** Encerramento da ordem de serviço.
  * **RF16.1:** Integração com Gateway de Pagamento externo para processamento financeiro.
  * **RF16.2:** A confirmação do pagamento só pode ocorrer se a OS estiver Finalizada.
  * **RF16.3:** O registro bem-sucedido do pagamento transita imediatamente a OS para o estado final Entregue, representando a liberação do veículo ao cliente.
