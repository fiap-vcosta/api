using System.ComponentModel.DataAnnotations.Schema;
using Domain.OrdemServico.ValueObjects;

namespace Domain.OrdemServico.Entities;

public enum StatusOrdemServico
{
    Recebida,
    EmDiagnostico,
    AguardandoAprovacao,
    ChecandoEstoque,
    AguardandoPeca,
    LiberadaParaExecucao,
    EmExecucao,
    Finalizada,
    Descartada,
    Entregue,
}

public class OrdemServicoAggregateRoot
{
    public int Id { get; private set; }
    
    public StatusOrdemServico Status { get; private set; }

    public decimal ValorTotal =>
        _servicos
            .Where(ios => ios.Status is not StatusItemOrdemServico.Rejeitado)
            .Sum(ios => ios.ValorCobrado);

    public DateTime RecebidaEm { get; private set; }
    public DateTime? EntregueEm { get; private set; }
    public DateTime? DescartadaEm { get; private set; }
    public DateTime? AprovadaEm { get; private set; }
    
    public required ClienteOrdemServico Cliente { get; init; }
    public required VeiculoOrdemServico Veiculo { get; init; }
    
    private readonly List<Servico> _servicos = new();
    public IReadOnlyCollection<Servico> Servicos => _servicos.AsReadOnly();

    [NotMapped]
    public IEnumerable<ItemNecessario> ItensNecessariosParaExecucao => _servicos
        .Where(s => s.Status is StatusItemOrdemServico.Aprovado or StatusItemOrdemServico.Sugerido)
        .SelectMany(s => s.ItensNecessarios);

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

    public void AdicionarItemServico(string nome, decimal valorCobrado, List<ItemNecessario.CriarItemNecessarioParams> itensNecessarios)
    {
        if (Status is not StatusOrdemServico.EmDiagnostico)
        {
            throw new InvalidOperationException($"Ordem de Serviço {Id} com status {Status} não pode ter itens adicionados.");
        }

        var itemOrdemServico = Servico.Criar(nome, valorCobrado);
        foreach (var itemNecessario in itensNecessarios)
        {
            itemOrdemServico.AdicionarItemNecessario(itemNecessario);
        }
        
        _servicos.Add(itemOrdemServico);
    }

    public void FinalizarDiagnostico()
    {
        if (Status is not StatusOrdemServico.EmDiagnostico)
        {
            throw new InvalidOperationException($"Ordem de Serviço {Id} com status {Status} não pode ter diagnóstico finalizado.");
        }
        
        if (_servicos.Count == 0)
        {
            throw new InvalidOperationException($"Ordem de Serviço {Id} não teve nenhum serviços adicionado.");
        }
        
        if (_servicos.Any(ios => ios.Status is StatusItemOrdemServico.Sugerido))
        {
            Status = StatusOrdemServico.AguardandoAprovacao;

            return;
        }

        if (_servicos.Any(ios => ios.Status is StatusItemOrdemServico.Aprovado))
        {
            EnviarParaChecagemEstoque();
            
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
        
        foreach (var servico in _servicos.Where(s => s.Status is StatusItemOrdemServico.Sugerido))
        {
            servico.Rejeitar();
        }
    }

    public void AprovarServicosSugeridos()
    {
        if (Status is not StatusOrdemServico.AguardandoAprovacao)
        {
            throw new InvalidOperationException($"Ordem de Serviço {Id} com status {Status} não pode ter serviços aprovados or rejeitados.");
        }
        
        foreach (var servico in _servicos.Where(s => s.Status is StatusItemOrdemServico.Sugerido))
        {
            servico.Aprovar();
        }
        
        EnviarParaChecagemEstoque();
    }

    public void AprovarServicosParcialmente(List<int> idsItensServicoAprovados)
    {
        if (Status is not StatusOrdemServico.AguardandoAprovacao)
        {
            throw new InvalidOperationException($"Ordem de Serviço {Id} com status {Status} não pode ter serviços aprovados.");
        }

        foreach (var idItemServicoAprovado in idsItensServicoAprovados)
        {
            _servicos
                .First(ios => ios.Id == idItemServicoAprovado)
                .Aprovar();
        }

        if (_servicos.Any(ios => ios.Status is StatusItemOrdemServico.Sugerido))
        {
            RejeitarServicosSugeridos();
            return;
        }
        
        EnviarParaChecagemEstoque();
    }
    
    private void EnviarParaChecagemEstoque()
    {
        Status = StatusOrdemServico.ChecandoEstoque;
        AprovadaEm = DateTime.UtcNow;
    }

    
    public void ChecarItensNecessarios(Dictionary<int, decimal> saldosDisponiveis)
    {
        if (Status is not (StatusOrdemServico.ChecandoEstoque or StatusOrdemServico.AguardandoPeca))
        {
            throw new InvalidOperationException($"Ordem de Serviço {Id} com status {Status} não pode ter estoque checado.");
        }

        var clonedQuantidadesDisponiveis = new Dictionary<int, decimal>(saldosDisponiveis);

        foreach (var item in ItensNecessariosParaExecucao)
        {
            var saldoAtual = clonedQuantidadesDisponiveis[item.ItemEstoque.Id];
            
            item.ChecarEstoque(saldoAtual);

            if (item.Status == StatusItemEstoque.EstoqueDisponivel)
            {
                clonedQuantidadesDisponiveis[item.ItemEstoque.Id] = saldoAtual - item.Quantidade;
            }
        }

        Status = ItensNecessariosParaExecucao.All(item => item.Status == StatusItemEstoque.EstoqueDisponivel)
            ? StatusOrdemServico.LiberadaParaExecucao
            : StatusOrdemServico.AguardandoPeca;
    }

    public void TravarItensNecessarios()
    {
        foreach (var item in ItensNecessariosParaExecucao)
        {
            item.TravarEstoque();
        }
    }

    public void ConfirmarExecucao(List<ServicoExecutado> servicosExecutados)
    {
        if (Status is not (StatusOrdemServico.LiberadaParaExecucao or StatusOrdemServico.EmExecucao))
        {
            throw new InvalidOperationException($"Ordem de Serviço {Id} com status {Status} não pode ter execução confirmada.");
        }

        foreach (var servicoExecutado in servicosExecutados)
        {
            var servico = _servicos.First(s => s.Id == servicoExecutado.IdServico);
            servico.ConfirmarConclusao(servicoExecutado.IniciadoEm, servicoExecutado.FinalizadoEm);
        }

        if (_servicos.All(servico => servico.Status == StatusItemOrdemServico.Concluido))
        {
            Status = StatusOrdemServico.Finalizada;
        }
    }

    public void ConfirmarPagamento()
    {
        Status = StatusOrdemServico.Entregue;
    }
}