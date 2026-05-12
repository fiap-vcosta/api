using Domain.OrdemServico.Entities;
using Domain.OrdemServico.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class ItemServicoRepository(AppDbContext appDbContext) : IItemServicoRepository
{
    public async Task<List<IItemServicoRepository.TempoMedioExecucaoServico>> GetAllTempoMediaExecucaoAsync()
    {
        return await appDbContext.ItensServicos
            .Where(servico => servico.Status == StatusItemOrdemServico.Concluido)
            .GroupBy(s => s.ServicoCatalogo.Id)
            .Select<IGrouping<int, Servico>, IItemServicoRepository.TempoMedioExecucaoServico>(grupo => new IItemServicoRepository.TempoMedioExecucaoServico(
                grupo.Key, grupo.Count(), TimeSpan.FromSeconds(grupo.Average(s => (s.ExecucaoFinalizadaEm - s.ExecucaoIniciadaEm).TotalSeconds))
            )).ToListAsync();
    }
}