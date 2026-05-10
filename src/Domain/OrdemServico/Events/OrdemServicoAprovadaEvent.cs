using MediatR;

namespace Domain.OrdemServico.Events;

public record OrdemServicoAprovadaEvent(int IdOrdemServico) : INotification;