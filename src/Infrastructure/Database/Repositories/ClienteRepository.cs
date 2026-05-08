using Domain.Administrativo.Entities;
using Domain.Administrativo.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class ClienteRepository(AppDbContext context) : IClienteRepository
{
    public async Task<ClienteAggregateRoot?> GetByIdAsync(int id)
    {
        return await context.Clientes.FindAsync(id);
    }

    public async Task<IEnumerable<ClienteAggregateRoot>> GetAllAsync()
    {
        return await context.Clientes.ToListAsync();
    }

    public async Task<ClienteAggregateRoot?> GetByDocumentoAsync(string documento)
    {
        return await context.Clientes.FirstOrDefaultAsync(c => c.Documento == documento);
    }

    public async Task CreateAsync(ClienteAggregateRoot clienteAggregateRoot)
    {
        context.Clientes.Add(clienteAggregateRoot);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ClienteAggregateRoot clienteAggregateRoot)
    {
        context.Clientes.Update(clienteAggregateRoot);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var cliente = await context.Clientes.FindAsync(id);
        if (cliente != null)
        {
            context.Clientes.Remove(cliente);
            await context.SaveChangesAsync();
        }
    }
}
