using Domain.OrdemServico.Entities;

namespace Domain.OrdemServico.Events;

public record OrdemServicoStatusAlteradoEvent(int IdOrdemServico, StatusOrdemServico Status);
