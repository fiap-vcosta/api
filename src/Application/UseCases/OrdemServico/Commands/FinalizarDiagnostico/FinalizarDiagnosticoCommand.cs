using MediatR;

namespace Application.UseCases.OrdemServico.Commands.FinalizarDiagnostico;

public class FinalizarDiagnosticoCommand : IRequest<FinalizarDiagnosticoCommandResponse>
{
    public int IdOrdemServico { get; init; }
}