using MediatR;

namespace Application.UseCases.Administrativo.Veiculo.Commands.DeleteVeiculo;

public class DeleteVeiculoCommand : IRequest<Unit>
{
    public int Id { get; init; }
}
