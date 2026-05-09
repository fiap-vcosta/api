using System.Diagnostics.CodeAnalysis;
using Domain.OrdemServico.ValueObjects;

namespace Domain.OrdemServico.Entities;

public enum StatusOrdemServico
{
    Recebida,
    EmDiagnostico,
    AguardandoAprovacao,
    EmExecucao,
    Finalizada,
    Paga,
    Entregue,
    Descartada
}

public class OrdemServicoAggregateRoot
{
    public int Id { get; private set; }
    
    public StatusOrdemServico Status { get; private set; }
    public DateTime RecebidaEm { get; private set; }
    public DateTime? EntregueEm { get; private set; }
    public DateTime? DescartadaEm { get; private set; }
    
    public required ClienteOrdemServico Cliente { get; init; }
    public required VeiculoOrdemServico Veiculo { get; init; }
    
    private readonly List<ItemOrdemServico> _itensServico = new();
    public IReadOnlyCollection<ItemOrdemServico> ItensServico => _itensServico.AsReadOnly();

    public static OrdemServicoAggregateRoot Criar(ClienteOrdemServico cliente, VeiculoOrdemServico veiculo)
    {
        return new OrdemServicoAggregateRoot
        {
            Cliente = cliente,
            Veiculo = veiculo,
            Status = StatusOrdemServico.Recebida,
            RecebidaEm = DateTime.UtcNow,
            EntregueEm = null,
            DescartadaEm = null
        };
    }

    public void EnviarParaDiagnostico()
    {
        this.Status = StatusOrdemServico.EmDiagnostico;
    }

    public void Descartar()
    {
        this.Status = StatusOrdemServico.Descartada;
        this.DescartadaEm = DateTime.UtcNow;

        this._itensServico.ForEach(item => item.Descartar());
    }
}