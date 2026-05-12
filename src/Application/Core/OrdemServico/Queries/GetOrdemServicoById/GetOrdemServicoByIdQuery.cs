using Domain.OrdemServico.Entities;
using MediatR;

namespace Application.Core.OrdemServico.Queries.GetOrdemServicoById;

public class GetOrdemServicoByIdQuery : IRequest<OrdemServicoAggregateRoot?>
{
    public int Id { get; init; }
}