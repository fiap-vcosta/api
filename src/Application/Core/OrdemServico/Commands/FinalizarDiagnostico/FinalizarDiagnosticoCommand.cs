using Domain.OrdemServico.Entities;
using Domain.OrdemServico.ValueObjects;
using MediatR;

namespace Application.Core.OrdemServico.Commands.FinalizarDiagnostico;

public class FinalizarDiagnosticoCommand : IRequest<FinalizarDiagnosticoCommandResponse>
{
    public int IdOrdemServico { get; init; }
}