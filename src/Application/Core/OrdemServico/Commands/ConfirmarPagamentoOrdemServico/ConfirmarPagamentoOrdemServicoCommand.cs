using Domain.OrdemServico.Entities;
using MediatR;

namespace Application.Core.OrdemServico.Commands.ConfirmarPagamentoOrdemServico;

public class ConfirmarPagamentoOrdemServicoCommand : IRequest<OrdemServicoAggregateRoot>
{
    public int IdOrdemServico { get; init; }
}