using MediatR;

namespace Application.Core.OrdemServico.Commands.AlocarEstoqueOrdemServico;

public record AlocarEstoqueOrdemServicoCommand(int idOrdemServico) : IRequest<Unit>;