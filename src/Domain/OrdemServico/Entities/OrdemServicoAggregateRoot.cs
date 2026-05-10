using System.Diagnostics.CodeAnalysis;
using Domain.Estoque.Entities;
using Domain.OrdemServico.ValueObjects;

namespace Domain.OrdemServico.Entities;

public enum StatusOrdemServico
{
    Recebida,
    EmDiagnostico,
    AguardandoAprovacao,
    AguardandoExecucao,
    EmExecucao,
    Finalizada,
    Paga,
    Descartada,
    Entregue,
}

public class OrdemServicoAggregateRoot
{
    public int Id { get; private set; }
    
    public StatusOrdemServico Status { get; private set; }

    public decimal ValorTotal =>
        _itensOrdemServico
            .Where(ios => ios.Status is not StatusItemOrdemServico.Rejeitado)
            .Sum(ios => ios.ValorCobrado);

    public DateTime RecebidaEm { get; private set; }
    public DateTime? EntregueEm { get; private set; }
    public DateTime? DescartadaEm { get; private set; }
    public DateTime? AprovadaEm { get; private set; }
    
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
    }

    public void FinalizarDiagnostico()
    {
        if (Status is not StatusOrdemServico.EmDiagnostico)
        {
            throw new InvalidOperationException($"Ordem de Serviço {Id} com status {Status} não pode ter diagnóstico finalizado.");
        }
        
        if (_itensOrdemServico.Count == 0)
        {
            throw new InvalidOperationException($"Ordem de Serviço {Id} não teve nenhum serviços adicionado.");
        }
        
        if (_itensOrdemServico.Any(ios => ios.Status is StatusItemOrdemServico.Sugerido))
        {
            Status = StatusOrdemServico.AguardandoAprovacao;

            return;
        }

        if (_itensOrdemServico.Any(ios => ios.Status is StatusItemOrdemServico.Aprovado))
        {
            EnviarParaExecucao();
            
            return;
        }
        
        Status = StatusOrdemServico.Entregue;
        EntregueEm = DateTime.UtcNow;
    }
    
    public void RejeitarServicosSugeridos()
    {
        if (Status is not StatusOrdemServico.AguardandoAprovacao)
        {
            throw new InvalidOperationException($"Ordem de Serviço {Id} com status {Status} não pode ter serviços rejeitados.");
        }

        Status = StatusOrdemServico.EmDiagnostico;
        
        _itensOrdemServico
            .Where(ios => ios.Status is StatusItemOrdemServico.Sugerido)
            .ToList()
            .ForEach(item => item.Rejeitar());
    }

    public void AprovarServicosSugeridos()
    {
        if (Status is not StatusOrdemServico.AguardandoAprovacao)
        {
            throw new InvalidOperationException($"Ordem de Serviço {Id} com status {Status} não pode ter serviços aprovados or rejeitados.");
        }
        
        _itensOrdemServico
            .Where(ios => ios.Status is StatusItemOrdemServico.Sugerido)
            .ToList()
            .ForEach(item => item.Aprovar());
        
        EnviarParaExecucao();
    }

    public void AprovarServicosParcialmente(List<int> idsItensServicoAprovados)
    {
        if (Status is not StatusOrdemServico.AguardandoAprovacao)
        {
            throw new InvalidOperationException($"Ordem de Serviço {Id} com status {Status} não pode ter serviços aprovados.");
        }

        foreach (var idItemServicoAprovado in idsItensServicoAprovados)
        {
            _itensOrdemServico
                .First(ios => ios.Id == idItemServicoAprovado)
                .Aprovar();
        }

        if (_itensOrdemServico.Any(ios => ios.Status is StatusItemOrdemServico.Sugerido))
        {
            RejeitarServicosSugeridos();
            return;
        }
        
        EnviarParaExecucao();
    }

    private void EnviarParaExecucao()
    {
        Status = StatusOrdemServico.AguardandoExecucao;
        AprovadaEm = DateTime.UtcNow;
    }
}