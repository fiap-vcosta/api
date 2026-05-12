using Domain.OrdemServico.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class ItemServicoRepository(AppDbContext appDbContext) : IItemServicoRepository
{
    public async Task<List<IItemServicoRepository.TempoMedioExecucaoServico>> GetAllTempoMediaExecucaoAsync()
    {
        return await appDbContext.ItensServicos
            .Where(servico => servico.ExecucaoIniciadaEm != null && servico.ExecucaoFinalizadaEm != null)
            .GroupBy(s => s.ServicoCatalogo.Id)
            .Select(grupo => new IItemServicoRepository.TempoMedioExecucaoServico(
                grupo.Key, grupo.Count(), TimeSpan.FromSeconds(grupo.Average(s => (s.ExecucaoFinalizadaEm!.Value.Subtract(s.ExecucaoIniciadaEm!.Value)).TotalSeconds))
            )).ToListAsync();
    }
}