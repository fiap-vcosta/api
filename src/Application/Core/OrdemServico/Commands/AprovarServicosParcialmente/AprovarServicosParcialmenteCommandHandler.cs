using Domain.OrdemServico.Repositories;
using MediatR;

namespace Application.Core.OrdemServico.Commands.AprovarServicosParcialmente;

public class AprovarServicosParcialmenteCommandHandler(
    IOrdemServicoRepository ordemServicoRepository
): IRequestHandler<AprovarServicosParcialmenteCommand, AprovarServicosParcialmenteCommandResponse>
{
    public async Task<AprovarServicosParcialmenteCommandResponse> Handle(AprovarServicosParcialmenteCommand request, CancellationToken cancellationToken)
    {
        var ordemServico = await ordemServicoRepository.GetByIdAsync(request.IdOrdemServico);
        if (ordemServico == null)
        {
            throw new KeyNotFoundException($"Ordem de Serviço com id {request.IdOrdemServico} não encontrada");
        }

        var idsInvalidos = request.IdServicosAprovados
            .Where(idServicoAprovado => ordemServico.ItensOrdemServico.All(ios => ios.Id != idServicoAprovado))
            .ToList();

        if (idsInvalidos.Count > 0)
        {
            throw new KeyNotFoundException($"Serviços [{string.Join(", ", idsInvalidos)}] não pertencem a Ordem de Serviço {request.IdOrdemServico}");
        }
        
        ordemServico.AprovarServicosParcialmente(request.IdServicosAprovados);
        await ordemServicoRepository.UpdateAsync(ordemServico);

        return new AprovarServicosParcialmenteCommandResponse()
        {
            Id = ordemServico.Id,
            Status = ordemServico.Status,
            ValorTotal = ordemServico.ValorTotal,
            RecebidaEm = ordemServico.RecebidaEm,
            Cliente = ordemServico.Cliente,
            Veiculo = ordemServico.Veiculo,
            Servicos = ordemServico.ItensOrdemServico.ToList()
        };
    }
}