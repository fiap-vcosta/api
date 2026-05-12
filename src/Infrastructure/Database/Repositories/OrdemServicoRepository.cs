using Domain.OrdemServico.Entities;
using Domain.OrdemServico.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class OrdemServicoRepository(AppDbContext context) : IOrdemServicoRepository
{
    public async Task<IEnumerable<OrdemServicoAggregateRoot>> GetAguardandoPecaPorItemEstoqueAsync(int idItemEstoque)
    {
        return await context.OrdensServico
            .Include(os => os.Servicos)
            .ThenInclude(s => s.ItensNecessarios)
            .Where(os => os.Status == StatusOrdemServico.AguardandoPeca && 
                         os.Servicos.Any(s => s.ItensNecessarios.Any(i => i.ItemEstoque.Id == idItemEstoque && i.Status == StatusItemEstoque.EstoqueEmFalta)))
            .OrderBy(os => os.AprovadaEm)
            .ToListAsync();
    }

    public async Task CriarAsync(OrdemServicoAggregateRoot ordemServico)
    {
        context.OrdensServico.Add(ordemServico);
        await context.SaveChangesAsync();
    }

    public async Task<OrdemServicoAggregateRoot?> GetByIdAsync(int IdOrdemServico)
    {
        return await context.OrdensServico
            .Include(os => os.Servicos)
            .ThenInclude(ios => ios.ItensNecessarios)
            .FirstOrDefaultAsync(os => os.Id == IdOrdemServico);
    }

    public async Task UpdateAsync(OrdemServicoAggregateRoot ordemServico)
    {
        context.OrdensServico.Update(ordemServico);
        await context.SaveChangesAsync();
    }
}