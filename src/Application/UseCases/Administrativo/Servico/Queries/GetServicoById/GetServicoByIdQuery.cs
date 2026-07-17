using Application.UseCases.Administrativo.Servico.Responses;
using MediatR;

namespace Application.UseCases.Administrativo.Servico.Queries.GetServicoById;

public class GetServicoByIdQuery : IRequest<ServicoResponse?>
{
    public int Id { get; init; }
}
