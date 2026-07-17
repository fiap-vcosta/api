using MediatR;

namespace Application.UseCases.OrdemServico.Commands.AlocarEstoqueOrdemServico;

public record AlocarEstoqueOrdemServicoCommand(int IdOrdemServico) : IRequest<Unit>;