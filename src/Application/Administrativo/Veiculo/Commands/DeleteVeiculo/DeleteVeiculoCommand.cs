using MediatR;

namespace Application.Administrativo.Veiculo.Commands.DeleteVeiculo;

public class DeleteVeiculoCommand : IRequest<Unit>
{
    public int Id { get; init; }
}
