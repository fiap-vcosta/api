using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class VeiculoRepository(AppDbContext context) : IVeiculoRepository
{
    public async Task<IEnumerable<Veiculo>> GetAllAsync()
    {
        return await context.Veiculos.Include(v => v.Dono).ToListAsync();
    }

    public async Task<Veiculo?> GetByIdAsync(int id)
    {
        return await context.Veiculos.Include(v => v.Dono).FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<IEnumerable<Veiculo>> GetByDonoIdAsync(int donoId)
    {
        return await context.Veiculos.Where(v => v.DonoId == donoId).ToListAsync();
    }

    public async Task<Veiculo?> GetByPlacaAsync(string placa)
    {
        return await context.Veiculos.FirstOrDefaultAsync(v => v.Placa == placa);
    }

    public async Task CreateAsync(Veiculo veiculo)
    {
        context.Veiculos.Add(veiculo);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Veiculo veiculo)
    {
        context.Veiculos.Update(veiculo);
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
