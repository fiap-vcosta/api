using MediatR;

namespace Application.UseCases.OrdemServico.Commands.EnviarOrdemServicoParaDiagnostico;

public class EnviarOrdemServicoParaDiagnosticoCommand : IRequest
{
    public int IdOrdemServico { get; init; }
}
