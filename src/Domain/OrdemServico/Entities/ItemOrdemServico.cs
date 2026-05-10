namespace Domain.OrdemServico.Entities;

public enum StatusItemOrdemServico
{
    Sugerido,
    Aprovado,
    Rejeitado,
    EmExecucao,
    AguardandoPeca,
    Concluido,
    Pago,
    Entregue
} 

public class ItemOrdemServico
{
    public int Id {  get; private set; }
    public int IdOrdemServico { get; private set; }
    public StatusItemOrdemServico Status { get; private set; }
    
    public DateTime AprovadoEm { get; private set; }
    public DateTime RejeitadoEm { get; private set; }
    
    public string Nome { get; private set; } = string.Empty;
    public decimal ValorCobrado { get; private set; }
    
    private readonly List<ItemEstoqueOrdemServico> _itensNecessarios = new();
    public IReadOnlyCollection<ItemEstoqueOrdemServico> ItensNecessarios => _itensNecessarios.AsReadOnly();

    public static ItemOrdemServico Criar(string nome, decimal valorCobrado)
    {
        return new ItemOrdemServico
        {
            Status = StatusItemOrdemServico.Sugerido,
            Nome = nome,
            ValorCobrado = valorCobrado
        };
    }

    public void Rejeitar()
    {
        if (Status is not StatusItemOrdemServico.Sugerido)
        {
            throw new InvalidOperationException($"Item de Serviço {Id} com status {Status} não pode ser rejeitado.");
        }
        
        Status = StatusItemOrdemServico.Rejeitado;
        RejeitadoEm =  DateTime.UtcNow;
    }

    public void Aprovar()
    {
        if (Status is not StatusItemOrdemServico.Sugerido)
        {
            throw new InvalidOperationException($"Item de Serviço {Id} com status {Status} não pode ser aprovado.");
        }
        
        Status = StatusItemOrdemServico.Aprovado;
        AprovadoEm =  DateTime.UtcNow;
    }

    public void AdicionarItemNecessario(ItemEstoqueOrdemServico.ItemNecessario itemNecessario)
    {
        var itemEstoqueOrdemServico = ItemEstoqueOrdemServico.Criar(itemNecessario);
        _itensNecessarios.Add(itemEstoqueOrdemServico);
    }
}