using MediatR;

namespace Application.Veiculo.Commands;

public class DeleteVeiculoCommand : IRequest<Unit>
{
    public int Id { get; init; }
}
