using Application.Abstractions.Gateways;
using Domain.Administrativo.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Gateways;

public class VeiculoGateway(AppDbContext context) : IVeiculoGateway
{
    public async Task<IEnumerable<VeiculoAggregateRoot>> GetAllAsync()
    {
        return await context.Veiculos.ToListAsync();
    }

    public async Task<VeiculoAggregateRoot?> GetByIdAsync(int id)
    {
        return await context.Veiculos.FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<IEnumerable<VeiculoAggregateRoot>> GetByClienteIdAsync(int clienteId)
    {
        return await context.Veiculos.Where(v => v.IdCliente == clienteId).ToListAsync();
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
