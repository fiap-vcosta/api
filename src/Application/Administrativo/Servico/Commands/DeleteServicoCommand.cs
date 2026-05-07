using MediatR;

namespace Application.Administrativo.Servico.Commands;

public class DeleteServicoCommand : IRequest<Unit>
{
    public int Id { get; init; }
}
