using Domain.Repositories;
using MediatR;

namespace Application.Servico.Commands.Handlers;

public class DeleteServicoCommandHandler(IServicoRepository servicoRepository)
    : IRequestHandler<DeleteServicoCommand, Unit>
{
    public async Task<Unit> Handle(DeleteServicoCommand request, CancellationToken cancellationToken)
    {
        var servico = await servicoRepository.GetByIdAsync(request.Id);
        if (servico == null)
        {
            throw new KeyNotFoundException($"Serviço com id {request.Id} não encontrado");
        }

        await servicoRepository.DeleteAsync(request.Id);
        return Unit.Value;
    }
}
