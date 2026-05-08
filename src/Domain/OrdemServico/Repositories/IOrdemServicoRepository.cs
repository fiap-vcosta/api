using Domain.OrdemServico.Entities;

namespace Domain.OrdemServico.Repositories;

public interface IOrdemServicoRepository
{
    Task CriarAsync(OrdemServicoAggregateRoot ordemServico);
}