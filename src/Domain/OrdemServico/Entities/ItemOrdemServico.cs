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
    
    public void Descartar()
    {
        Status = StatusItemOrdemServico.Descartado;
    }

    public void AdicionarItemNecessario(ItemEstoqueOrdemServico.ItemNecessario itemNecessario)
    {
        var itemEstoqueOrdemServico = ItemEstoqueOrdemServico.Criar(itemNecessario);
        _itensNecessarios.Add(itemEstoqueOrdemServico);
    }
}