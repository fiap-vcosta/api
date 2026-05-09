using MediatR;

namespace Domain.OrdemServico.Events;

public record OrdemServicoRecebidaDiagnosticoEvent(int IdOrdemServico) : INotification;