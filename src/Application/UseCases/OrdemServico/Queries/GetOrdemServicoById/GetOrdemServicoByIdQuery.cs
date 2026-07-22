using MediatR;

namespace Application.UseCases.OrdemServico.Queries.GetOrdemServicoById;

public class GetOrdemServicoByIdQuery : IRequest<OrdemServicoResponse?>
{
    public int Id { get; init; }
}
