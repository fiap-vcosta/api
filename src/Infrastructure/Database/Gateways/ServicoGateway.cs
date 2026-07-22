using Application.Abstractions.Gateways;
using Domain.Administrativo.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Gateways;

public class ServicoGateway(AppDbContext context) : IServicoGateway
{
    public async Task<IEnumerable<ServicoAggregateRoot>> GetAllAsync()
    {
        return await context.Servicos.ToListAsync();
    }

    public async Task<ServicoAggregateRoot?> GetByIdAsync(int id)
    {
        return await context.Servicos.FindAsync(id);
    }

    public async Task<ServicoAggregateRoot?> GetByCodigoAsync(string codigo)
    {
        return await context.Servicos.FirstOrDefaultAsync(s => s.Codigo == codigo);
    }

    public async Task CreateAsync(ServicoAggregateRoot servicoAggregateRoot)
    {
        context.Servicos.Add(servicoAggregateRoot);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ServicoAggregateRoot servicoAggregateRoot)
    {
        context.Servicos.Update(servicoAggregateRoot);
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
