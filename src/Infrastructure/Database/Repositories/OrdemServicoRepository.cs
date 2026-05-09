using Domain.OrdemServico.Entities;
using Domain.OrdemServico.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class OrdemServicoRepository(AppDbContext context) : IOrdemServicoRepository
{
    public async Task CriarAsync(OrdemServicoAggregateRoot ordemServico)
    {
        context.OrdensServico.Add(ordemServico);
        await context.SaveChangesAsync();
    }

    public async Task<OrdemServicoAggregateRoot?> GetByIdAsync(int IdOrdemServico)
    {
        return await context.OrdensServico.FirstOrDefaultAsync(os => os.Id == IdOrdemServico);
    }

    public async Task UpdateAsync(OrdemServicoAggregateRoot ordemServico)
    {
        context.OrdensServico.Update(ordemServico);
        await context.SaveChangesAsync();
    }
}