using Domain.Estoque.Entities;
using Domain.Estoque.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class ItemEstoqueRepository(AppDbContext context) : IItemEstoqueRepository
{
    public async Task<IEnumerable<ItemEstoqueAggregateRoot>> GetAllAsync()
    {
        return await context.ItensEstoque.ToListAsync();
    }

    public async Task<ItemEstoqueAggregateRoot?> GetByIdAsync(int id)
    {
        return await context.ItensEstoque.FindAsync(id);
    }

    public async Task<ItemEstoqueAggregateRoot?> GetByCodigoAsync(string codigo)
    {
        return await context.ItensEstoque.FirstOrDefaultAsync(i => i.Codigo == codigo);
    }

    public async Task CreateAsync(ItemEstoqueAggregateRoot itemEstoqueAggregateRoot)
    {
        context.ItensEstoque.Add(itemEstoqueAggregateRoot);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ItemEstoqueAggregateRoot itemEstoqueAggregateRoot)
    {
        context.ItensEstoque.Update(itemEstoqueAggregateRoot);
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
