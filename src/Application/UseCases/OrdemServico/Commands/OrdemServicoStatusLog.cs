using Domain.OrdemServico.Entities;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.OrdemServico.Commands;

public static partial class OrdemServicoStatusLog
{
    [LoggerMessage(EventId = 1000, Level = LogLevel.Information, Message = "OS {OrdemServicoId} movida para o status {Status}")]
    public static partial void Emit(ILogger logger, int ordemServicoId, StatusOrdemServico status);
}
