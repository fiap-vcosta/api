namespace Domain.OrdemServico.Entities;

public enum StatusItemOrdemServico
{
    Sugerido,
    Aprovado,
    Rejeitado,
    EmExecucao,
    AguardandoPeca,
    Concluido,
    Descartado,
    Pago,
    Entregue
} 

public class ItemOrdemServico
{
    public int Id {  get; private set; }
    public int IdOrdemServico { get; private set; }
    public StatusItemOrdemServico Status { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public decimal ValorCobrado { get; private set; }
    
    private readonly List<ItemEstoqueOrdemServico> _itensServico = new();
    public IReadOnlyCollection<ItemEstoqueOrdemServico> ItensNecessarios => _itensServico.AsReadOnly();
    
    public void Descartar()
    {
        Status = StatusItemOrdemServico.Descartado;
    }
}