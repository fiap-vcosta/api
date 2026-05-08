using Application.Administrativo.Servico.Commands.CreateServico;
using MediatR;

namespace Application.Administrativo.Servico.Queries.GetServicoById;

public class GetServicoByIdQuery : IRequest<ServicoResponse?>
{
    public int Id { get; init; }
}
