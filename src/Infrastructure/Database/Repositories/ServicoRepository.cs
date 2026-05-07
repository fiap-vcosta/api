using Domain.Administrativo.Entities;
using Domain.Administrativo.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class ServicoRepository(AppDbContext context) : IServicoRepository
{
    public async Task<IEnumerable<Servico>> GetAllAsync()
    {
        return await context.Servicos.ToListAsync();
    }

    public async Task<Servico?> GetByIdAsync(int id)
    {
        return await context.Servicos.FindAsync(id);
    }

    public async Task<Servico?> GetByCodigoAsync(string codigo)
    {
        return await context.Servicos.FirstOrDefaultAsync(s => s.Codigo == codigo);
    }

    public async Task CreateAsync(Servico servico)
    {
        context.Servicos.Add(servico);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Servico servico)
    {
        context.Servicos.Update(servico);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var servico = await context.Servicos.FindAsync(id);
        if (servico != null)
        {
            context.Servicos.Remove(servico);
            await context.SaveChangesAsync();
        }
    }
}
