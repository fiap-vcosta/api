using Application.Abstractions.Gateways;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Gateways;

public class ItemServicoGateway(AppDbContext appDbContext) : IItemServicoGateway
{
    public async Task<List<IItemServicoGateway.TempoMedioExecucaoServico>> GetAllTempoMedioExecucaoAsync()
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
            .Select(grupo => new IItemServicoGateway.TempoMedioExecucaoServico(
                grupo.Key,
                grupo.Count(),
                TimeSpan.FromSeconds(grupo.Average(s => (s.Fim - s.Inicio).TotalSeconds))))
            .ToList();
    }
}
