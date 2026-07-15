using Domain.OrdemServico.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class ItemServicoRepository(AppDbContext appDbContext) : IItemServicoRepository
{
    public async Task<List<IItemServicoRepository.TempoMedioExecucaoServico>> GetAllTempoMediaExecucaoAsync()
    {
        var conclusoes = await appDbContext.ItensServicos
            .Where(servico => servico.ExecucaoIniciadaEm != null && servico.ExecucaoFinalizadaEm != null)
            .Select(s => new
            {
                ServicoId = s.ServicoCatalogo.Id,
                Inicio = s.ExecucaoIniciadaEm!.Value,
                Fim = s.ExecucaoFinalizadaEm!.Value
            })
            .ToListAsync();

        return conclusoes
            .GroupBy(s => s.ServicoId)
            .Select(grupo => new IItemServicoRepository.TempoMedioExecucaoServico(
                grupo.Key,
                grupo.Count(),
                TimeSpan.FromSeconds(grupo.Average(s => (s.Fim - s.Inicio).TotalSeconds))))
            .ToList();
    }
}
