using MediatR;

namespace Domain.OrdemServico.Events;

public record OrdemServicoRejeitadaEvent(int IdOrdemServico) : INotification;