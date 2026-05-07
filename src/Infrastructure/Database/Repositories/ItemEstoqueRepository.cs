using Domain.Estoque.Entities;
using Domain.Estoque.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class ItemEstoqueRepository(AppDbContext context) : IItemEstoqueRepository
{
    public async Task<IEnumerable<ItemEstoque>> GetAllAsync()
    {
        return await context.ItensEstoque.ToListAsync();
    }

    public async Task<ItemEstoque?> GetByIdAsync(int id)
    {
        return await context.ItensEstoque.FindAsync(id);
    }

    public async Task<ItemEstoque?> GetByCodigoAsync(string codigo)
    {
        return await context.ItensEstoque.FirstOrDefaultAsync(i => i.Codigo == codigo);
    }

    public async Task CreateAsync(ItemEstoque itemEstoque)
    {
        context.ItensEstoque.Add(itemEstoque);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ItemEstoque itemEstoque)
    {
        context.ItensEstoque.Update(itemEstoque);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var itemEstoque = await context.ItensEstoque.FindAsync(id);
        if (itemEstoque != null)
        {
            context.ItensEstoque.Remove(itemEstoque);
            await context.SaveChangesAsync();
        }
    }
}
