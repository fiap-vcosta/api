using Domain.OrdemServico.Entities;
using Domain.OrdemServico.ValueObjects;
using MediatR;

namespace Application.Core.OrdemServico.Commands.ConfirmarExecucaoOrdemServico;

public class ConfirmarExecucaoOrdemServicoCommand : IRequest<OrdemServicoAggregateRoot>
{
    public int IdOrdemServico { get; init; }
    public List<ServicoExecutado> ServicoExecutados { get; init; } = [];
}