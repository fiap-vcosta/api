using MediatR;

namespace Application.UseCases.Administrativo.Servico.Commands.DeleteServico;

public class DeleteServicoCommand : IRequest<Unit>
{
    public int Id { get; init; }
}
