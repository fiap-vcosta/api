using MediatR;

namespace Application.Servico.Commands;

public class DeleteServicoCommand : IRequest<Unit>
{
    public int Id { get; init; }
}
