using MediatR;

namespace Domain.OrdemServico.Events;

public class OrdemServicoDescartadaEvent(int IdOrdemServico) : INotification;