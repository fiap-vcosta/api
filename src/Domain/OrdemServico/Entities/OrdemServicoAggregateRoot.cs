using System.Diagnostics.CodeAnalysis;
using Domain.Estoque.Entities;
using Domain.OrdemServico.ValueObjects;

namespace Domain.OrdemServico.Entities;

public enum StatusOrdemServico
{
    Recebida,
    EmDiagnostico,
    AguardandoAprovacao,
    Rejeitada,
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
    public decimal ValorTotal { get; private set; }
    
    public DateTime RecebidaEm { get; private set; }
    public DateTime? EntregueEm { get; private set; }
    public DateTime? DescartadaEm { get; private set; }
    public DateTime? RejeitadaEm { get; private set; }
    
    public required ClienteOrdemServico Cliente { get; init; }
    public required VeiculoOrdemServico Veiculo { get; init; }
    
    private readonly List<ItemOrdemServico> _itensOrdemServico = new();
    public IReadOnlyCollection<ItemOrdemServico> ItensOrdemServico => _itensOrdemServico.AsReadOnly();

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
        if (Status is not StatusOrdemServico.Recebida)
        {
            throw new InvalidOperationException($"Ordem de Serviço {Id} com status {Status} não pode ser enviada para diagnístico.");
        }
        
        Status = StatusOrdemServico.EmDiagnostico;
    }

    public void Descartar()
    {
        if (Status is not (StatusOrdemServico.Recebida or StatusOrdemServico.EmDiagnostico))
        {
            throw new InvalidOperationException($"Ordem de Serviço {Id} com status {Status} não pode ser descartada.");
        }
        
        Status = StatusOrdemServico.Descartada;
        DescartadaEm = DateTime.UtcNow;
    }

    public void Rejeitar()
    {
        if (Status is not StatusOrdemServico.AguardandoAprovacao)
        {
            throw new InvalidOperationException($"Ordem de Serviço {Id} com status {Status} não pode ser rejeitada.");
        }

        Status = StatusOrdemServico.Rejeitada;
        RejeitadaEm = DateTime.UtcNow;
        
        _itensOrdemServico.ForEach(item => item.Rejeitar());
    }

    public void AdicionarItemServico(string nome, decimal valorCobrado, List<ItemEstoqueOrdemServico.ItemNecessario> itensNecessarios)
    {
        if (Status is not StatusOrdemServico.EmDiagnostico)
        {
            throw new InvalidOperationException($"Ordem de Serviço {Id} com status {Status} não pode ter itens adicionados.");
        }

        var itemOrdemServico = ItemOrdemServico.Criar(nome, valorCobrado);
        foreach (var itemNecessario in itensNecessarios)
        {
            itemOrdemServico.AdicionarItemNecessario(itemNecessario);
        }
        
        _itensOrdemServico.Add(itemOrdemServico);
        ValorTotal = _itensOrdemServico.Sum(ios => ios.ValorCobrado);
    }

    public void FinalizarDiagnostico()
    {
        if (Status is not StatusOrdemServico.EmDiagnostico)
        {
            throw new InvalidOperationException($"Ordem de Serviço {Id} com status {Status} não pode ter diagnóstico finalizado.");
        }

        if (_itensOrdemServico.Count == 0)
        {
            throw new InvalidOperationException($"Ordem de Serviço {Id} não possui itens de serviço para serem realiazdos.");
        }
        
        Status = StatusOrdemServico.AguardandoAprovacao;
    }
}