using Domain.Administrativo.Entities;
using Domain.Administrativo.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class VeiculoRepository(AppDbContext context) : IVeiculoRepository
{
    public async Task<IEnumerable<VeiculoAggregateRoot>> GetAllAsync()
    {
        return await context.Veiculos.Include(v => v.Dono).ToListAsync();
    }

    public async Task<VeiculoAggregateRoot?> GetByIdAsync(int id)
    {
        return await context.Veiculos.Include(v => v.Dono).FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<IEnumerable<VeiculoAggregateRoot>> GetByDonoIdAsync(int donoId)
    {
        return await context.Veiculos.Where(v => v.DonoId == donoId).ToListAsync();
    }

    public async Task<VeiculoAggregateRoot?> GetByPlacaAsync(string placa)
    {
        return await context.Veiculos.FirstOrDefaultAsync(v => v.Placa == placa);
    }

    public async Task CreateAsync(VeiculoAggregateRoot veiculoAggregateRoot)
    {
        context.Veiculos.Add(veiculoAggregateRoot);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(VeiculoAggregateRoot veiculoAggregateRoot)
    {
        context.Veiculos.Update(veiculoAggregateRoot);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var veiculo = await context.Veiculos.FindAsync(id);
        if (veiculo != null)
        {
            context.Veiculos.Remove(veiculo);
            await context.SaveChangesAsync();
        }
    }
}
