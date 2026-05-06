using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class ClienteRepository(AppDbContext context) : IClienteRepository
{
    public async Task<Cliente?> GetByIdAsync(int id)
    {
        return await context.Clientes.FindAsync(id);
    }

    public async Task<IEnumerable<Cliente>> GetAllAsync()
    {
        return await context.Clientes.ToListAsync();
    }

    public async Task<Cliente?> GetByDocumentoAsync(string documento)
    {
        return await context.Clientes.FirstOrDefaultAsync(c => c.Documento == documento);
    }

    public async Task CreateAsync(Cliente cliente)
    {
        context.Clientes.Add(cliente);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Cliente cliente)
    {
        context.Clientes.Update(cliente);
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
