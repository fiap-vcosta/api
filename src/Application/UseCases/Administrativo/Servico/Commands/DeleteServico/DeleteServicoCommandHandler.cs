using Application.Abstractions.Gateways;
using Domain.Exceptions;

using MediatR;

namespace Application.UseCases.Administrativo.Servico.Commands.DeleteServico;

public class DeleteServicoCommandHandler(IServicoGateway servicoGateway)
    : IRequestHandler<DeleteServicoCommand, Unit>
{
    public async Task<Unit> Handle(DeleteServicoCommand request, CancellationToken cancellationToken)
    {
        var servico = await servicoGateway.GetByIdAsync(request.Id);
        if (servico == null)
        {
            throw new DomainNotFoundException($"Serviço com id {request.Id} não encontrado");
        }

        await servicoGateway.DeleteAsync(request.Id);
        return Unit.Value;
    }
}
