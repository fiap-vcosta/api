# Dicionário de Linguagem Onipresente

## 1. Gestão de Ordem de Serviço

*   **Ordem de Serviço (OS):** O registro principal que centraliza todo o ciclo de vida do atendimento. Agrupa o cliente, veículo, itens de serviço e itens necessários (estoque).
*   **Cliente:** O proprietário ou responsável financeiro pelo veículo em atendimento.
*   **Veículo:** O automóvel que está recebendo a manutenção, identificado unicamente pela sua placa.
*   **Serviço (Item de Serviço):** Um serviço específico adicionado a uma OS durante o diagnóstico. Possui ciclo de vida simplificado (Sugerido, Aprovado, Rejeitado, Concluído) e registra os horários de início e fim da sua execução.
*   **Item Necessário:** A relação de uma peça ou insumo de estoque vinculada a um serviço específico dentro da OS. Possui uma verificação de status própria em relação ao saldo do estoque.
*   **Serviço SMTP:** O serviço de e-mail utilizado para enviar notificações aos clientes.
*   **Pagamento/Entrega:** A transação financeira que finaliza o ciclo de vida da OS, movendo-a para o estado irreversível de Entregue.

### 1.1 Status da Ordem de Serviço (OS)
*   **Recebida:** A OS foi criada com cliente e veículo, aguardando início do diagnóstico.
*   **Em Diagnóstico:** O veículo está sendo avaliado. Serviços e itens necessários podem ser adicionados. É também para este status que a OS retorna caso haja rejeição de itens sugeridos.
*   **Aguardando Aprovação:** O diagnóstico foi finalizado contendo itens sugeridos e aguarda a decisão do cliente.
*   **Checando Estoque:** A OS possui serviços aprovados e o sistema está validando se as quantidades exigidas pelos Itens Necessários estão disponíveis no inventário físico.
*   **Aguardando Peça:** A checagem revelou que a quantidade solicitada em algum Item Necessário é maior do que o saldo disponível, bloqueando a execução da OS até o reabastecimento.
*   **Liberada Para Execução:** A checagem de estoque foi bem-sucedida (todos os itens possuem disponibilidade). O mecânico está autorizado a iniciar os trabalhos.
*   **Em Execução:** Os trabalhos mecânicos foram iniciados na OS.
*   **Finalizada:** O sistema confirmou a execução e a conclusão de todos os serviços aprovados.
*   **Entregue:** O pagamento foi confirmado ou a OS foi finalizada sem aprovação de serviços. É o estado final de sucesso e encerramento.
*   **Descartada:** A OS foi abandonada ou cancelada antes da aprovação. Estado final.

### 1.2 Status do Serviço (Item de Serviço)
*   **Sugerido:** O serviço foi recém-adicionado à OS durante o diagnóstico.
*   **Aprovado:** O cliente concordou com a execução do serviço.
*   **Rejeitado:** O cliente recusou a execução do serviço.
*   **Concluído:** O trabalho mecânico referente a este serviço foi terminado, marcando a data/hora de início e fim.
   
### 1.3 Status do Item Necessário (Estoque na OS)
*   **Estoque Não Checado:** O item foi adicionado à OS, mas seu saldo ainda não foi confrontado com o estoque físico.
*   **Estoque Em Falta:** A quantidade exigida para este item é superior ao saldo disponível na oficina.
*   **Estoque Disponível:** Existe saldo suficiente para atender à demanda deste item na OS.
*   **Estoque Travado:** A quantidade necessária foi reservada (travada) sistemicamente para garantir a execução desta OS.
Utilizado: A execução do serviço foi concluída e o item foi efetivamente gasto/consumido da trava.

## 2. Administrativo (Cadastros Básicos)

*   **Cadastro de Cliente:** O registro completo do cliente contendo seus dados sensíveis e informações de contato.
*   **Cadastro de Veículo:** O registro completo do veículo, mantendo o histórico de propriedades e dados identificadores (placa, chassi).
*   **Serviço Padrão (Catálogo):** O cadastro base de mão de obra oferecida pela oficina com seu preço tabelado padrão, utilizado para preencher os Itens de Serviço da OS.
*   **CPF:** Cadastro de Pessoa Física. Documento de identificação para clientes pessoa física, exigindo validação estrita de formato e dígito verificador.
*   **CNPJ:** Cadastro Nacional da Pessoa Jurídica. Documento de identificação para clientes corporativos/frotistas, exigindo validação estrita de formato e dígito verificador.
*   **Placa:** Identificador alfanumérico do veículo. Contém regras de validação para suportar tanto o formato antigo (ABC-1234) quanto o formato Mercosul (ABC1D23).

## 3. Gestão de Estoque
*   **Item de Estoque:** A abstração geral para qualquer produto físico gerenciado pelo sistema.
*   **Peça:** Produto contável e unitário.
*   **Insumo:** Produto de uso genérico e fracionável.
*   **Movimentação de Estoque:** O registro de entrada ou saída no estoque.
*   **Trava de Estoque:** A garantia sistêmica de que o Estoque Disponível verificado na OS não será consumido por outras ordens de serviço