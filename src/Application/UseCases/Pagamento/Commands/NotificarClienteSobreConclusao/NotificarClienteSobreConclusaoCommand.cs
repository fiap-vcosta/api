using MediatR;

namespace Application.UseCases.Pagamento.Commands.NotificarClienteSobreConclusao;

public class NotificarClienteSobreConclusaoCommand : IRequest<Unit>
{
    public int IdCliente { get; init; }
    public int IdOrdemServico { get; init; }
}
