using Application.Servico.Commands;
using MediatR;

namespace Application.Servico.Queries;

public class GetServicoByIdQuery : IRequest<ServicoResponse?>
{
    public int Id { get; init; }
}
