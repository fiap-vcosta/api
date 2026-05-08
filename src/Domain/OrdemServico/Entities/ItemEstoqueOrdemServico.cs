using Domain.Estoque.Entities;

namespace Domain.OrdemServico.Entities;

public class ItemEstoqueOrdemServico
{
    public int Id {  get; private set; }
    public int IdOrdemServico { get; private set; }
    public int IdItemOrdemServico { get; private set; }
    public string Codigo { get; private set; } = string.Empty;
    public string Nome { get; private set; } = string.Empty;
    public UnidadeMedida UnidadeMedida { get; private set; }
    public decimal Quantidade { get; private set; }
}