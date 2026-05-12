namespace Domain.OrdemServico.Repositories;

public interface IItemServicoRepository
{
    public record TempoMedioExecucaoServico(int idServico, int totalExecucoes, TimeSpan execucaoMedia);
    
    Task<List<TempoMedioExecucaoServico>> GetAllTempoMediaExecucaoAsync();
}