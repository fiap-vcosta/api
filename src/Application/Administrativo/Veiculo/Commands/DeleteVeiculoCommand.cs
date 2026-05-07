using MediatR;

namespace Application.Administrativo.Veiculo.Commands;

public class DeleteVeiculoCommand : IRequest<Unit>
{
    public int Id { get; init; }
}
