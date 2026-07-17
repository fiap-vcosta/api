namespace Application.Abstractions.Gateways;

public interface IItemServicoGateway
{
    public record TempoMedioExecucaoServico(int idServico, int totalExecucoes, TimeSpan execucaoMedia);
    
    Task<List<TempoMedioExecucaoServico>> GetAllTempoMedioExecucaoAsync();
}
