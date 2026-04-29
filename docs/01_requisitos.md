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

* **RF01 (Criação e Evolução da OS):** O sistema deve permitir a abertura e evolução da Ordem de Serviço em etapas:
  * **RF01.1:** Criação inicial da OS exigindo apenas a identificação do cliente (CPF/CNPJ e nome) e dados do veículo (placa, marca, modelo e ano), refletindo a recepção do carro.
  * **RF01.2:** Inclusão posterior de serviços sugeridos e peças/insumos necessários durante a fase de diagnóstico.
* **RF02 (Orçamento):** O sistema deve gerar um orçamento automaticamente, calculando os valores com base nos serviços e peças incluídos na OS.
* **RF03 (Comunicação):** O sistema deve realizar o envio do orçamento ao cliente para aprovação.
* **RF04 (Aprovação Parcial e Negociação):** Gestão granular da aprovação do cliente:
  * **RF04.1:** O cliente pode aprovar ou rejeitar serviços individualmente.
  * **RF04.2:** O mecânico pode enviar uma contra-proposta (alterar valor ou peças) para serviços rejeitados.
  * **RF04.3:** O mecânico ou atendente pode descartar definitivamente serviços rejeitados da OS.
* **RF05 (Gestão de Status da OS):** O sistema deve gerenciar o ciclo de vida da OS, permitindo estritamente as seguintes transições de status:
  * **RF05.1:** *Recebida* pode transitar para *Em diagnóstico* ou ser *Cancelada*.
  * **RF05.2:** *Em diagnóstico* pode transitar para *Aguardando aprovação* (orçamento gerado) ou ser *Cancelada*.
  * **RF05.3:** *Aguardando aprovação* pode transitar para *Em execução* (início dos trabalhos), *Em revisão* (cliente rejeitou itens e oficina analisa) ou ser *Cancelada*.
  * **RF05.4:** *Em revisão* pode transitar para *Aguardando aprovação* (nova proposta enviada), *Em execução* ou ser *Cancelada*.
  * **RF05.5:** *Em execução* pode transitar para *Finalizada* (trabalho concluído aguardando cliente), ou voltar para *Aguardando aprovação* / *Em revisão* (caso surja um imprevisto que exija novo orçamento).
  * **RF05.6:** *Finalizada* pode transitar para *Entregue* (cliente aceitou e levou o carro), *Em execução* (cliente reprovou a entrega e exigiu retrabalho) ou *Em diagnóstico* (cliente pediu um serviço extra de última hora).
  * **RF05.7:** *Cancelada* e *Entregue* são estados finais e irreversíveis.
* **RF06 (Máquina de Estados dos Serviços):** O sistema deve gerenciar o ciclo de vida individual de cada serviço dentro da OS, permitindo as seguintes transições:
  * **RF06.1:** *Sugerido* pode transitar para *Aprovado*, *Rejeitado* ou *Descartado* ou *Aguardando Peça* (falta de estoque).
  * **RF06.2:** *Rejeitado* pode transitar para *Sugerido* (mecânico alterou valores/peças para nova proposta) ou *Descartado*.
  * **RF06.4:** *Aguardando Peça* pode transitar exclusivamente para *Aprovado*.
  * **RF06.3:** *Aprovado* pode transitar exclusivamente para *Em execução*.
  * **RF06.5:** *Em execução* pode transitar excluisvamente para *Concluído* (mecânico terminou).
  * **RF06.6:** *Concluído* pode transitar para *Entregue* (cliente aceitou a OS como um todo) ou voltar para *Em execução* (em caso de retrabalho).
  * **RF06.7:** *Descartado* e *Entregue* são estados finais e irreversíveis.
* **RF07 (Automação de Status por Agregação):** O status da OS deve ser calculado e alterado automaticamente com base no ciclo de vida de seus serviços internos:
  * **RF07.1:** Se houver pelo menos um serviço *Em execução*, a OS deve mudar automaticamente para *Em execução*.
  * **RF07.2:** A OS só assume o status *Finalizada* quando **todos** os serviços ativos (não-descartados) estiverem com o status *Concluído*.
  * **RF07.3:** Quando a OS for movida manualmente pelo usuário para o status *Entregue*, o sistema deve automaticamente transitar todos os serviços com status *Concluído* para o status final *Entregue*.
  * **RF07.4:** Se houver serviços *Concluídos* e *Rejeitados*, mas nenhum *Em execução*, a OS deve assumir *Em revisão* ou *Aguardando aprovação*.
* **RF08 (Gestão de Retrabalho):** Fluxo para lidar com reprovação do cliente no momento da entrega do veículo:
  * **RF08.1:** O sistema deve permitir retornar um serviço específico do status *Concluído* de volta para *Em execução*.
  * **RF08.2:** Ao reabrir um serviço (RF08.1), o sistema deve reverter automaticamente a OS do status *Finalizada* para *Em execução* (conforme regra de agregação RF07.1).
  * **RF08.3:** Serviços que já atingiram o status *Entregue* não podem sofrer retrabalho na mesma OS.
* **RF09 (Aprovação com Estoque Insuficiente):** Tratamento de falta de peças em serviços aprovados:
  * **RF09.1:** Permitir a aprovação do serviço mesmo sem saldo físico, gerando um estoque negativo temporário da peça.
  * **RF09.2:** O serviço atrelado à peça faltante deve assumir o status *Aguardando Peça*, bloqueando sua ida para *Aprovado* ou *Concluído*.
  * **RF09.3:** O sistema deve transitar o status do serviço para *Aprovado* assim que o reabastecimento do estoque for registrado no sistema.
* **RF10 (Consulta do Cliente):** O sistema deve disponibilizar uma API para que o cliente possa consultar o progresso de sua OS e o status individual de cada serviço.
* **RF11 (Métricas):** O sistema deve monitorar o tempo médio de execução de cada serviço individual e da OS como um todo.
* **RF12 (Gestão Administrativa de OS):** Capacidade administrativa interna:
  * **RF12.1:** Listar ordens de serviço cadastradas com paginação e filtros.
  * **RF12.2:** Detalhar todas as informações, histórico e serviços de uma OS específica.

---

## 3. Requisitos Funcionais - Domínio Administrativo (Cadastros Básicos)

* **RF13 (Gestão de Clientes):** O sistema deve possuir CRUD completo de clientes.
* **RF14 (Gestão de Veículos):** O sistema deve possuir CRUD completo de veículos.
* **RF15 (Gestão de Serviços):** O sistema deve possuir CRUD completo de serviços base (catálogo de mão de obra).

---

## 4. Requisitos Funcionais - Domínio de Estoque

* **RF16 (Gestão de Estoque):** Gerenciamento do catálogo de produtos físicos:
  * **RF16.1:** CRUD de peças e insumos.
  * **RF16.2:** Registro de movimentação (entrada/saída) garantindo a auditoria de controle de estoque.
* **RF17 (Reserva Temporária de Peças):** Controle de concorrência de peças durante orçamentos:
  * **RF17.1:** Realizar a reserva temporária da peça no estoque quando um serviço é enviado para orçamento (Aguardando Aprovação).
  * **RF17.2:** Cancelar a reserva e devolver o saldo disponível caso o serviço seja rejeitado ou a OS cancelada.
  * **RF17.3:** Transformar a reserva em baixa definitiva caso o serviço seja aprovado (aplicando a regra do RF09 caso falte saldo físico).