namespace Application.Abstractions.Gateways;

public interface IItemServicoGateway
{
    public record TempoMedioExecucaoServico(int IdServico, int TotalExecucoes, TimeSpan ExecucaoMedia);
    
    Task<List<TempoMedioExecucaoServico>> GetAllTempoMedioExecucaoAsync();
}
