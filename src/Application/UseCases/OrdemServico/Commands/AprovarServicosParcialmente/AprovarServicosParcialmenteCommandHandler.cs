using Application.Abstractions.Gateways;
using Application.UseCases.OrdemServico.Responses;
using Domain.Exceptions;
using MediatR;

namespace Application.UseCases.OrdemServico.Commands.AprovarServicosParcialmente;

public class AprovarServicosParcialmenteCommandHandler(
    IOrdemServicoGateway ordemServicoGateway
): IRequestHandler<AprovarServicosParcialmenteCommand, AprovarServicosParcialmenteCommandResponse>
{
    public async Task<AprovarServicosParcialmenteCommandResponse> Handle(AprovarServicosParcialmenteCommand request, CancellationToken cancellationToken)
    {
        var ordemServico = await ordemServicoGateway.GetByIdAsync(request.IdOrdemServico);
        if (ordemServico == null)
        {
            throw new DomainNotFoundException($"Ordem de Serviço com id {request.IdOrdemServico} não encontrada");
        }

        var idsInvalidos = request.IdServicosAprovados
            .Where(idServicoAprovado => ordemServico.Servicos.All(ios => ios.Id != idServicoAprovado))
            .ToList();

        if (idsInvalidos.Count > 0)
        {
            throw new DomainNotFoundException($"Serviços [{string.Join(", ", idsInvalidos)}] não pertencem a Ordem de Serviço {request.IdOrdemServico}");
        }
        
        ordemServico.AprovarServicosParcialmente(request.IdServicosAprovados);
        await ordemServicoGateway.UpdateAsync(ordemServico);

        return new AprovarServicosParcialmenteCommandResponse()
        {
            Id = ordemServico.Id,
            Status = ordemServico.Status,
            ValorTotal = ordemServico.ValorTotal,
            RecebidaEm = ordemServico.RecebidaEm,
            Cliente = ClienteOrdemServicoResponse.From(ordemServico.Cliente),
            Veiculo = VeiculoOrdemServicoResponse.From(ordemServico.Veiculo),
            Servicos = ServicoOrdemServicoResponse.FromMany(ordemServico.Servicos)
        };
    }
}