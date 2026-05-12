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
    
    public async Task<IEnumerable<ItemEstoqueAggregateRoot>> GetEBloquearItensAsync(List<int> ids)
    {
        if (ids.Count == 0)
        {
            return new List<ItemEstoqueAggregateRoot>();
        }
        
        var idsOrdenados = ids.Distinct().OrderBy(id => id).ToList();
        var tokens = idsOrdenados.Select((_, index) => $"{{{index}}}");
        var tokensFormatados = string.Join(", ", tokens);
        
        var query = $"SELECT * FROM \"ItensEstoque\" WHERE \"Id\" IN ({tokensFormatados}) FOR UPDATE";

        var itensBloqueados = await context.ItensEstoque
            .FromSqlRaw(query, idsOrdenados.Cast<object>().ToArray())
            .ToListAsync();

        return itensBloqueados;
    }

    public async Task<IEnumerable<ItemEstoqueAggregateRoot>> GetUtilizadosByOrdemServico(int idOrdemServico)
    {
        var itensEstoque = await context.ItensEstoque
            .Where(estoque => context.OrdensServico
                .Where(os => os.Id == idOrdemServico)
                .SelectMany(os => os.Servicos)
                .SelectMany(servico => servico.ItensNecessarios)
                .Select(item => item.ItemEstoque.Id)
                .Contains(estoque.Id))
            .ToListAsync();

        return itensEstoque;
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
