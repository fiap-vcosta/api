using MediatR;

namespace Domain.OrdemServico.Events;

public record DiagnosticoPreenchidoEvent(int IdOrdemServico) : INotification;