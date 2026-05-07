using Application.Administrativo.Servico.Commands;
using MediatR;

namespace Application.Administrativo.Servico.Queries;

public class GetServicoByIdQuery : IRequest<ServicoResponse?>
{
    public int Id { get; init; }
}
