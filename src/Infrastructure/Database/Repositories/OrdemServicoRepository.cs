using Domain.OrdemServico.Entities;
using Domain.OrdemServico.Repositories;

namespace Infrastructure.Database.Repositories;

public class OrdemServicoRepository(AppDbContext context) : IOrdemServicoRepository
{
    public async Task CriarAsync(OrdemServicoAggregateRoot ordemServico)
    {
        context.OrdensServico.Add(ordemServico);
        await context.SaveChangesAsync();
    }
}