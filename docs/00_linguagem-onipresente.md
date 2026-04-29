# Dicionário de Linguagem Onipresente

## 1. Gestão de Ordem de Serviço

*   **Ordem de Serviço (OS):** O registro principal que centraliza todo o ciclo de vida do atendimento. Agrupa o cliente, veículo, itens de serviço e itens de estoque utilizados.
*   **Cliente:** O proprietário ou responsável financeiro pelo veículo em atendimento.
*   **Veículo:** O automóvel que está recebendo a manutenção, identificado unicamente pela sua placa.
*   **Item de Serviço:** Um serviço específico e individualizado adicionado a uma OS (ex: "Troca de Óleo"). Possui ciclo de vida, status próprio e valor.
*   **Item de Estoque na OS:** A relação de uma Peça ou Insumo que será utilizada na execução de um Item de Serviço específico dentro da OS.
*   **Retrabalho:** A ação de reabrir um Item de Serviço que já estava marcado como concluído devido à reprovação do cliente na entrega, exigindo a reversão do status da OS para execução.
*   **Contra-proposta:** Ação do mecânico ou atendente de alterar valores, itens ou peças de um Item de Serviço que foi previamente rejeitado pelo cliente, visando uma nova negociação.

### 1.1 Status da Ordem de Serviço (OS)
*   **Recebida:** A OS foi criada com os dados iniciais do cliente e do veículo, aguardando o início do atendimento pelo mecânico.
*   **Em diagnóstico:** O veículo está sob avaliação do mecânico, que está ativamente adicionando Itens de Serviço e Itens de Estoque à OS.
*   **Aguardando aprovação:** O diagnóstico foi concluído, os valores foram totalizados e a OS aguarda a resposta formal do cliente.
*   **Em revisão:** O cliente rejeitou um ou mais serviços, e a oficina está avaliando o cenário ou preparando uma contra-proposta.
*   **Em execução:** Pelo menos um Item de Serviço da OS está sendo executado ativamente pelo mecânico no momento.
*   **Finalizada:** Todos os Itens de Serviço (que não foram descartados) estão com o status *Concluído*. O veículo aguarda a avaliação final e retirada pelo cliente.
*   **Cancelada:** A OS foi abortada antes ou durante o orçamento. Estado final e irreversível.
*   **Entregue:** O cliente avaliou os serviços, aceitou o veículo e o atendimento foi encerrado. Estado final e irreversível.

### 1.2 Status do Item de Serviço
*   **Sugerido:** O serviço foi adicionado à OS pelo mecânico durante o diagnóstico, mas ainda não foi submetido à aprovação do cliente.
*   **Aprovado:** O cliente concordou formalmente com a execução e o valor do serviço.
*   **Rejeitado:** O cliente não concordou com a execução do serviço, abrindo margem para descarte ou contra-proposta.
*   **Em execução:** O mecânico está com a "mão na massa" trabalhando especificamente neste serviço.
*   **Aguardando Peça:** O serviço foi aprovado, mas sua execução ou conclusão está bloqueada devido à falta temporária da peça/insumo no estoque físico.
*   **Concluído:** O trabalho mecânico referente a este serviço foi terminado.
*   **Descartado:** O serviço foi removido definitivamente da OS (por decisão do cliente ou da oficina). Estado final para este item.
*   **Entregue:** A OS como um todo foi finalizada e entregue ao cliente, travando o serviço contra alterações. Estado final e irreversível.
*   
## 2. Administrativo (Cadastros Básicos)

*   **Cadastro de Cliente:** O registro completo do cliente contendo seus dados sensíveis e informações de contato.
*   **Cadastro de Veículo:** O registro completo do veículo, mantendo o histórico de propriedades e dados identificadores (placa, chassi).
*   **Serviço Padrão (Catálogo):** O cadastro base de mão de obra oferecida pela oficina com seu preço tabelado padrão, utilizado para preencher os Itens de Serviço da OS.
*   **CPF:** Cadastro de Pessoa Física. Documento de identificação para clientes pessoa física, exigindo validação estrita de formato e dígito verificador.
*   **CNPJ:** Cadastro Nacional da Pessoa Jurídica. Documento de identificação para clientes corporativos/frotistas, exigindo validação estrita de formato e dígito verificador.
*   **Placa:** Identificador alfanumérico do veículo. Contém regras de validação para suportar tanto o formato antigo (ABC-1234) quanto o formato Mercosul (ABC1D23).

## 3. Gestão de Estoque

*   **Item de Estoque:** A abstração geral para qualquer produto físico gerenciado pelo sistema.
*   **Peça:** Tipo de Item de Estoque contável e unitário, geralmente específico para montadoras ou modelos (ex: Pastilha de Freio, Correia Dentada).
*   **Insumo:** Tipo de Item de Estoque de uso genérico e muitas vezes fracionável (ex: Óleo de Motor, Graxa, Fluido de Freio).
*   **Estoque:** O controle da quantidade física e lógica disponível de um determinado Item de Estoque.
*   **Movimentação de Estoque:** O registro imutável de uma entrada (reabastecimento, ajuste positivo) ou saída (baixa, ajuste negativo) no estoque.
*   **Reserva Temporária:** Uma "trava" temporária no saldo do estoque atrelada a uma OS que está aguardando aprovação, garantindo que os itens orçados não sejam consumidos por outra OS concorrente.
*   **Baixa Definitiva:** A efetivação da saída de um item do estoque, ocorrendo quando um serviço é aprovado ou finalizado.
*   **Estoque Negativo:** Estado excepcional permitido pelo sistema onde o saldo de um item é menor que zero, resultante da aprovação de uma OS sem saldo físico suficiente.
*   **Reabastecimento:** A movimentação de entrada de novos itens no sistema, que automaticamente libera serviços que estavam travados pelo status "Aguardando Peça".