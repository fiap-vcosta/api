using Domain.OrdemServico.Entities;

namespace Application.Abstractions.Services;

public interface IOsMetrics
{
    void IncrementStatus(int idOrdemServico, StatusOrdemServico status);
}
